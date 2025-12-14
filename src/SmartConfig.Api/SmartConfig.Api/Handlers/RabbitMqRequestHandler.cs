using System.Diagnostics;
using System.Text;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SmartConfig.Application.Telemetry;

namespace SmartConfig.Api.Handlers;

public class RabbitMqRequestHandler : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;
    private IConnection _connection;
    private IModel _channel;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RabbitMqRequestHandler(IConfiguration configuration, ILoggerFactory loggerFactory, 
        IConnection connection, IServiceScopeFactory serviceScopeFactory)
    {
        _configuration = configuration;
        _logger = loggerFactory.CreateLogger<RabbitMqRequestHandler>();
        _connection = connection;
        _serviceScopeFactory = serviceScopeFactory;

        InitRabbitMq();
    }

    private string Exchange => $"{_configuration["RabbitMq:SmartConfigMq:Exchange"]!}.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}";
    private string Queue => $"{_configuration["RabbitMq:SmartConfigMq:Queue"]!}.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}";
    private string RoutingKey => $"{_configuration["RabbitMq:SmartConfigMq:RoutingKey"]!}.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}";

    private void InitRabbitMq()
    {
        try
        {
            // create channel
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(Exchange, ExchangeType.Direct, true);
            _channel.QueueDeclare(Queue, true, false, false, null);
            _channel.QueueBind(Queue, Exchange, RoutingKey, null);
            _channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

            _logger.LogInformation("RabbitMQ initialized successfully. Exchange: {Exchange}, Queue: {Queue}, RoutingKey: {RoutingKey}",
                Exchange, Queue, RoutingKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ connection error. Exchange: {Exchange}, Queue: {Queue}", Exchange, Queue);
            throw;
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (ch, ea) =>
        {
            var stopwatch = Stopwatch.StartNew();

            string content = Encoding.UTF8.GetString(ea.Body.ToArray());
            SmartConfigQueueMessage message = JsonConvert.DeserializeObject<SmartConfigQueueMessage>(content)!;

            // Extract trace context from message headers if available
            ActivityContext parentContext = default;
            if (message.Headers != null && message.Headers.TryGetValue("traceparent", out var traceparent))
            {
                ActivityContext.TryParse(traceparent, null, out parentContext);
            }

            // Start activity for distributed tracing
            using var activity = ApplicationTelemetry.ActivitySource.StartActivity(
                $"RabbitMQ.Consume {message.OperationName}",
                ActivityKind.Consumer,
                parentContext);

            activity?.SetTag("messaging.system", "rabbitmq");
            activity?.SetTag("messaging.destination", Queue);
            activity?.SetTag("messaging.operation", message.OperationName);

            try
            {
                // handle the received message
                await HandleMessage(message);
                _channel.BasicAck(ea.DeliveryTag, false);

                stopwatch.Stop();
                ApplicationTelemetry.MessagePublishDurationHistogram.Record(stopwatch.Elapsed.TotalMilliseconds,
                    new KeyValuePair<string, object?>("operation", message.OperationName),
                    new KeyValuePair<string, object?>("status", "success"));
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                ApplicationTelemetry.MessagesPublishFailedCounter.Add(1,
                    new KeyValuePair<string, object?>("operation", message.OperationName),
                    new KeyValuePair<string, object?>("error", ex.GetType().Name));

                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                _logger.LogError(ex, "Failed to process RabbitMQ message: {OperationName}", message.OperationName);

                // Nack the message so it can be requeued
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        consumer.Shutdown += OnConsumerShutdown;
        consumer.Registered += OnConsumerRegistered;
        consumer.Unregistered += OnConsumerUnregistered;
        consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

        _channel.BasicConsume(Queue, false, consumer);
        return Task.CompletedTask;
    }

    private async Task HandleMessage(SmartConfigQueueMessage message)
    {
        _logger.LogInformation("Processing RabbitMQ message: {OperationName}", message.OperationName);

        try
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                IMediator scopedService = scope.ServiceProvider.GetRequiredService<IMediator>();

                var type = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .FirstOrDefault(t => t.Name == message.OperationName);

                if (type == null)
                {
                    _logger.LogError("Operation type not found: {OperationName}", message.OperationName);
                    throw new InvalidOperationException($"Operation type not found: {message.OperationName}");
                }

                object command = message.Variables.ToObject(type)!;
                var response = await scopedService.Send(command);

                _logger.LogInformation("Successfully processed RabbitMQ message: {OperationName}", message.OperationName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Consumer handler error: {ex.Message}");
            //throw; //We should not throw because this stop RabbitMq worker
        }
    }

    private Task OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { return Task.CompletedTask; }
    private Task OnConsumerUnregistered(object sender, ConsumerEventArgs e) { return Task.CompletedTask; }
    private Task OnConsumerRegistered(object sender, ConsumerEventArgs e) { return Task.CompletedTask; }
    private Task OnConsumerShutdown(object sender, ShutdownEventArgs e) { return Task.CompletedTask; }
    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}

public class SmartConfigQueueMessage
{
    public string OperationName { get; set; }
    public JObject Variables { get; set; }

    public IDictionary<string, string>? Headers { get; set; }
}