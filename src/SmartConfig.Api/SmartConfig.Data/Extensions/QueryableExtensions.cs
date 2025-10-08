using System.Linq.Dynamic.Core;
using System.Net;
using SmartConfig.Common.Exceptions;
using SmartConfig.Common.Models;

namespace SmartConfig.Data.Extensions;

public static class QueryableExtensions
{
    public static ResultSet<T> GetResultSet<T>(this IQueryable<T> query, ConfigPagination? configPagination) where T : class
    {
        if (configPagination == null)
        {
            var queryResult = query.ToList();

            return new ResultSet<T>
            {
                CurrentPage = 1,
                PageSize = queryResult.Count,
                RowCount = queryResult.Count,
                PageCount = 1,
                Results = queryResult
            };
        }
        if (!configPagination.Page.HasValue || !configPagination.PageSize.HasValue)
            throw new SmartConfigException(HttpStatusCode.BadRequest,
                "Both Page and PageSize are required for configPagination.");

        var result = new ResultSet<T>
        {
            CurrentPage = configPagination.Page.Value,
            PageSize = configPagination.PageSize.Value,
            RowCount = query.Count()
        };

        var pageCount = (double)result.RowCount / configPagination.PageSize.Value;
        result.PageCount = (int)Math.Ceiling(pageCount);

        var skip = (configPagination.Page.Value - 1) * configPagination.PageSize.Value;
        result.Results = query.Skip(skip).Take(configPagination.PageSize.Value).ToList();

        return result;
    }

    public static IQueryable<T> SetOrder<T>(this IQueryable<T> query, ConfigOrder? configOrder) where T : class
    {
        if (configOrder == null) return query;
        if (!configOrder.OrderType.HasValue || string.IsNullOrEmpty(configOrder.OrderBy))
            throw new SmartConfigException(HttpStatusCode.BadRequest,
                "Both OrderType and OrderBy are required to sort.");

        //TODO::CHECK IF PROPERTY EXIST
        //if (!typeof(T).HasProperty(configOrder.OrderBy))
        //    throw new SmartConfigException(HttpStatusCode.BadRequest, "It's not possible to sort because the attribute doesn't exist");

        if (configOrder.OrderType == ConfigSearchOrderType.Ascending)
            query = query.OrderBy(configOrder.OrderBy);

        if (configOrder.OrderType == ConfigSearchOrderType.Descending)
            query = query.OrderBy($"{configOrder.OrderBy} DESC");

        return query;
    }
}