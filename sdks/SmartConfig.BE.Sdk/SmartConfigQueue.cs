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
        private readonly SmartConfigApiSettings _apiSettings;
        private readonly ISmartConfigQueueManager _queueManager;

        public SmartConfigQueue(SmartConfigApiSettings apiSettings, ISmartConfigQueueManager queueManager)
        {
            _apiSettings = apiSettings;
            _queueManager = queueManager;
        }

        public Task<QueueResponse> Request<T>(T command, string? accessToken = null)
        {
            if (_apiSettings.DryRun)
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

                if (!string.IsNullOrEmpty(_apiSettings.ApplicationName))
                    headers.Add("AppId", _apiSettings.ApplicationName);

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
