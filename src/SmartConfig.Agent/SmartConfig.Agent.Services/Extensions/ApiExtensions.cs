using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SmartConfig.BE.Sdk;
using SmartConfig.BE.Sdk.Extensions;
using SmartConfig.BE.Sdk.Queue;

namespace SmartConfig.Agent.Services.Extensions;

public static class ApiExtensions
{
    public static WebApplicationBuilder AddSmartConfigApi(this WebApplicationBuilder builder)
    {
        // Add SmartConfig API.
        builder.Services.AddSingleton(new SmartConfigApiSettings
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
        builder.Services.AddSmartConfigApiClient(true);

        return builder;
    }
}