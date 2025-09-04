using Projects;
using SmartConfig.Host.Extensions;

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

// Backend
var api = builder.AddProject<SmartConfig_Api>("api")
    .WithReference(dbs.SmartConfigDb)
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
builder.AddFrontendResources(api);

builder.Build().Run();