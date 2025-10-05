using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SmartConfig.BE.Sdk;

namespace SmartConfig.AiAgent.Plugins;

public class HelloWorldPlugin(ILogger<HelloWorldPlugin> logger, ISmartConfigApiClient smartConfigApiClient)
{
    [KernelFunction("say_hello")]
    [Description("Says hello to a user using Orleans grain")]
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