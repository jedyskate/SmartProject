using DotNetEnv;
using Projects;
using SmartConfig.Host.Extensions;


Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env.local")); // HIDDEN SECRETS
var builder = DistributedApplication.CreateBuilder(args);

// Databases
var dbs = builder.AddDatabaseResources();

// RabbitMq
var rabbitMq = builder.AddRabbitMqResource();

// Data Migration
var migration = builder.AddProject<SmartConfig_Migration>("migration")
    .WithReference(dbs.SmartConfigDb)
    .WithReference(dbs.SchedulerDb)
    .WaitFor(dbs.SmartConfigDb)
    .WaitFor(dbs.SchedulerDb);

// Ollama
var ollama = builder.AddOllama("ollama")
    .WithHttpEndpoint(port: 11434, targetPort: 11434, name: "ollama-http", isProxied: false)
    .WithExternalHttpEndpoints()
    .WithDataVolume()
    // .AddModel("phi4-mini", "phi4-mini:latest");
    .AddModel("llama32", "llama3.2:latest");

// Agent
var agent = builder.AddProject<SmartConfig_Agent>("agent")
    .WithReference(ollama)
    .WaitFor(ollama)
    .WithEnvironment("Agent__OpenRouter__ApiKey", builder.Configuration["Agent:OpenRouter:ApiKey"]);

// Backend
var api = builder.AddProject<SmartConfig_Api>("api")
    .WithReference(dbs.SmartConfigDb)
    .WithReference(agent)
    .WaitFor(agent) 
    .WaitFor(rabbitMq)
    .WaitForCompletion(migration);

// Scheduler
if (bool.Parse(builder.Configuration["SmartConfig:Clients:Scheduler"] ?? "false"))
    builder.AddProject<SmartConfig_Scheduler>("scheduler")
        .WaitFor(rabbitMq)
        .WithReference(dbs.SchedulerDb)
        .WaitForCompletion(migration);

// AI
builder.AddAiResources(api);

// Frontends (next.js, angular, react and blazor)
// builder.AddFrontendResources(api);

builder.Build().Run();