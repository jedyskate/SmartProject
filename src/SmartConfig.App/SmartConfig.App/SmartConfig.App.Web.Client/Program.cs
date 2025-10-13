using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SmartConfig.AI.Sdk;
using SmartConfig.AI.Sdk.Extensions;
using SmartConfig.App.Shared.Services;
using SmartConfig.App.Web.Client.Extensions;
using SmartConfig.App.Web.Client.Services;
using SmartConfig.BE.Sdk;
using SmartConfig.BE.Sdk.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the SmartConfig.App.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();


// SPECIFIC
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