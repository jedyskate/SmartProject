using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SmartConfig.BE.Sdk.Queue;
using SmartConfig.BE.Sdk.Resolvers;

namespace SmartConfig.BE.Sdk.Extensions;

public static class IocExtensions
{
    public static IServiceCollection AddSmartConfigApiClient(this IServiceCollection services, bool? queueEnabled = false)
    {
        //HTTP CLIENT
        services.AddHttpClient<ISmartConfigApiClient, SmartConfigApiClient>("SmartConfig", (provider, client) =>
        {
            var settings = provider.GetService<SmartConfigApiSettings>()!;
            client.BaseAddress = new Uri(settings.SmartConfigApiEndpoint);
        });
        services.AddTransient<ISmartConfigApiClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("SmartConfig");
    
            var smartConfigClient = new SmartConfigApiClient(httpClient)
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

        if (queueEnabled ?? false)
        {
            //QUEUE CLIENT
            services.AddSingleton<ISmartConfigQueue, SmartConfigQueue>();
            services.AddSingleton<ISmartConfigQueueManager, SmartConfigQueueManager>();

            //RabbitMQ Connection
            services.AddSingleton<IConnection>(provider =>
            {
                var settings = provider.GetService<SmartConfigQueueSettings>()!;
                var factory = new ConnectionFactory
                {
                    HostName = settings.HostName,
                    Port = settings.Port,
                    UserName = settings.UserName,
                    Password = settings.Password,
                    VirtualHost = settings.VirtualHost,
                    AutomaticRecoveryEnabled = true,
                    TopologyRecoveryEnabled = true,
                    RequestedConnectionTimeout = TimeSpan.FromMilliseconds(60000),
                    RequestedHeartbeat = TimeSpan.FromSeconds(60),
                    DispatchConsumersAsync = true
                };
                return factory.CreateConnection();
            });   
        }

        return services;
    }
}