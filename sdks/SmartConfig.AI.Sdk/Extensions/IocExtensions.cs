using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SmartConfig.AI.Sdk.Resolvers;
using SmartConfig.Sdk;

namespace SmartConfig.AI.Sdk.Extensions;

public static class IocExtensions
{
    public static IServiceCollection AddSmartConfigAgentClient(this IServiceCollection services)
    {
        //HTTP CLIENT
        services.AddHttpClient<ISmartConfigAgentClient, SmartConfigAgentClient>("SmartConfig", (provider, client) =>
        {
            var settings = provider.GetService<SmartConfigAgentSettings>()!;
            client.BaseAddress = new Uri(settings.SmartConfigApiEndpoint);
        });
        services.AddTransient<ISmartConfigAgentClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("SmartConfig");
    
            var smartConfigClient = new SmartConfigAgentClient(httpClient)
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