using System.ComponentModel;
using ModelContextProtocol.Server;
using SmartConfig.BE.Sdk;

namespace SmartConfig.McpServer.Tools;

[McpServerToolType]
public sealed class OrleansTool
{
    private readonly ISmartConfigApiClient _smartConfigApiClient;

    public OrleansTool(ISmartConfigApiClient smartConfigApiClient)
    {
        _smartConfigApiClient = smartConfigApiClient;
    }

    [McpServerTool]
    [Description("Say hello to the orlenas backend.")]
    public async Task<string> HelloOrleans([Description("Name who's sying hello.")] string name)
    {
        var result = await _smartConfigApiClient.HelloWorldAsync(new HelloWorldCommand { Name = name });
        
        return result.Response;
    }
}