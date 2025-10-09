using System.ClientModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OllamaSharp;
using OpenAI;
using SmartConfig.Agent.Services.Agents;
using SmartConfig.Agent.Services.Agents.Workers;
using SmartConfig.Agent.Services.Tools;

namespace SmartConfig.Agent.Services.Extensions;

public static class AiIocExtensions
{
    public static WebApplicationBuilder AddAiAgentIoc(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;
        
        // Ollama Client
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
        
        // OpenRouter Client
        builder.Services.AddSingleton<OpenAIClient>(sp =>
        {
            return new OpenAIClient(new ApiKeyCredential(config["Agent:OpenRouter:ApiKey"]!), new OpenAIClientOptions
            {
                Endpoint = new Uri(config["Agent:OpenRouter:Url"]!),
                ProjectId = config["Agent:ServiceId"]!
            });
        });
        
        builder.Services.AddScoped<IAgentService, AgentService>();
        builder.Services.AddAgents();
        builder.Services.AddTools();
        
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