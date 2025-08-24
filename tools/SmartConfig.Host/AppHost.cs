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
    .WithOpenWebUI()
    .AddModel("phi4-mini");

// Mcp Server
var mcp = builder.AddProject<SmartConfig_McpServer>("mpc")
    .WaitFor(ollama)
    .WithExternalHttpEndpoints();

// Frontends (next.js, angular, react and blazor)
builder.AddFrontends(api);

builder.Build().Run();