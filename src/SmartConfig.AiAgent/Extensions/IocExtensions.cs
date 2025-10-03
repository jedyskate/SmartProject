using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;
using SmartConfig.AiAgent.Agents;
using SmartConfig.AiAgent.Agents.Workers;
using SmartConfig.AiAgent.Tools;

namespace SmartConfig.AiAgent.Extensions;

public static class IocExtensions
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
        
        // builder.Services.AddAgents(options => 
        // {
        //     // ...
        // });
        //
        // builder.Services.addage options.AddAgent<JokeAgent>()
        //     .WithTool<HelloWorldTool>();
        
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