using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartConfig.Data;
using SmartConfig.Migration.Extensions;
using SmartConfig.Scheduler.Database;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<ISeedData, SeedData>();
builder.Services.AddSingleton(TimeProvider.System);

builder.AddSqlServerDbContext<SmartConfigContext>("SmartConfig");
builder.AddSqlServerDbContext<SchedulerContext>("TickerQ");

var host = builder.Build();

// Start the host to initialize DI, logging, etc.
await host.StartAsync();

// RUN MIGRATIONS
host.RunBackendMigration();
host.RunSchedulerMigration();

await host.StopAsync();