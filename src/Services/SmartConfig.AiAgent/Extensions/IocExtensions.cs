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
        builder.AddKeyedOllamaApiClient("ollama-phi4-mini");

        builder.Services.AddScoped<IKernelService, KernelService>();
        builder.Services.AddSingleton<Kernel>(sp =>
        {
            var ollamaClient = sp.GetRequiredKeyedService<IOllamaApiClient>("ollama-phi4-mini");

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