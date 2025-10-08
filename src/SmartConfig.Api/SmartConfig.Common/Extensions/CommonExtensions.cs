using Microsoft.Extensions.DependencyInjection;
using SmartConfig.Common.Models;

namespace SmartConfig.Common.Extensions;

public static class CommonExtensions
{
    public static IServiceCollection AddCommon(this IServiceCollection services)
    {
        services.AddSingleton<ConfigPagination>();
        services.AddSingleton(typeof(ResultSet<>));

        return services;
    }
}