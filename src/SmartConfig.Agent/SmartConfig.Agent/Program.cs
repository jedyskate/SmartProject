using SmartConfig.Agent.Extensions;

namespace SmartConfig.Agent;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services
        builder.Services.AddOpenApi();
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

        builder.AddServiceDefaults();
        builder.AddSwaggerDocumentation();

        var app = builder.Build();

        app.UseSwaggerDocumentation();
        app.UseHttpsRedirection();
        app.AddApiEndpoints();
        
        app.Run();
    }
}