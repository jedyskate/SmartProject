using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SmartConfig.AI.Sdk;
using SmartConfig.AI.Sdk.Extensions;
using SmartConfig.Blazor.Client.Extensions;
using SmartConfig.BE.Sdk;
using SmartConfig.BE.Sdk.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddCommonClientIoc(builder.Configuration);

// Add SmartConfig Api Proxy.
builder.Services.AddSingleton(new SmartConfigApiSettings
{
    SmartConfigApiEndpoint = builder.HostEnvironment.BaseAddress,
    ApplicationName = "SmartConfig.Blazor.Client",
    DryRun = false
});
builder.Services.AddSmartConfigApiClient();

// Add SmartConfig Agent Proxy.
builder.Services.AddSingleton(new SmartConfigAgentSettings
{
    SmartConfigAgentEndpoint = builder.HostEnvironment.BaseAddress,
    ApplicationName = "SmartConfig.Blazor.Client",
    DryRun = false
});
builder.Services.AddSmartConfigAgentClient();


await builder.Build().RunAsync();
