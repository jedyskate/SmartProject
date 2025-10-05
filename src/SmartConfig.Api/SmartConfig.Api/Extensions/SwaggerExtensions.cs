using Microsoft.OpenApi.Models;
using SmartConfig.Api.Filters;

namespace SmartConfig.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        string testToken = String.Empty;
        switch (env)
        {
            case "Local":
                testToken = "bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjbGllbnRfaWQiOiJTbWFydFdheS5XZWJhcHAiLCJ1c2VyX2lkIjoiMSIsInVuaXF1ZV9uYW1lIjoiaGVsbG9AcXVlcnlvdXQuY29tIiwicm9sZSI6IlNtYXJ0QXBwLFdlYmFwcCxVc2VyLEFkbWluLE9yZ2FuaXNhdGlvbiIsInBlcm1pc3Npb25zIjoiU21hcnRBdXRoLFNtYXJ0V2F5LFNtYXJ0Q29uZmlnLFNtYXJ0TG9nIiwic2Vzc2lvbl9pZCI6IjM2NjY1NDI3LTU5NmYtNDU5MS1hOTIwLTIyYTI5Njg4OTQyYiIsIm5iZiI6MTY3NTM5NTM4MiwiZXhwIjoxOTI3ODU2MTgxLCJpYXQiOjE2NzUzOTUzODIsImlzcyI6IlNtYXJ0QXV0aC1Mb2NhbCIsImF1ZCI6Imh0dHBzOi8vc21hcnR3YXkifQ.6-ZBEExVWZTDc9ORMCeYMyBB4D6uzSu12AMteiTfgHo";
                break;
            case "Test":
                testToken = "bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjbGllbnRfaWQiOiIzIiwicm9sZSI6IjMyIiwibmJmIjoxNTgxNzU0NjA3LCJleHAiOjE4OTcxMTQ2MDcsImlhdCI6MTU4MTc1NDYwNywiaXNzIjoiU21hcnRBdXRoIiwiYXVkIjoiU21hcnRBcHBzIn0.em8TKzTBLrIif7Qsx8_9AXiGvwmkNEoFMHEQIfRMwp8";
                break;
        }

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = $"SmartConfig Rest API - {env}", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = $@"JWT Authorization header using the Bearer scheme. 
                      Enter 'bearer' [space] and then your token in the text input below.
                      Example: '{testToken}'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = ""
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
            c.MapType<decimal>(() => new OpenApiSchema { Type = "number", Format = "decimal" });
            c.SchemaFilter<SwaggerSchemaFilter>();
        });

        services.AddSwaggerGenNewtonsoftSupport();

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
        });

        return app;
    }
}