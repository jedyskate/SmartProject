using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace SmartConfig.BE.Sdk.Queue
{
    public interface ISmartConfigQueueManager
    {
        void SendSmartConfigMessage<T>(T message) where T : class;
    }

    public class SmartConfigQueueManager : ISmartConfigQueueManager
    {
        private readonly ILogger _logger;
        private readonly SmartConfigQueueSettings _settings;
        private readonly IConnection _connection;

        public SmartConfigQueueManager(ILoggerFactory loggerFactory, SmartConfigQueueSettings settings, IConnection connection)
        {
            _logger = loggerFactory.CreateLogger<SmartConfigQueueManager>();
            _settings = settings;
            _connection = connection;
        }

        public void SendSmartConfigMessage<T>(T message) where T : class
        {
            if (message == null)
                return;

            try
            {
                using var channel = _connection.CreateModel();
                channel.QueueDeclare($"{_settings.Queue}.{_settings.Environment}", true, false, false, null);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                channel.BasicPublish($"{_settings.Exchange}.{_settings.Environment}",
                    $"{_settings.RoutingKey}.{_settings.Environment}", properties, body);
            }
            catch (Exception ex)
            {
                // Logging exception   
                _logger.LogError($"Producer error: {ex.Message}");
            }
        }
    }
}
