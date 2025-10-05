using SmartConfig.Blazor.Client.Extensions;
using SmartConfig.Blazor.Components;
using SmartConfig.Blazor.Extensions;
using SmartConfig.BE.Sdk;
using SmartConfig.BE.Sdk.Extensions;
using SmartConfig.BE.Sdk.Queue;

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

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();
        
        builder.AddServiceDefaults();
        builder.Services.AddCommonClientIoc(builder.Configuration);

        // Add SmartConfig API.
        builder.Services.AddSingleton(new SmartConfigSettings
        {
            SmartConfigApiEndpoint = builder.Configuration["SmartConfig:ApiEndpoint"]!,
            ApplicationName = "SmartConfig.Blazor",
            DryRun = false
        });
        builder.Services.AddSingleton(new SmartConfigQueueSettings
        {
            HostName = builder.Configuration["RabbitMq:TcpEndpoint:HostName"]!,
            Port = int.Parse(builder.Configuration["RabbitMq:TcpEndpoint:Port"]!),
            UserName = builder.Configuration["RabbitMq:Credentials:UserName"]!,
            Password = builder.Configuration["RabbitMq:Credentials:Password"]!,
            VirtualHost = builder.Configuration["RabbitMq:Credentials:VirtualHost"]!,
            Exchange = builder.Configuration["RabbitMq:SmartConfigMq:Exchange"]!,
            Queue = builder.Configuration["RabbitMq:SmartConfigMq:Queue"]!,
            RoutingKey = builder.Configuration["RabbitMq:SmartConfigMq:RoutingKey"]!,
            Environment = builder.Environment.EnvironmentName
        });
        builder.Services.AddSmartConfigClient(true);

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
        app.MapDefaultEndpoints();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(SmartConfig.Blazor.Client._Imports).Assembly);

        app.MapReverseProxy();

        return app;
    }
}