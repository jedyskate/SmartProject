using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartConfig.Data;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<ISeedData, SeedData>();
builder.Services.AddSingleton(TimeProvider.System);
builder.AddSqlServerDbContext<SmartConfigContext>("SmartConfig");

var host = builder.Build();


// Start the host to initialize DI, logging, etc.
await host.StartAsync();

using (var serviceScope = host.Services.GetService<IServiceScopeFactory>()!.CreateScope())
{
    var environment = serviceScope.ServiceProvider.GetRequiredService<IHostEnvironment>();
    var ctx = serviceScope.ServiceProvider.GetRequiredService<SmartConfigContext>();
    if (ctx.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
    {
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
}

//Ensure seed data
var seedData = host.Services.GetRequiredService<ISeedData>();
seedData.EnsureSeedData();

await host.StopAsync();