using SmartConfig.McpServer.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithMcpTools();

var app = builder.Build();

app.MapMcp();
app.MapDefaultEndpoints();

app.Run();