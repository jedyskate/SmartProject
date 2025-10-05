using Microsoft.AspNetCore.Authorization;
using SmartConfig.Api.Handlers;

namespace SmartConfig.Api.Extensions;

public static class SecurityExtensions
{
    public static IServiceCollection AddSecurityIoc(this IServiceCollection services)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        services.AddAuthentication().AddJwtBearer();
        services.AddAuthorization(options =>
        {
            options.AddPolicy("User", policy => policy.Requirements.Add(new PolicyRequirement("User")));
        });

        services.AddSingleton<IAuthorizationHandler, PolicyHandler>();


        return services;
    }

    public static IApplicationBuilder UseSecurityIoc(this IApplicationBuilder app)
    {
        app.UseAuthorization();

        return app;
    }
}