using Microsoft.Extensions.Configuration;
using Projects;

namespace SmartConfig.Host.Extensions;

public static class ResourceFrontendExtensions
{
    public static void AddFrontendResources(this IDistributedApplicationBuilder builder, IResourceBuilder<ProjectResource> api)
    {
        var otlpEndpoint = Environment.GetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL") ?? throw new ArgumentException();
        var otlpHeaders = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS") ?? throw new ArgumentException();

        var frontends = builder.Configuration.GetSection("SmartConfig:Clients:Frontends").Get<string[]>();
        var apiPort = api.Resource.Annotations.OfType<EndpointAnnotation>()
            .FirstOrDefault(r => r.Name == "https")?.Port ?? throw new ArgumentException();
        
        // NextJs
        if (frontends?.Contains("nextjs") ?? false)
            builder.AddNpmApp("nextjs", "../../src/SmartConfig.NextJs", "dev")
                .WaitFor(api)
                .WithReference(api)
                .WithHttpsEndpoint(7042, env: "PORT", name: "nextjs-https")
                .WithExternalHttpEndpoints()
                .WithParentRelationship(api)
                .WithEnvironment("NEXT_PUBLIC_API_BASE_URL", $"https://localhost:{apiPort.ToString()}")
                .WithEnvironment("NEXT_PUBLIC_OTEL_EXPORTER_OTLP_ENDPOINT", otlpEndpoint)
                .WithEnvironment("NEXT_PUBLIC_OTEL_EXPORTER_OTLP_HEADERS", otlpHeaders);
        
        // Angular
        if (frontends?.Contains("angular") ?? false)
            builder.AddNpmApp("angular", "../../src/SmartConfig.Angular", "start")
                .WaitFor(api)
                .WithReference(api)
                .WithUrl("https://localhost:4200")
                .WithExternalHttpEndpoints()
                .WithParentRelationship(api)
                .WithEnvironment("API_BASE_URL", $"https://localhost:{apiPort.ToString()}")
                .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", otlpEndpoint)
                .WithEnvironment("OTEL_EXPORTER_OTLP_HEADERS", otlpHeaders);
        
        // React
        if (frontends?.Contains("react") ?? false)
            builder.AddNpmApp("react", "../../src/SmartConfig.React", "dev")
                .WaitFor(api)
                .WithReference(api)
                .WithUrl("https://localhost:5175")
                .WithExternalHttpEndpoints()
                .WithParentRelationship(api)
                .WithEnvironment("VITE_API_BASE_URL", $"https://localhost:{apiPort.ToString()}")
                .WithEnvironment("VITE_OTEL_EXPORTER_OTLP_ENDPOINT", otlpEndpoint)
                .WithEnvironment("VITE_OTEL_EXPORTER_OTLP_HEADERS", otlpHeaders);
        
        // Blazor
        if (frontends?.Contains("blazor") ?? false)
            builder.AddProject<SmartConfig_Blazor>("blazor")
                .WaitFor(api)
                .WithReference(api)
                .WithHttpsEndpoint(7052, name: "blazor-https")
                .WithParentRelationship(api);
    }
}