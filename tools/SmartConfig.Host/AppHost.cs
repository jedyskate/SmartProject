using Projects;
using SmartConfig.Host.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

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

// Scheduler
var scheduler = builder.AddProject<SmartConfig_Scheduler>("scheduler")
    .WaitFor(rabbitMq)
    .WithReference(dbs.SchedulerDb)
    .WaitForCompletion(migration);

// Ollama
var ollama = builder.AddOllama("ollama")
    .WithDataVolume()
    .AddModel("phi4-mini", "phi4-mini");
    // .AddModel("llama3-8b-instruct", "koesn/llama3-8b-instruct");

// Mcp Server
var mcp = builder.AddProject<SmartConfig_McpServer>("mcp")
    .WaitFor(ollama)
    .WithExternalHttpEndpoints();

var anythingLlm = builder.AddContainer("anythingllm", "mintplexlabs/anythingllm", "latest")
    .WithReference(mcp)
    .WaitFor(mcp)
    .WithEnvironment("OLLAMA_API_BASE", "http://localhost:11434")
    .WithEnvironment("STORAGE_DIR", "/app/server/storage")
    .WithVolume("anythingllm-storage", "/app/server/storage")
    .WithBindMount("Configurations/anythingllm_mcp_servers.json", "/app/server/storage/plugins/anythingllm_mcp_servers.json")
    .WithBindMount("Configurations/empty.json", "/app/server/storage/plugins/agent-flows/empty.json") // Directory is needed
    .WithBindMount("Configurations/empty.json", "/app/server/storage/plugins/agent-skills/empty.json") // Directory is needed
    .WithHttpEndpoint(targetPort: 3001, name: "http", isProxied: true);

// Frontends (next.js, angular, react and blazor)
builder.AddFrontends(api);

builder.Build().Run();