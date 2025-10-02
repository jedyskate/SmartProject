using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using OllamaSharp;
using SmartConfig.AiAgent.Agents;
using SmartConfig.AiAgent.Agents.Workers;
using SmartConfig.AiAgent.Plugins;

namespace SmartConfig.AiAgent.Extensions;

public static class IocExtensions
{
    public static WebApplicationBuilder AddAiAgentIoc(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;
        builder.Services.AddHttpClient("ollama", client =>
        {
            client.BaseAddress = new Uri(config["SemanticKernel:Ollama:Url"]!);
        });
        
        builder.Services.AddSingleton<IOllamaApiClient>(sp =>
        {
            var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = clientFactory.CreateClient("ollama");
            var model = config["SemanticKernel:Ollama:Model"]!;
            
            return new OllamaApiClient(httpClient, model);
        });
        
        builder.Services.AddScoped<IKernelService, KernelService>();
        builder.Services.AddAgents();
        builder.Services.AddPlugins();

        builder.Services.AddSingleton<Kernel>(sp =>
        {
            var ollamaClient = sp.GetRequiredService<IOllamaApiClient>();
            
            var kernel = Kernel.CreateBuilder()
                .AddOllamaChatCompletion(
                    ollamaClient: (OllamaApiClient)ollamaClient,
                    serviceId: "ollama")
                .Build();
            
            kernel.ImportPluginFromObject(sp.GetRequiredService<HelloWorldPlugin>(), "HelloWorld");

            return kernel;
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
    
    private static IServiceCollection AddPlugins(this IServiceCollection services)
    {
        services.AddSingleton<HelloWorldPlugin>();
        
        return services;
    }
}