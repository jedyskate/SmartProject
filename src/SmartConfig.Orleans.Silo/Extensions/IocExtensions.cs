using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SmartConfig.Orleans.Silo.Extensions;

public static class IocExtensions
{
    public static IServiceCollection AddOrleansSiloIoc(this IServiceCollection services, IConfiguration configuration)
    {

        return services;
    }
}