using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SmartConfig.AI.Sdk.Resolvers;

namespace SmartConfig.AI.Sdk.Extensions;

public static class IocExtensions
{
    public static IServiceCollection AddSmartConfigAgentClient(this IServiceCollection services)
    {
        //HTTP CLIENT
        services.AddHttpClient("SmartConfigAgent", (provider, client) =>
            {
                var settings = provider.GetRequiredService<SmartConfigAgentSettings>();
                client.BaseAddress = new Uri(settings.SmartConfigAgentEndpoint);
            })
            .AddTypedClient<ISmartConfigAgentClient>((httpClient, provider) =>
                new SmartConfigAgentClient(httpClient)
                {
                    ReadResponseAsString = true, // Required for Blazor WebAssembly compatibility
                    JsonSerializerSettings =
                    {
                        ContractResolver = new CustomCamelCaseResolver(),
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    }
                });

        return services;
    }
}