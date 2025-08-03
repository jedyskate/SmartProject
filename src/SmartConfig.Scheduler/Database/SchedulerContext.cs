using Microsoft.EntityFrameworkCore;
using TickerQ.EntityFrameworkCore.Configurations;

namespace SmartConfig.Scheduler.Database;

public class SchedulerContext : DbContext
{
    public SchedulerContext(DbContextOptions<SchedulerContext> options) : base(options)
    {
    }
        
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new TimeTickerConfigurations());
        builder.ApplyConfiguration(new CronTickerConfigurations());
        builder.ApplyConfiguration(new CronTickerOccurrenceConfigurations());

        // builder.ApplyConfigurationsFromAssembly(typeof(TimeTickerConfigurations).Assembly);
    }
}