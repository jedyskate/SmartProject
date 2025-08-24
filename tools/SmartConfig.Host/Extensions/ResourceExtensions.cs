using Projects;

namespace SmartConfig.Host.Extensions;

public static class ResourceExtensions
{
    public static (IResourceBuilder<SqlServerDatabaseResource> SmartConfigDb, IResourceBuilder<SqlServerDatabaseResource> SchedulerDb) 
        AddDatabases(this IDistributedApplicationBuilder builder)
    {
        // Set up the SQL Server container once
        var sqlServer = builder
            .AddSqlServer("sql", port: 1800)
            .WithLifetime(ContainerLifetime.Persistent);

        var smartConfigDb = sqlServer.AddDatabase("SmartConfig");
        var schedulerDb = sqlServer.AddDatabase("TickerQ");

        return (smartConfigDb, schedulerDb);
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

    public static void AddFrontends(this IDistributedApplicationBuilder builder, IResourceBuilder<ProjectResource> api)
    {
        var otlpEndpoint = Environment.GetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL") ?? throw new ArgumentException();
        var otlpHeaders = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS") ?? throw new ArgumentException();

        var apiPort = api.Resource.Annotations.OfType<EndpointAnnotation>()
            .FirstOrDefault(r => r.Name == "https")?.Port ?? throw new ArgumentException();
        
        // Frontend NextJs
        var nextjs = builder.AddNpmApp("nextjs", "../../src/SmartConfig.NextJs", "dev")
            .WaitFor(api)
            .WithReference(api)
            .WithHttpsEndpoint(7042, env: "PORT", name: "nextjs-https")
            .WithExternalHttpEndpoints()
            .WithParentRelationship(api)
            .WithEnvironment("NEXT_PUBLIC_API_BASE_URL", $"https://localhost:{apiPort.ToString()}")
            .WithEnvironment("NEXT_PUBLIC_OTEL_EXPORTER_OTLP_ENDPOINT", otlpEndpoint)
            .WithEnvironment("NEXT_PUBLIC_OTEL_EXPORTER_OTLP_HEADERS", otlpHeaders);

        // Frontend Angular
        var angular = builder.AddNpmApp("angular", "../../src/SmartConfig.Angular", "start")
            .WaitFor(api)
            .WithReference(api)
            .WithUrl("https://localhost:4200")
            .WithExternalHttpEndpoints()
            .WithParentRelationship(api)
            .WithEnvironment("API_BASE_URL", $"https://localhost:{apiPort.ToString()}")
            .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", otlpEndpoint)
            .WithEnvironment("OTEL_EXPORTER_OTLP_HEADERS", otlpHeaders);

        // Frontend React
        var react = builder.AddNpmApp("react", "../../src/SmartConfig.React", "dev")
            .WaitFor(api)
            .WithReference(api)
            .WithUrl("https://localhost:5175")
            .WithExternalHttpEndpoints()
            .WithParentRelationship(api)
            .WithEnvironment("VITE_API_BASE_URL", $"https://localhost:{apiPort.ToString()}")
            .WithEnvironment("VITE_OTEL_EXPORTER_OTLP_ENDPOINT", otlpEndpoint)
            .WithEnvironment("VITE_OTEL_EXPORTER_OTLP_HEADERS", otlpHeaders);
        
        // Frontend Blazor
        var blazor = builder.AddProject<SmartConfig_Blazor>("blazor")
            .WaitFor(api)
            .WithReference(api)
            .WithHttpsEndpoint(7052, name: "blazor-https")
            .WithParentRelationship(api);
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