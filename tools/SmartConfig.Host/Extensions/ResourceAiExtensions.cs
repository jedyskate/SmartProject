using Projects;

namespace SmartConfig.Host.Extensions;

public static class ResourceAiExtensions
{
    public static void AddAiResources(this IDistributedApplicationBuilder builder)
    {
        if (bool.Parse(builder.Configuration["SmartConfig:Clients:Ai"] ?? "false"))
        {
            // Mcp Server
            var mcp = builder.AddProject<SmartConfig_McpServer>("mcp")
                .WithExternalHttpEndpoints();

            // Ollama
            var ollama = builder.AddOllama("ollama")
                .WaitFor(mcp)
                .WithParentRelationship(mcp)
                .WithDataVolume();
                // .AddModel("phi4-mini", "phi4-mini");
            
            // AnythingLLM
            var anythingLlm = builder.AddContainer("anythingllm", "mintplexlabs/anythingllm", "latest")
                .WithReference(mcp)
                .WaitFor(mcp)
                .WithParentRelationship(mcp)
                // .WithEnvironment("OLLAMA_API_BASE", "http://localhost:11434")
                .WithEnvironment("STORAGE_DIR", "/app/server/storage")
                .WithVolume("anythingllm-storage", "/app/server/storage")
                .WithBindMount("Configurations/anythingllm_mcp_servers.json", "/app/server/storage/plugins/anythingllm_mcp_servers.json")
                .WithBindMount("Configurations/empty.json", "/app/server/storage/plugins/agent-flows/empty.json") // Directory is needed
                .WithBindMount("Configurations/empty.json", "/app/server/storage/plugins/agent-skills/empty.json") // Directory is needed
                .WithHttpEndpoint(targetPort: 3001, name: "http", isProxied: true);
        }
    }
}