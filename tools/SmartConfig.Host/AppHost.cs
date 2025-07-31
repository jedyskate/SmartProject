using Projects;
using SmartConfig.Host.Extensions;

var builder = DistributedApplication.CreateBuilder(args);
var otlpEndpoint = Environment.GetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL") ?? throw new ArgumentException();
var otlpHeaders = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS") ?? throw new ArgumentException();

// Database
var db = builder.AddDatabase();

// RabbitMq
var rabbitMq = builder.AddRabbitMq();

// Data Migration
var migration = builder.AddProject<SmartConfig_Migration>("migration")
    .WithReference(db)
    .WaitFor(db);

// Backend
var api = builder.AddProject<SmartConfig_Api>("api")
    .WaitFor(rabbitMq)
    .WithReference(db)
    .WaitForCompletion(migration);
var apiPort = api.Resource.Annotations.OfType<EndpointAnnotation>()
    .FirstOrDefault(r => r.Name == "https")?.Port ?? throw new ArgumentException();

// Frontend NextJs
var nextjs = builder.AddNpmApp("nextjs", "../../src/SmartConfig.NextJs", "dev")
    .WaitFor(api)
    .WithReference(api)
    .WithHttpsEndpoint(7042, env: "PORT", name: "nextjs-https") 
    .WithExternalHttpEndpoints()
    .WithEnvironment("NEXT_PUBLIC_API_BASE_URL", $"https://localhost:{apiPort.ToString()}")
    .WithEnvironment("NEXT_PUBLIC_OTEL_EXPORTER_OTLP_ENDPOINT", otlpEndpoint)
    .WithEnvironment("NEXT_PUBLIC_OTEL_EXPORTER_OTLP_HEADERS", otlpHeaders);

// Frontend Angular
var angular = builder.AddNpmApp("angular", "../../src/SmartConfig.Angular", "start")
    .WaitFor(api)
    .WithReference(api)
    .WithUrl("https://localhost:4200")
    .WithExternalHttpEndpoints()
    .WithEnvironment("API_BASE_URL", $"https://localhost:{apiPort.ToString()}")
    .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", otlpEndpoint)
    .WithEnvironment("OTEL_EXPORTER_OTLP_HEADERS", otlpHeaders);

// Frontend Blazor
var blazor = builder.AddProject<SmartConfig_Blazor>("blazor")
        .WaitFor(api)
        .WithReference(api)
        .WithHttpsEndpoint(7052, name: "blazor-https");

builder.Build().Run();