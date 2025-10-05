using SmartConfig.McpServer.Tools;
using SmartConfig.BE.Sdk;
using SmartConfig.BE.Sdk.Extensions;

namespace SmartConfig.McpServer.Extensions;

public static class IocExtensions
{
    public static IMcpServerBuilder WithMcpTools(this IMcpServerBuilder builder)
    {
        return builder
            .WithTools<EchoTool>()
            .WithTools<OrleansTool>();

        return builder;
    }
    
    public static WebApplicationBuilder AddSmartConfig(this WebApplicationBuilder builder)
    {
        // Add SmartConfig API.
        builder.Services.AddSingleton(new SmartConfigSettings
        {
            SmartConfigApiEndpoint = builder.Configuration["SmartConfig:ApiEndpoint"]!,
            ApplicationName = "SmartConfig.MpcServer",
            DryRun = false
        });
        builder.Services.AddSmartConfigClient();

        return builder;
    }
}
