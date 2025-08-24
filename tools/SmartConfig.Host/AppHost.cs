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
    .WithReference(dbs.SchedulerDb)
    .WaitForCompletion(migration);

// Frontends (next.js, angular, react and blazor)
builder.AddFrontends(api);

builder.Build().Run();