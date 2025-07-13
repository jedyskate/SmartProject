using Newtonsoft.Json;
using SmartConfig.Api.Filters;
using SmartConfig.Application.Extensions;
using SmartConfig.Common.Extensions;
using SmartConfig.Data;

namespace SmartConfig.Api.Extensions;

public static class IocExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddControllers(options =>
            {
                options.Filters.Add(typeof(HttpResponseExceptionFilter));
                options.Filters.Add(typeof(CustomValidationFilter));
            })
            .AddValidators(services) // Assuming AddValidators is an extension method
            .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

        services.AddMvc();
        services.AddMediatR();

        services.AddApplication(configuration);
        services.AddCommon();
        services.AddOrleansInterface(configuration);
        services.AddRabbitMqConsumer(configuration);
        services.AddSwaggerDocumentation();
        services.AddAppSettingsIoc(configuration);
        services.AddSecurityIoc();
        services.AddCorsConfiguration();
        services.AddOpenTelemetryConfiguration();

        return builder;
    }
    
    public static IApplicationBuilder Initialize(this IApplicationBuilder app, IConfiguration configuration, 
        ISeedData seedData)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (env == "Development")
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseEntityFramework(seedData);
        app.UseSwaggerDocumentation();
        app.UseHttpsRedirection();
        app.UseOrleansInterface(configuration);
        app.UseRouting();
        app.UseSecurityIoc();
        app.UseCorsConfiguration();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        return app;
    }
}