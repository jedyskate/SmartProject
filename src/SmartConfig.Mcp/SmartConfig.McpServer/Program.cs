using SmartConfig.McpServer.Extensions;
using SmartConfig.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddSmartConfig();
builder.AddServiceDefaults();

builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithMcpTools();

var app = builder.Build();

app.MapMcp();
app.MapDefaultEndpoints();

app.Run();