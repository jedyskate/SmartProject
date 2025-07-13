using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartConfig.Orleans.Silo.Extensions;

namespace SmartConfig.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOrleansSiloIoc(configuration);
            
        services.AddHttpClient();

        return services;
    }
}