using Microsoft.Extensions.DependencyInjection;
using NBomber.Contracts;
using NBomber.CSharp;
using SmartConfig.BE.Sdk;
using SmartConfig.BE.Sdk.Extensions;

namespace SmartConfig.LoadTests.Tests;

[TestFixture]
public class HelloWorldTest : IDisposable
{
    private IServiceProvider _serviceProvider;

    [OneTimeSetUp]
    public void GlobalSetup()
    {
        var services = new ServiceCollection();
        services.AddSingleton(new SmartConfigSettings());
        services.AddSmartConfigClient();

        _serviceProvider = services.BuildServiceProvider();
    }

    [Test, Order(1)]
    public void HelloWorld_LoadTest()
    {
        var scenario = Scenario.Create("hello_world_scenario", async context =>
            {
                var smartConfigClient = _serviceProvider.GetRequiredService<ISmartConfigClient>();
                var response = await smartConfigClient.HelloWorldAsync(new HelloWorldCommand
                {
                    Name = "NBomber"
                });
                return response.Response.Contains("Hello world") ? Response.Ok() : Response.Fail();
            })
            .WithLoadSimulations(
                Simulation.Inject(rate: 10, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(5)),
                Simulation.Inject(rate: 20, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(10)),
                Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(15)),
                Simulation.Inject(rate: 100, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(20)),
                Simulation.Inject(rate: 200, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(25))
            )
            .WithThresholds(
                Threshold.Create(scenarioStats => scenarioStats.Fail.Request.Percent < 1.0),
                Threshold.Create(scenarioStats => scenarioStats.Ok.Latency.Percent95 < 500)
            );

        NBomberRunner.RegisterScenarios(scenario).Run();
    }

    public void Dispose()
    {
        (_serviceProvider as IDisposable)?.Dispose();
    }
}
