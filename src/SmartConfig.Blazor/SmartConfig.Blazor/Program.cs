using SmartConfig.Blazor.Components;
using SmartConfig.Blazor.Extensions;
using SmartConfig.Sdk;
using SmartConfig.Sdk.Extensions;

namespace SmartConfig.Blazor;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            CreateHost(args).Run();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static IHost CreateHost(string[] args)
    {
        var builder = WebApplication.CreateBuilder();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        // Add Open Telemetry.
        builder.AddServerOpenTelemetry();

        // Add SmartConfig API.
        builder.Services.AddSingleton(new SmartConfigSettings
        {
            SmartConfigApiEndpoint = builder.Configuration["SmartConfig:ApiEndpoint"]!,
            ApplicationName = "SmartConfig.Blazor",
            DryRun = false
        });
        builder.Services.AddSmartConfigClient();

        // Add YARP to proxy FE requests to SmartConfig API
        builder.AddYarp();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseAntiforgery();
        app.MapStaticAssets();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(SmartConfig.Blazor.Client._Imports).Assembly);

        app.MapReverseProxy();

        return app;
    }
}