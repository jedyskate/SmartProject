using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartConfig.BE.Sdk.Queue;

namespace SmartConfig.BE.Sdk
{
    public interface ISmartConfigQueue
    {
        Task<QueueResponse> Request<T>(T command, string? accessToken = null);
    }

    public class SmartConfigQueue : ISmartConfigQueue
    {
        private readonly SmartConfigSettings _settings;
        private readonly ISmartConfigQueueManager _queueManager;

        public SmartConfigQueue(SmartConfigSettings settings, ISmartConfigQueueManager queueManager)
        {
            _settings = settings;
            _queueManager = queueManager;
        }

        public Task<QueueResponse> Request<T>(T command, string? accessToken = null)
        {
            if (_settings.DryRun)
            {
                return Task.FromResult(new QueueResponse
                {
                    Message = "DryRun",
                    Code = HttpStatusCode.Accepted
                });
            }
            if (!typeof(T).Name.ToLower().Contains("command"))
            {
                return Task.FromResult(new QueueResponse
                {
                    Message = "AccessToken is not a command.",
                    Code = HttpStatusCode.BadRequest
                });
            }

            try
            {
                var headers = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(accessToken))
                    headers.Add("Authorization", accessToken);

                if (!string.IsNullOrEmpty(_settings.ApplicationName))
                    headers.Add("AppId", _settings.ApplicationName);

                _queueManager.SendSmartConfigMessage(new SmartConfigQueueMessage
                {
                    OperationName = typeof(T).Name,
                    Variables = JObject.Parse(JsonConvert.SerializeObject(command)),
                    Headers = headers
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new QueueResponse
                {
                    Message = $"Queue exception: {ex.Message}",
                    Code = HttpStatusCode.BadRequest
                });
            }

            return Task.FromResult(new QueueResponse
            {
                Message = "Message queued",
                Code = HttpStatusCode.Accepted
            });
        }
    }

    public class QueueResponse
    {
        public string Message { get; set; }
        public HttpStatusCode Code { get; set; }
    }
}
