using Projects;
using SmartConfig.Host.Extensions;

var builder = DistributedApplication.CreateBuilder(args);
var otlpEndpoint = Environment.GetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL") ?? throw new ArgumentException();
var otlpHeaders = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS") ?? throw new ArgumentException();

// Databases
var dbs = builder.AddDatabases();

// RabbitMq
var rabbitMq = builder.AddRabbitMq();

// Data Migration
var migration = builder.AddProject<SmartConfig_Migration>("migration")
    .WithReference(dbs.SmartConfigDb)
    .WithReference(dbs.SchedulerDb)
    .WaitFor(dbs.SmartConfigDb)
    .WaitFor(dbs.SchedulerDb);

// Backend
var api = builder.AddProject<SmartConfig_Api>("api")
    .WaitFor(rabbitMq)
    .WithReference(dbs.SmartConfigDb)
    .WaitForCompletion(migration);
var apiPort = api.Resource.Annotations.OfType<EndpointAnnotation>()
    .FirstOrDefault(r => r.Name == "https")?.Port ?? throw new ArgumentException();

// Scheduler
var scheduler = builder.AddProject<SmartConfig_Scheduler>("scheduler")
    .WithReference(dbs.SchedulerDb)
    .WaitForCompletion(migration)
    .WithHttpsEndpoint(7068, env: "PORT", name: "scheduler-https")
    .WithExternalHttpEndpoints();

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