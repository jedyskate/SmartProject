using Newtonsoft.Json;
using Orleans.Serialization;
using OrleansDashboard;

namespace SmartConfig.Api.Extensions;

public static class OrleansExtensions
{
    public static WebApplicationBuilder AddOrleansConfigurations(this WebApplicationBuilder builder)
    {
        builder.Services.AddOrleans(siloBuilder =>
        {
            var configuration = builder.Configuration;
            var environment = builder.Environment.EnvironmentName;

            if (environment == "Test" || environment == "Local")
            {
                siloBuilder
                    .UseLocalhostClustering()
                    .AddMemoryGrainStorageAsDefault();
            }
            else
            {
#if DEBUG
                siloBuilder
                    .UseLocalhostClustering()
                    .AddMemoryGrainStorage("Redis");
#endif
#if RELEASE
                siloBuilder
                    .UseRedisClustering(opt =>
                    {
                        var cfg = ConfigurationOptions.Parse($"{configuration["Redis:Connection"]},password={configuration["Redis:Password"]}");
                        cfg.DefaultDatabase = int.Parse(configuration["Redis:DatabaseIndex"]);
                        opt.ConfigurationOptions = cfg;
                    })
                    .AddRedisGrainStorage("Redis", optBuilder => optBuilder.Configure(opt =>
                    {
                        var cfg = ConfigurationOptions.Parse($"{configuration["Redis:Connection"]},password={configuration["Redis:Password"]}");
                        cfg.DefaultDatabase = int.Parse(configuration["Redis:DatabaseIndex"]);
                        opt.ConfigurationOptions = cfg;
                        opt.DeleteStateOnClear = true;
                    }))
                    .AddMemoryGrainStorage("Memory")
                    .Configure<GrainCollectionOptions>(opt =>
                    {
                        opt.CollectionAge = TimeSpan.FromMinutes(15);
                    })
                    .UseInMemoryReminderService()
                    .Configure<ClusterOptions>(opts =>
                    {
                        opts.ClusterId = $"{configuration["Application:Name"]}-{environment}";
                        opts.ServiceId = $"{configuration["Application:Name"]}-OrleansSiloService";
                    })
                    .Configure<EndpointOptions>(opts =>
                    {
                        opts.AdvertisedIPAddress = IPAddress.Loopback;
                        opts.SiloPort = int.Parse(configuration["OrleansConfig:SiloPort"]);
                        opts.GatewayPort = int.Parse(configuration["OrleansConfig:GatewayPort"]);
                    })
                    .ConfigureLogging(logging =>
                    {
                        logging.AddConsole();
                    });
#endif
            }

            if (bool.TryParse(configuration["OrleansConfig:Dashboard:Enabled"], out var dashboardEnabled) && dashboardEnabled)
            {
                siloBuilder.UseDashboard(options =>
                {
                    options.HostSelf = true;
                    options.Host = "*";
                    options.CounterUpdateIntervalMs = int.Parse(configuration["OrleansConfig:Dashboard:CounterUpdateIntervalMs"]);
                });
            }

            siloBuilder.Services.AddSerializer(serializerBuilder =>
            {
                serializerBuilder.AddNewtonsoftJsonSerializer(
                    isSupported: type => type!.Namespace!.StartsWith("SmartConfig.Core"), 
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                serializerBuilder.AddNewtonsoftJsonSerializer(
                    isSupported: type => type!.Namespace!.StartsWith("SmartConfig.Common"));

                serializerBuilder.AddNewtonsoftJsonSerializer(
                    isSupported: type => type!.Namespace!.StartsWith("SmartConfig.Silo"));
            });
        });

        return builder;
    }
        
    public static IApplicationBuilder UseOrleansInterface(this IApplicationBuilder app, IConfiguration configuration)
    {
        if (bool.Parse(configuration["OrleansConfig:Dashboard:Enabled"]))
        {
            app.UseOrleansDashboard(new DashboardOptions
            {
                Username = configuration["OrleansConfig:Dashboard:Username"],
                Password = configuration["OrleansConfig:Dashboard:Password"],
                BasePath = "orleans"
            });
        }
        
        return app;
    }
        
    public static IServiceCollection AddOrleansInterface(this IServiceCollection services, IConfiguration configuration)
    {
        if (bool.Parse(configuration["OrleansConfig:Dashboard:Enabled"]))
        {
            services.AddDashboard(options => {
                options.Host = "*";
                options.HostSelf = true;
            });
        }
        
        return services;
    }
}