using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SmartConfig.Blazor.Client.Extensions;
using SmartConfig.BE.Sdk;
using SmartConfig.BE.Sdk.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddCommonClientIoc(builder.Configuration);

// Add SmartConfig Proxy.
builder.Services.AddSingleton(new SmartConfigApiSettings
{
    SmartConfigApiEndpoint = builder.HostEnvironment.BaseAddress,
    ApplicationName = "SmartConfig.Blazor.Client",
    DryRun = false
});
builder.Services.AddSmartConfigApiClient();

await builder.Build().RunAsync();
