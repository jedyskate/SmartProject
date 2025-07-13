using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using SmartConfig.Data;

namespace SmartConfig.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Test.json", optional: true)
            .AddEnvironmentVariables();

        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");

        builder.ConfigureServices(ConfigureTestsServices)
            .UseConfiguration(configurationBuilder.Build())
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .UseEnvironment("Test");
    }

    private void ConfigureTestsServices(IServiceCollection services)
    {
        IdentityModelEventSource.ShowPII = true;

        services.AddDbContext<SmartConfigContext>(options =>
        {
            options.UseInMemoryDatabase("SmartConfig-db");
        });

        services.AddHttpClient();
    }
}