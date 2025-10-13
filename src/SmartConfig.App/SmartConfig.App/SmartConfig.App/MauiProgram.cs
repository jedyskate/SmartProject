using Microsoft.Extensions.Logging;
using SmartConfig.AI.Sdk;
using SmartConfig.AI.Sdk.Extensions;
using SmartConfig.App.Shared.Services;
using SmartConfig.App.Services;
using SmartConfig.BE.Sdk;
using SmartConfig.BE.Sdk.Extensions;

namespace SmartConfig.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        // Add device-specific services used by the SmartConfig.App.Shared project
        builder.Services.AddSingleton<IFormFactor, FormFactor>();

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        
        // Add SmartConfig Api Proxy.
        builder.Services.AddSingleton(new SmartConfigApiSettings
        {
            SmartConfigApiEndpoint = "https://localhost:7230",
            ApplicationName = "SmartConfig.App.Web.Client",
            DryRun = false
        });
        builder.Services.AddSmartConfigApiClient();

        // Add SmartConfig Agent Proxy.
        builder.Services.AddSingleton(new SmartConfigAgentSettings
        {
            SmartConfigAgentEndpoint = "https://localhost:7230",
            ApplicationName = "SmartConfig.App.Web.Client",
            DryRun = false
        });
        builder.Services.AddSmartConfigAgentClient();

        return builder.Build();
    }
}