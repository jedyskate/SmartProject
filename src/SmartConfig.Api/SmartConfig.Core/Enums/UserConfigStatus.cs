using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SmartConfig.Core.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum UserConfigStatus
{
    Inactive,
    Active,
    Deleted
}