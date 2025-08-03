using Microsoft.EntityFrameworkCore;
using SmartConfig.Scheduler.Database;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SchedulerContext>(options => options.UseSqlServer("Scheduler"));

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

var app = builder.Build();
// app.MapGet("/", () => "Hello World!");

app.UseTickerQ();

app.Run();