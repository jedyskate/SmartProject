namespace SmartConfig.Agent.Extensions;

public static class CorsExtensions
{
    public static WebApplicationBuilder AddCorsConfig(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return builder;
    }
    
    public static WebApplication UseCorsConfig(this WebApplication app)
    {
        app.UseCors("AllowAll");

        return app;
    }
}