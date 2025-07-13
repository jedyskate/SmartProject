using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SmartConfig.Common.Models;

public class ConfigOrder
{
    public string OrderBy { get; set; }
    public ConfigSearchOrderType? OrderType { get; set; }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum ConfigSearchOrderType
{
    Ascending = 0,
    Descending = 1
}