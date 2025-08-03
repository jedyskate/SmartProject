using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartConfig.Data;
using SmartConfig.Scheduler.Database;

namespace SmartConfig.Migration.Extensions;

public static class IocExtensions
{
    public static IHost RunBackendMigration(this IHost host)
    {
        using (var serviceScope = host.Services.GetService<IServiceScopeFactory>()!.CreateScope())
        {
            var environment = serviceScope.ServiceProvider.GetRequiredService<IHostEnvironment>();
            var ctx = serviceScope.ServiceProvider.GetRequiredService<SmartConfigContext>();
            if (environment.EnvironmentName == "Local")
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

        //Ensure seed data
        var seedData = host.Services.GetRequiredService<ISeedData>();
        seedData.EnsureSeedData();
        
        return host;
    }
    
    public static IHost RunSchedulerMigration(this IHost host)
    {
        using (var serviceScope = host.Services.GetService<IServiceScopeFactory>()!.CreateScope())
        {
            var environment = serviceScope.ServiceProvider.GetRequiredService<IHostEnvironment>();
            var ctx = serviceScope.ServiceProvider.GetRequiredService<SchedulerContext>();
            if (environment.EnvironmentName == "Local")
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
        
        return host;
    }
}