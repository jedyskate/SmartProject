using System.ComponentModel;
using ModelContextProtocol.Server;

namespace SmartConfig.McpServer.Tools;

[McpServerToolType]
[Description("Echo tool to check MCP server health.")]
public sealed class EchoTool
{
    [McpServerTool]
    [Description("Echoes the input back to the client.")]
    public static string Echo([Description("The name to echo back.")] string name)
    {
        return "Hello " + name;
    }
}