using System.ComponentModel;
using ModelContextProtocol.Server;

namespace SmartConfig.McpServer.Tools;

[McpServerToolType]
[Description("Orleans tool help to validate SmartConfig backend is up.")]
public sealed class OrleansTool
{
    [McpServerTool]
    [Description("Say hello to the orlenas backend.")]
    public static string HelloOrleans([Description("Name of the person sying hello.")] string name)
    {
        return "Hello " + name;
    }
}