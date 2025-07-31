namespace SmartConfig.Host.Extensions;

public static class ResourceExtensions
{
    public static IResourceBuilder<SqlServerDatabaseResource> AddDatabase(this IDistributedApplicationBuilder builder)
    {
        // var sqlPassword = builder.AddParameter("sqlPassword", secret: true);
        var resource = builder
            .AddSqlServer("sql", port: 1800)
            .WithLifetime(ContainerLifetime.Persistent)
            .AddDatabase("SmartConfig");

        return resource;
    }
    
    public static IResourceBuilder<ContainerResource> AddRabbitMq(this IDistributedApplicationBuilder builder)
    {
        var resource = builder.AddContainer("rabbitmq", "rabbitmq:3.11-management-alpine")
            .WithEnvironment("RABBITMQ_DEFAULT_USER", "admin")
            .WithEnvironment("RABBITMQ_DEFAULT_PASS", "admin123")
            .WithHttpEndpoint(name: "management", port: 15672, targetPort: 15672) // Management UI
            .WithEndpoint(name: "amqp", port: 5672, targetPort: 5672)             // AMQP default
            .WithEndpoint(name: "amqp-alt", port: 5673, targetPort: 5673);        // Optional second port

        return resource;
    }
    
    public static IResourceBuilder<ContainerResource> AddOTelCollector(this IDistributedApplicationBuilder builder)
    {
        var resource = builder.AddContainer("otel-collector", "otel/opentelemetry-collector:latest")
            .WithBindMount("otel-config.yaml", "/etc/otelcol/otel-config.yaml")
            .WithHttpEndpoint(name: "http", port: 4318, targetPort: 4318)
            .WithArgs("--config", "/etc/otelcol/otel-config.yaml");

        return resource;
    }
}