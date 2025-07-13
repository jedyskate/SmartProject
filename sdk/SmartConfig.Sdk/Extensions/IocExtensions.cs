using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SmartConfig.Sdk.Resolvers;

namespace SmartConfig.Sdk.Extensions;

public static class IocExtensions
{
    public static IServiceCollection AddSmartConfigClient(this IServiceCollection services)
    {
        services.AddHttpClient<ISmartConfigClient, SmartConfigClient>("SmartConfig", (provider, client) =>
        {
            var settings = provider.GetService<SmartConfigSettings>()!;
            client.BaseAddress = new Uri(settings.SmartConfigApiEndpoint);
        });
        services.AddTransient<ISmartConfigClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("SmartConfig");
    
            var smartConfigClient = new SmartConfigClient(httpClient)
            {
                JsonSerializerSettings =
                {
                    ContractResolver = new CustomCamelCaseResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                }
            };
            return smartConfigClient;
        });

        return services;
    }
}