using RabbitMQ.Client;
using SmartConfig.Api.Handlers;

namespace SmartConfig.Api.Extensions;

public static class RabbitMqExtensions
{
    public static IServiceCollection AddRabbitMqConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        if (bool.Parse(configuration["RabbitMq:Enabled"]))
        {
            services.AddHostedService<RabbitMqRequestHandler>();
            services.AddSingleton<IConnection>(provider => {
                var factory = new ConnectionFactory
                {
                    HostName = configuration["RabbitMq:TcpEndpoint:HostName"],
                    Port = int.Parse(configuration["RabbitMq:TcpEndpoint:Port"]),
                    UserName = configuration["RabbitMq:Credentials:UserName"],
                    Password = configuration["RabbitMq:Credentials:Password"],
                    VirtualHost = configuration["RabbitMq:Credentials:VirtualHost"],
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

    public static IApplicationBuilder UseRabbitMqConsumer(this IApplicationBuilder app)
    {

        return app;
    }
}