using Microsoft.EntityFrameworkCore;
using SmartConfig.Data;

namespace SmartConfig.UnitTests.Infrastructure;

public class TestBase : IDisposable
{
    protected readonly SmartConfigContext _context;

    protected TestBase()
    {
        _context = SmartConfigContextFactory.Create();
    }

    public void Dispose()
    {
        SmartConfigContextFactory.Destroy(_context);
    }
}

public static class SmartConfigContextFactory
{
    public static SmartConfigContext Create()
    {
        var options = new DbContextOptionsBuilder<SmartConfigContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new SmartConfigContext(options);
        context.Database.EnsureCreated();

        return context;
    }

    public static void Destroy(SmartConfigContext context)
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }
}