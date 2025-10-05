using EntityFramework.Exceptions.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartConfig.Data;

namespace SmartConfig.Application.Extensions;

public static class EntityFrameworkExtensions
{
    public static WebApplicationBuilder AddEntityFramework(this WebApplicationBuilder builder)
    {
        var env = builder.Environment.EnvironmentName;
        var isAspire = Environment.GetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES")
            ?.Contains("Microsoft.AspNetCore") == true;
        
        if (isAspire)
        {
            builder.AddSqlServerDbContext<SmartConfigContext>("SmartConfig",
                configureDbContextOptions: options =>
                {
                    var connectionString = builder.Configuration.GetConnectionString("SmartConfig");

                    options.UseSqlServer(connectionString).EnableSensitiveDataLogging();
                    options.UseExceptionProcessor();
                });
        }
        else if (env != "Test")
        {
            builder.Services.AddDbContext<SmartConfigContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("SmartConfig-db"))
                    .EnableSensitiveDataLogging();
            });
        }

        builder.Services.AddSingleton<ISeedData, SeedData>();

        return builder;
    }

    public static IApplicationBuilder UseEntityFramework(this IApplicationBuilder app, ISeedData seedData)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope())
        {
            var ctx = serviceScope.ServiceProvider.GetRequiredService<SmartConfigContext>();
            if (ctx.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            {
                if (env == "Local")
                {
                    ctx.Database.Migrate();
                }
                else
                {
#if RELEASE
                        ctx.Database.Migrate();
#endif
                }
            }

            seedData.EnsureSeedData();
        }

        return app;
    }
}