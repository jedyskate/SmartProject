using SmartConfig.McpServer.Tools;

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
}
