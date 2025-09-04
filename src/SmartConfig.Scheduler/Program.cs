using Microsoft.EntityFrameworkCore;
using SmartConfig.Scheduler.Database;
using SmartConfig.Scheduler.Extensions;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddSqlServerDbContext<SchedulerContext>("TickerQ",
    configureDbContextOptions: options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("TickerQ");
        options.UseSqlServer(connectionString).EnableSensitiveDataLogging();
    });

builder.Services.AddTickerQ(options =>
{
    options.AddOperationalStore<SchedulerContext>(efOpts =>
    {
        efOpts.UseModelCustomizerForMigrations();
        efOpts.CancelMissedTickersOnApplicationRestart();
    });
    options.AddDashboard(basePath: "/tickerQ");
    options.AddDashboardBasicAuth();
});

builder.AddSmartConfig();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseTickerQ();

app.Run();