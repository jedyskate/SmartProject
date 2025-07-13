namespace SmartConfig.Host.Extensions;

public static class OpenTelemetryExtensions
{
    public static IResourceBuilder<ContainerResource> AddOTelCollector(this IDistributedApplicationBuilder builder)
    {
        var otelCollector = builder.AddContainer("otel-collector", "otel/opentelemetry-collector:latest")
            .WithBindMount("otel-config.yaml", "/etc/otelcol/otel-config.yaml")
            .WithHttpEndpoint(name: "http", port: 4318, targetPort: 4318)
            .WithArgs("--config", "/etc/otelcol/otel-config.yaml");

        return otelCollector;
    }
}