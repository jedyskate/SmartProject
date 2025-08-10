using SmartConfig.Sdk;
using TickerQ.Utilities.Base;
using TickerQ.Utilities.Models;

namespace SmartConfig.Scheduler.Jobs;

public class Orleans
{
    private readonly ILogger<Orleans> _logger;
    private readonly ISmartConfigClient _smartConfigClient;
    private readonly ISmartConfigQueue _smartConfigQueue;

    public Orleans(ILogger<Orleans> logger, ISmartConfigClient smartConfigClient, ISmartConfigQueue smartConfigQueue)
    {
        _logger = logger;
        _smartConfigClient = smartConfigClient;
        _smartConfigQueue = smartConfigQueue;
    }

    [TickerFunction("HelloWorld", "*/1 * * * *")]
    public async Task HelloWorld(TickerFunctionContext<string> context)
    {
        _logger.LogInformation($"Hello World Context: {context}");

        var response = await _smartConfigClient.HelloWorldAsync(new HelloWorldCommand
        {
            Name = string.IsNullOrEmpty(context.Request) ? "Scheduler" : context.Request
        });
        var message = response.Response.Contains("Hello world");
        
        _logger.LogInformation($"Hello World Message Result: {message}");
    }
    
    [TickerFunction("QueueHelloWorld", "*/1 * * * *")]
    public async Task QueueHelloWorld()
    {
        _logger.LogInformation($"Queuing Hello World");
    
        var response = await _smartConfigQueue.Request(new HelloWorldCommand
        {
            Name = "Unattended Scheduler"
        });
        
        _logger.LogInformation($"Hello World Queue Response: {response.Message}");
    }
}