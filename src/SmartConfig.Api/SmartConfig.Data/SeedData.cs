namespace SmartConfig.Data;

public interface ISeedData
{
    void EnsureSeedData();
}

public class SeedData : ISeedData
{
    private readonly SmartConfigContext _context;

    public SeedData(SmartConfigContext context)
    {
        _context = context;
    }

    public void EnsureSeedData()
    {
    }
}