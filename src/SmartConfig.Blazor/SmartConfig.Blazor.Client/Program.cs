using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SmartConfig.Blazor.Client.Extensions;
using SmartConfig.BE.Sdk;
using SmartConfig.BE.Sdk.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddCommonClientIoc(builder.Configuration);

// Add SmartConfig Proxy.
builder.Services.AddSingleton(new SmartConfigSettings
{
    SmartConfigApiEndpoint = builder.HostEnvironment.BaseAddress,
    ApplicationName = "SmartConfig.Blazor.Client",
    DryRun = false
});
builder.Services.AddSmartConfigClient();

await builder.Build().RunAsync();
