using Newtonsoft.Json.Linq;

namespace SmartConfig.BE.Sdk.Queue
{
    public class SmartConfigQueueMessage
    {
        public string OperationName { get; set; }
        public JObject Variables { get; set; }

        public IDictionary<string, string> Headers { get; set; }
    }
}
