using Projects;

namespace SmartConfig.Host.Extensions;

public static class ResourceAiExtensions
{
    public static void AddAiResources(this IDistributedApplicationBuilder builder)
    {
        if (!bool.Parse(builder.Configuration["SmartConfig:Clients:Ai"] ?? "true")) return;

        // Mcp Server
        var mcp = builder.AddProject<SmartConfig_McpServer>("mcp")
            .WithExternalHttpEndpoints();

        // Ollama
        var ollama = builder.AddOllama("ollama")
            .WaitFor(mcp)
            .WithParentRelationship(mcp)
            .WithHttpEndpoint(port: 11434, targetPort: 11434, name: "n8n-http", isProxied: false)
            .WithExternalHttpEndpoints()
            .WithDataVolume()
            .AddModel("phi4-mini", "phi4-mini");

        // n8n
        var n8n = builder.AddContainer("n8n", "n8nio/n8n", "latest")
            .WaitFor(mcp)
            .WaitFor(ollama)
            .WithParentRelationship(mcp)
            .WithHttpEndpoint(port: 5678, targetPort: 5678, name: "n8n-http", isProxied: false)
            .WithExternalHttpEndpoints()
            .WithEnvironment("TZ", "Australia/Sydney")
            .WithEnvironment("GENERIC_TIMEZONE", "Australia/Sydney")
            .WithEnvironment("N8N_HOST", "localhost:5678")
            .WithEnvironment("WEBHOOK_URL", "http://localhost:5678")
            // TODO::Add credential directly in n8n. Ollama doesn't need ApiKey.
            // .WithEnvironment("OLLAMA_BASE_URL", "http://host.docker.internal:11434")
            .WithVolume("n8n-storage", "/home/node/.n8n");

        // AnythingLLM
        var anythingLlm = builder.AddContainer("anythingllm", "mintplexlabs/anythingllm", "latest")
            .WaitFor(mcp)
            .WaitFor(ollama)
            .WithParentRelationship(mcp)
            .WithHttpEndpoint(port: 3001, targetPort: 3001, name: "anythingLlm-http", isProxied: false)
            .WithExternalHttpEndpoints()
            .WithEnvironment("OLLAMA_API_BASE", "http://localhost:11434")
            .WithEnvironment("STORAGE_DIR", "/app/server/storage")
            .WithVolume("anythingllm-storage", "/app/server/storage")
            .WithBindMount("Configurations/anythingllm_mcp_servers.json", "/app/server/storage/plugins/anythingllm_mcp_servers.json")
            .WithBindMount("Configurations/empty.json", "/app/server/storage/plugins/agent-flows/empty.json") // Directory is needed
            .WithBindMount("Configurations/empty.json", "/app/server/storage/plugins/agent-skills/empty.json"); // Directory is needed
    }
}