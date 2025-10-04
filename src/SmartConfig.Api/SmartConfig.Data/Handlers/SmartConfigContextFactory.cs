using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SmartConfig.Data.Handlers;

public class SmartConfigContextFactory : IDesignTimeDbContextFactory<SmartConfigContext>
{
    public SmartConfigContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SmartConfigContext>();
        optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=SmartConfigContext;");

        return new SmartConfigContext(optionsBuilder.Options);
    }
}