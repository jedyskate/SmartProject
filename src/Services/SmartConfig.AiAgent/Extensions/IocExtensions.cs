using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using OllamaSharp;

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
        builder.Services.AddSingleton<Kernel>(sp =>
        {
            var ollamaClient = sp.GetRequiredService<IOllamaApiClient>();
            
            var kernel = Kernel.CreateBuilder()
                .AddOllamaChatCompletion(
                    ollamaClient: (OllamaApiClient)ollamaClient,
                    serviceId: "ollama")
                .Build();

            return kernel;
        });
        
        return builder;
    }
}