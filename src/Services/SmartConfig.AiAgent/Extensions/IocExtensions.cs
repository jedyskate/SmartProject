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
        var ollama = new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.2");

        builder.Services.AddScoped<IKernelService, KernelService>();
        builder.Services.AddSingleton<Kernel>(sp =>
        {
            // var ollamaClient = sp.GetRequiredKeyedService<IOllamaApiClient>("ollama-llama32");

            var kernel = Kernel.CreateBuilder()
                .AddOllamaChatCompletion(
                    ollamaClient: ollama,
                    serviceId: "ollama")
                .Build();

            return kernel;
        });
        
        return builder;
    }
}