using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AutoGen.RuntimeGateway.Grpc;
using Microsoft.Extensions.Hosting;

namespace SmartConfig.AgentHost;

public static class Host
{
    public static async Task<WebApplication> StartAsync(bool local = false, bool useGrpc = true)
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddServiceDefaults();
        builder.AddAgentService();

        var app = builder.Build();
        app.MapAgentService(local, useGrpc);
        app.MapDefaultEndpoints();
        await app.StartAsync().ConfigureAwait(false);
        return app;
    }
}
