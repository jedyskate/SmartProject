namespace SmartConfig.Common.Models;

public class ConfigPagination
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}

public abstract class ResultSetBase
{
    public int CurrentPage { get; set; }
    public int PageCount { get; set; }
    public int PageSize { get; set; }
    public int RowCount { get; set; }

    public int FirstRowOnPage
    {
        get { return (CurrentPage - 1) * PageSize + 1; }
    }

    public int LastRowOnPage
    {
        get { return Math.Min(CurrentPage * PageSize, RowCount); }
    }
}

public class ResultSet<T> : ResultSetBase //where T : class
{
    public IList<T> Results { get; set; }

    public ResultSet()
    {
        Results = new List<T>();
    }
}