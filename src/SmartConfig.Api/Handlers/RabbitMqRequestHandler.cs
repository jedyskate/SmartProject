using System.Text;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
        }
        catch (Exception ex)
        {
            _logger.LogError($"RabbitMq connection error: {ex.Message}");
            throw;
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (ch, ea) =>
        {
            string content = Encoding.UTF8.GetString(ea.Body.ToArray());
            SmartConfigQueueMessage message = JsonConvert.DeserializeObject<SmartConfigQueueMessage>(content)!;

            // handle the received message
            await HandleMessage(message);
            _channel.BasicAck(ea.DeliveryTag, false);
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
        try
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                IMediator scopedService = scope.ServiceProvider.GetRequiredService<IMediator>();

                var type = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .FirstOrDefault(t => t.Name == message.OperationName);

                object command = message.Variables.ToObject(type!)!;
                var response = await scopedService.Send(command!);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Consumer handler error: {ex.Message}");
            //throw; //We should not throw because this stop RabbitMq worker
        }

        // We just print this message   
        _logger.LogInformation($"Consumer message received: {message.OperationName}");
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