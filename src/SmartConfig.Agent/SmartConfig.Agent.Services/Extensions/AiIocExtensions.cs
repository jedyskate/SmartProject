using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OllamaSharp;
using SmartConfig.Agent.Services.Agents;
using SmartConfig.Agent.Services.Agents.Workers;
using SmartConfig.Agent.Services.Tools;

namespace SmartConfig.Agent.Services.Extensions;

public static class AiIocExtensions
{
    public static WebApplicationBuilder AddAiAgentIoc(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;
        builder.Services.AddHttpClient("ollama", client =>
        {
            client.BaseAddress = new Uri(config["Agent:Ollama:Url"]!);
        });
        
        builder.Services.AddSingleton<IOllamaApiClient>(sp =>
        {
            var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = clientFactory.CreateClient("ollama");
            var model = config["Agent:Ollama:Model"]!;
            
            return new OllamaApiClient(httpClient, model);
        });
        
        builder.Services.AddScoped<IAgentService, AgentService>();
        builder.Services.AddAgents();
        builder.Services.AddTools();

        builder.Services.AddSingleton<IChatClient>(sp =>
        {
            var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = clientFactory.CreateClient("ollama");
            var model = config["Agent:Ollama:Model"]!;
            
            return new OllamaApiClient(httpClient, model);
        });

        builder.Services.AddSingleton<AIAgent>(sp =>
        {
            var chatClient = sp.GetRequiredService<IChatClient>();
            return new ChatClientAgent(chatClient);
        });
        
        return builder;
    }

    private static IServiceCollection AddAgents(this IServiceCollection services)
    {
        // Register the orchestrator and worker agents
        services.AddScoped<OrchestratorAgent>();
        services.AddScoped<IWorkerAgent, HelloWorldAgent>();
        services.AddScoped<IWorkerAgent, JokeAgent>();
        services.AddScoped<IWorkerAgent, GeneralPurposeAgent>();
        
        return services;
    }
    
    private static IServiceCollection AddTools(this IServiceCollection services)
    {
        services.AddSingleton<HelloWorldTool>();
        
        return services;
    }
}