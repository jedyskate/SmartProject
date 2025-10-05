using SmartConfig.BE.Sdk;
using SmartConfig.BE.Sdk.Extensions;
using SmartConfig.BE.Sdk.Queue;

namespace SmartConfig.Scheduler.Extensions;

public static class IocExtensions
{
    public static WebApplicationBuilder AddSmartConfig(this WebApplicationBuilder builder)
    {
        // Add SmartConfig API.
        builder.Services.AddSingleton(new SmartConfigSettings
        {
            SmartConfigApiEndpoint = builder.Configuration["SmartConfig:ApiEndpoint"]!,
            ApplicationName = "SmartConfig.Scheduler",
            DryRun = false
        });
        builder.Services.AddSingleton(new SmartConfigQueueSettings
        {
            HostName = builder.Configuration["RabbitMq:TcpEndpoint:HostName"]!,
            Port = int.Parse(builder.Configuration["RabbitMq:TcpEndpoint:Port"]!),
            UserName = builder.Configuration["RabbitMq:Credentials:UserName"]!,
            Password = builder.Configuration["RabbitMq:Credentials:Password"]!,
            VirtualHost = builder.Configuration["RabbitMq:Credentials:VirtualHost"]!,
            Exchange = builder.Configuration["RabbitMq:SmartConfigMq:Exchange"]!,
            Queue = builder.Configuration["RabbitMq:SmartConfigMq:Queue"]!,
            RoutingKey = builder.Configuration["RabbitMq:SmartConfigMq:RoutingKey"]!,
            Environment = builder.Environment.EnvironmentName
        });
        builder.Services.AddSmartConfigClient(true);

        return builder;
    }
}