using System.ComponentModel;
using ModelContextProtocol.Server;
using SmartConfig.Sdk;

namespace SmartConfig.McpServer.Tools;

[McpServerToolType]
public sealed class OrleansTool
{
    private readonly ISmartConfigClient _smartConfigClient;

    public OrleansTool(ISmartConfigClient smartConfigClient)
    {
        _smartConfigClient = smartConfigClient;
    }

    [McpServerTool]
    [Description("Say hello to the orlenas backend.")]
    public async Task<string> HelloOrleans([Description("Name who's sying hello.")] string name)
    {
        var response = await _smartConfigClient.HelloWorldAsync(new HelloWorldCommand { Name = name });
        
        return response.Response;
    }
}