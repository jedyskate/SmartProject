using SmartConfig.AI.Sdk;
using SmartConfig.AI.Sdk.Extensions;
using SmartConfig.App.Web.Components;
using SmartConfig.App.Shared.Services;
using SmartConfig.App.Web.Client.Extensions;
using SmartConfig.App.Web.Extensions;
using SmartConfig.App.Web.Services;
using SmartConfig.BE.Sdk;
using SmartConfig.BE.Sdk.Extensions;
using SmartConfig.BE.Sdk.Queue;
using SmartConfig.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add device-specific services used by the SmartConfig.App.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();



// SPECIFIC
builder.AddServiceDefaults();
builder.Services.AddCommonClientIoc(builder.Configuration);

// Add SmartConfig API Proxy.
builder.Services.AddSingleton(new SmartConfigApiSettings
{
    SmartConfigApiEndpoint = builder.Configuration["SmartConfig:ApiEndpoint"]!,
    ApplicationName = "SmartConfig.Blazor",
    DryRun = false
});
builder.Services.AddSingleton(new SmartConfigQueueSettings
{
    HostName = builder.Configuration["RabbitMq:TcpEndpoint:HostName"]!,
    Port = int.Parse(builder.Configuration["RabbitMq:TcpEndpoint:Port"]!),
    UserName = builder.Configuration["RabbitMq:Credentials:UserName"]!,
    Password = builder.Configuration["RabbitMq:Credentials:Password"]!,
    VirtualHost = builder.Configuration["RabbitMq:Credentials:VirtualHost"]!,
    Exchange = builder.Configuration["RabbitMq:SmartConfigMq:Exchange"]!,
    Queue = builder.Configuration["RabbitMq:SmartConfigMq:Queue"]!,
    RoutingKey = builder.Configuration["RabbitMq:SmartConfigMq:RoutingKey"]!,
    Environment = builder.Environment.EnvironmentName
});
builder.Services.AddSmartConfigApiClient(true);
        
// Add SmartConfig Agent Proxy.
builder.Services.AddSingleton(new SmartConfigAgentSettings
{
    SmartConfigAgentEndpoint = builder.Configuration["SmartConfig:AgentEndpoint"]!,
    ApplicationName = "SmartConfig.Blazor.Client",
    DryRun = false
});
builder.Services.AddSmartConfigAgentClient();

// Add YARP to proxy FE requests to SmartConfig API
builder.AddYarp();



var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseAntiforgery();
app.MapDefaultEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(SmartConfig.App.Shared._Imports).Assembly,
        typeof(SmartConfig.App.Web.Client._Imports).Assembly);

app.MapReverseProxy();

app.Run();