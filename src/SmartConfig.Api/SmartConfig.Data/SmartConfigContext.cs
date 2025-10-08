using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartConfig.Core.Models;
using SmartConfig.Data.Extensions;

namespace SmartConfig.Data;

public class SmartConfigContext : DbContext
{
    public SmartConfigContext(DbContextOptions<SmartConfigContext> options) : base(options)
    {
    }
        
    public DbSet<UserConfig> UserConfigs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfigConfiguration());
    }
}

public class UserConfigConfiguration : IEntityTypeConfiguration<UserConfig>
{
    public void Configure(EntityTypeBuilder<UserConfig> builder)
    {
        builder.HasIndex(u => u.Identifier).IsUnique();
        builder.Property(e => e.UserPreferences)!.HasJsonConversion<UserPreferences>();
        builder.Property(e => e.UserSettings)!.HasJsonConversion<List<UserSetting>>();
    }
}