using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SmartConfig.App.Shared.Services;
using SmartConfig.App.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the SmartConfig.App.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

await builder.Build().RunAsync();