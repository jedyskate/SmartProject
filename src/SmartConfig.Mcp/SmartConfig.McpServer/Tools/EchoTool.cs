using System.ComponentModel;
using ModelContextProtocol.Server;

namespace SmartConfig.McpServer.Tools;

[McpServerToolType]
public sealed class EchoTool
{
    [McpServerTool]
    [Description("Echoes the input back to the client.")]
    public string Echo([Description("The message to echo back.")] string message)
    {
        return "Hello " + message;
    }
}