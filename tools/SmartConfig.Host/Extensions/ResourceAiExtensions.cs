using Microsoft.Extensions.Configuration;
using Projects;

namespace SmartConfig.Host.Extensions;

public static class ResourceAiExtensions
{
    public static void AddAiResources(this IDistributedApplicationBuilder builder, IResourceBuilder<ProjectResource> api)
    {
        var clients = builder.Configuration.GetSection("SmartConfig:Ai:Clients").Get<string[]>();
        if (!clients?.Any() ?? true) return;
        
        // Mcp Server
        var mcp = builder.AddProject<SmartConfig_McpServer>("mcp")
            .WaitFor(api)
            .WithReference(api)
            .WithExternalHttpEndpoints();

        // Ollama
        var ollama = builder.AddOllama("ollama")
            .WithHttpEndpoint(port: 11434, targetPort: 11434, name: "ollama-http", isProxied: false)
            .WithExternalHttpEndpoints()
            .WithParentRelationship(mcp)
            .WithDataVolume()
            // .AddModel("phi4-mini", "phi4-mini:latest");
            .AddModel("llama32", "llama3.2:latest");

        var agentApiKey = builder.Configuration["Agent:OpenRouter:ApiKey"];
        var isAgentFrameworkEnabled = !string.IsNullOrWhiteSpace(agentApiKey);

        // Agent
        if ((clients?.Contains("agent") ?? false) && isAgentFrameworkEnabled)
            builder.AddProject<SmartConfig_Agent>("agent")
                .WithReference(ollama)
                .WaitFor(mcp)
                .WaitFor(ollama)
                .WithParentRelationship(mcp)
                .WithEnvironment("Agent__OpenRouter__ApiKey", agentApiKey);
        
        // n8n
        if (clients?.Contains("n8n") ?? false)
            builder.AddContainer("n8n", "n8nio/n8n", "latest")
                .WaitFor(mcp)
                .WaitFor(ollama)
                .WithParentRelationship(mcp)
                .WithHttpEndpoint(port: 5678, targetPort: 5678, name: "n8n-http", isProxied: false)
                .WithExternalHttpEndpoints()
                .WithEnvironment("TZ", "Australia/Sydney")
                .WithEnvironment("GENERIC_TIMEZONE", "Australia/Sydney")
                .WithEnvironment("N8N_HOST", "localhost:5678")
                .WithEnvironment("WEBHOOK_URL", "http://localhost:5678")
                .WithVolume("n8n-storage", "/home/node/.n8n")
                .WithDefaultAiAgent(builder.Configuration); //TODO::Disable for prod environments

        // AnythingLLM
        if (clients?.Contains("anythingLlm") ?? false)
            builder.AddContainer("anythingllm", "mintplexlabs/anythingllm", "latest")
                .WaitFor(mcp)
                .WaitFor(ollama)
                .WithParentRelationship(mcp)
                .WithHttpEndpoint(port: 3001, targetPort: 3001, name: "anythingLlm-http", isProxied: false)
                .WithExternalHttpEndpoints()
                .WithEnvironment("OLLAMA_API_BASE", "http://localhost:11434")
                .WithEnvironment("STORAGE_DIR", "/app/server/storage")
                .WithVolume("anythingllm-storage", "/app/server/storage")
                .WithBindMount("Volumes/AnythingLLM/anythingllm_mcp_servers.json", "/app/server/storage/plugins/anythingllm_mcp_servers.json")
                .WithBindMount("Volumes/AnythingLLM/empty.json", "/app/server/storage/plugins/agent-flows/empty.json") // Directory is needed
                .WithBindMount("Volumes/AnythingLLM/empty.json", "/app/server/storage/plugins/agent-skills/empty.json"); // Directory is needed
    }

    private static IResourceBuilder<T> WithDefaultAiAgent<T>(this IResourceBuilder<T> builder,
        IConfiguration configuration) where T : ContainerResource
    {
        if (bool.Parse(configuration["SmartConfig:Ai:n8nDefaultAgent"] ?? "false"))
        {
            builder.WithBindMount("Volumes/n8n/config", "/home/node/.n8n/config")
                .WithBindMount("Volumes/n8n/database.sqlite", "/home/node/.n8n/database.sqlite");
        }
        
        return builder;
    }
}