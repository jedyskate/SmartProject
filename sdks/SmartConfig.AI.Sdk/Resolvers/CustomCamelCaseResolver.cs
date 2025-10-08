using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SmartConfig.AI.Sdk.Resolvers;

public class CustomCamelCaseResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo info, MemberSerialization memberSerialization)
    {
        var jsonProp = base.CreateProperty(info, memberSerialization);
        jsonProp.Required = Required.Default;

        return jsonProp;
    }
}