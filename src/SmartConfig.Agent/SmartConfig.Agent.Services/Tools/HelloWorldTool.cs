using System.ComponentModel;
using Microsoft.Extensions.Logging;
using SmartConfig.BE.Sdk;

namespace SmartConfig.Agent.Services.Tools;

public class HelloWorldTool(ILogger<HelloWorldTool> logger, ISmartConfigApiClient smartConfigApiClient)
{
    [Description("Says hello to a user")]
    public async Task<string> SayHelloAsync([Description("The name of the user to greet")] string name)
    {
        logger.LogInformation($"Hello World Name: {name}");

        var result = await smartConfigApiClient.HelloWorldAsync(new HelloWorldCommand
        {
            Name = name
        });

        logger.LogInformation($"Hello World Message Result: {result.Response}");
        
        return result.Response;
    }
}