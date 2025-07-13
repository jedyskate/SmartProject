using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SmartConfig.Sdk;
using SmartConfig.Sdk.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add SmartConfig Proxy.
builder.Services.AddSingleton(new SmartConfigSettings
{
    SmartConfigApiEndpoint = builder.HostEnvironment.BaseAddress,
    ApplicationName = "SmartConfig.Blazor.Client",
    DryRun = false
});
builder.Services.AddSmartConfigClient();

await builder.Build().RunAsync();
