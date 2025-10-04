using Microsoft.OpenApi.Models;
using SmartConfig.Agent.Filters;

namespace SmartConfig.Agent.Extensions;

public static class SwaggerExtensions
{
    public static WebApplicationBuilder AddSwaggerDocumentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer(); // Required for minimal APIs
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "AI Agent API",
                Version = "v1",
                Description = "AI Agent API"
            });
            
            options.MapType<decimal>(() => new OpenApiSchema { Type = "number", Format = "decimal" });
            options.SchemaFilter<EnumSchemaFilter>();
        });
        
        return builder;
    }
    
    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "AI Agent API v1 Documentation");
        });

        return app;
    }
}