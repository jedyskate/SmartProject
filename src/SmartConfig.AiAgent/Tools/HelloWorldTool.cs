using System.ComponentModel;
using SmartConfig.Orleans.Silo.Grains.Tests;

namespace SmartConfig.AiAgent.Tools;

public class HelloWorldTool(IClusterClient clusterClient)
{
    [Description("Says hello to a user using Orleans grain")]
    public async Task<string> SayHelloAsync([Description("The name of the user to greet")] string name)
    {
        var grain = clusterClient.GetGrain<IHelloWorldUserGrain>($"hello-grain-identifier-{name}");
        var result = await grain.SayHelloWorld(name);
        return result;
    }
}