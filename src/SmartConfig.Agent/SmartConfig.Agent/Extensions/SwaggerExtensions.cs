namespace SmartConfig.Agent.Extensions;

public static class SwaggerExtensions
{
    public static WebApplicationBuilder AddSwaggerDocumentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer(); // Required for minimal APIs
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "AI Agent API",
                Version = "v1",
                Description = "AI Agent API"
            });
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