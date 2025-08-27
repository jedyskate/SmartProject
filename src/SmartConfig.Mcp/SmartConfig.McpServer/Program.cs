using SmartConfig.McpServer.Tools;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithTools<EchoTool>();

var app = builder.Build();

app.MapMcp();
app.MapDefaultEndpoints();

app.Run();