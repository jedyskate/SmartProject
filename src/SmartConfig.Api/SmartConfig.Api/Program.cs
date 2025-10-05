using SmartConfig.Api.Extensions;
using SmartConfig.Application.Extensions;
using SmartConfig.Data;
using SmartConfig.ServiceDefaults;

namespace SmartConfig.Api;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            CreateHostBuilder(args).Run();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args)
            .AddAppSettingConfigurations()
            .AddConfigureLogging()
            .ConfigureServices()
            .AddEntityFramework()
            .AddOrleansConfigurations();

        var app = builder.Build();
        
        app.MapDefaultEndpoints();
        app.Initialize(builder.Configuration, app.Services.GetRequiredService<ISeedData>());

        return app;
    }
}