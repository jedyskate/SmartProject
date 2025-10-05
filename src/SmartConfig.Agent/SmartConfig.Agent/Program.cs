using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using SmartConfig.Agent.Extensions;
using SmartConfig.AiAgent.Extensions;
using SmartConfig.ServiceDefaults;

namespace SmartConfig.Agent;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        // Add services
        builder.Services.AddOpenApi();
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

        builder.AddServiceDefaults();
        builder.AddSwaggerDocumentation();
        builder.AddAiAgentIoc();
        builder.AddSmartConfigApi();

        var app = builder.Build();

        app.UseSwaggerDocumentation();
        app.UseHttpsRedirection();
        app.AddApiEndpoints();
        
        app.Run();
    }
}