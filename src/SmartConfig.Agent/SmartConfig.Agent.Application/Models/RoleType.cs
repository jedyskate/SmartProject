using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SmartConfig.AiAgent.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum RoleType
{
    None,
    System,
    User,
    Assistant,
    Tool,
}
