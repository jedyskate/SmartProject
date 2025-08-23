using System.ComponentModel;
using ModelContextProtocol.Server;

namespace SmartConfig.McpServer.Tools;

[McpServerToolType]
public static class Orleans
{
    [McpServerTool(Name = "SayHelloToBackend")]
    [Description("Say hello to backend")]
    public static string HelloOrleans(string name, CancellationToken cancellationToken)
    {
        return $"hello {name}";
    }
}