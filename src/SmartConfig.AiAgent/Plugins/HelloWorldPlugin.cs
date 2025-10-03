using System.ComponentModel;
using Microsoft.SemanticKernel;
using SmartConfig.Orleans.Silo.Grains.Tests;

namespace SmartConfig.AiAgent.Plugins;

public class HelloWorldPlugin(IClusterClient clusterClient)
{
    [KernelFunction("say_hello")]
    [Description("Says hello to a user using Orleans grain")]
    public async Task<string> SayHelloAsync([Description("The name of the user to greet")] string name)
    {
        var grain = clusterClient.GetGrain<IHelloWorldUserGrain>($"hello-grain-identifier-{name}");
        var result = await grain.SayHelloWorld(name);
        return result;
    }
}