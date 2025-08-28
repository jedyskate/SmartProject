using System.ComponentModel;
using ModelContextProtocol.Server;

namespace SmartConfig.McpServer.Tools;

[McpServerToolType]
public sealed class OrleansTool
{
    [McpServerTool]
    [Description("Say hello to the orlenas backend.")]
    public string HelloOrleans([Description("Name who's sying hello.")] string name)
    {
        return "Hello " + name;
    }
}