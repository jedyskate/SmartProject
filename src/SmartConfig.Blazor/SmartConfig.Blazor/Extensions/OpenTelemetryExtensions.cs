using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;

namespace SmartConfig.Blazor.Extensions;

public static class OpenTelemetryExtensions
{
    public static WebApplicationBuilder AddServerOpenTelemetry(this WebApplicationBuilder builder)
    {
        var resourceBuilder = ResourceBuilder.CreateEmpty();

        // Add Tracing and Metrics to the service collection.
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(r =>
            {
                resourceBuilder = r.AddService("blazor", "1.0.0", Environment.MachineName)
                    .AddAttributes(new Dictionary<string, object>
                    {
                        ["deployment.environment"] = builder.Environment.EnvironmentName.ToLowerInvariant(),
                        ["host.name"] = Environment.MachineName,
                    });
                r.Build();
            })
            .WithTracing(tracing =>
            {
                tracing.AddSource("Microsoft.AspNetCore.Components");
                tracing.AddHttpClientInstrumentation();
                tracing.AddAspNetCoreInstrumentation(options =>
                {
                    options.EnrichWithHttpRequest = (activity, httpRequest) =>
                    {
                        activity.SetTag("request.path", httpRequest.Path);
                        activity.SetTag("request.host", httpRequest.Host);
                    };
                });
                tracing.AddOtlpExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics.AddMeter("Microsoft.AspNetCore.Components");
                metrics.AddAspNetCoreInstrumentation();
                metrics.AddHttpClientInstrumentation();
                metrics.AddRuntimeInstrumentation();
                metrics.AddProcessInstrumentation();
                metrics.AddOtlpExporter();
            })
            .WithLogging()
            .UseOtlpExporter();

        // Configure logging and add the OpenTelemetry logger provider.
        builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
            options.ParseStateValues = true;
            options.SetResourceBuilder(resourceBuilder);
            options.AddOtlpExporter();
        });

        return builder;
    }
}