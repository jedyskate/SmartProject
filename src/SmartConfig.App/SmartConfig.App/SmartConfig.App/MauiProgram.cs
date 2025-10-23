using System.Reflection;
using Microsoft.Extensions.Configuration;
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
        
       
        
        var assembly = typeof(MauiProgram).GetTypeInfo().Assembly;
        using var streamMain = assembly.GetManifestResourceStream("appsettings.json");
        if (streamMain != null)
        {
            builder.Configuration.AddJsonStream(streamMain);
        }

        var resourceName = $"appsettings.{builder.Configuration["Environment"]}.json";
        using var streamEnv = assembly.GetManifestResourceStream(resourceName);
        if (streamEnv != null)
        {
            builder.Configuration.AddJsonStream(streamEnv);
        }
        
        
        
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
            ApplicationName = "SmartConfig.App",
            DryRun = false
        });
        builder.Services.AddSmartConfigApiClient();

        // Add SmartConfig Agent Proxy.
        builder.Services.AddSingleton(new SmartConfigAgentSettings
        {
            SmartConfigAgentEndpoint = "https://localhost:7230",
            ApplicationName = "SmartConfig.App",
            DryRun = false
        });
        builder.Services.AddSmartConfigAgentClient();

        return builder.Build();
    }
}