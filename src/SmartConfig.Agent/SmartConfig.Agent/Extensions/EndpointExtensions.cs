using System.Text.Json;
using MediatR;
using SmartConfig.Agent.Endpoints.Commands;
using SmartConfig.Agent.Endpoints.Queries;
using SmartConfig.AiAgent.Models;

namespace SmartConfig.Agent.Extensions;

public static class EndpointExtensions
{
    public static WebApplication AddApiEndpoints(this WebApplication app)
    {
        // Weather
        app.MapGet("/agent/weatherforecast", async (IMediator mediator) => await mediator.Send(new WeatherForecastQuery()))
            .WithName("WeatherForecast")
            .WithOpenApi();
        
        // Chat Agent
        app.MapPost("/agent/completechatstreaming", async (CompleteChatCommand command, IMediator mediator, HttpContext httpContext, ILogger<Program> logger) =>
            {
                httpContext.Response.StatusCode = StatusCodes.Status200OK;
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.Headers.Append("Cache-Control", "no-cache");
                httpContext.Response.Headers.Append("X-Content-Type-Options", "nosniff");

                var cancellationToken = httpContext.RequestAborted;

                try
                {
                    await foreach (var chunk in mediator.CreateStream(command, cancellationToken))
                    {
                        var json = JsonSerializer.Serialize(chunk);
                        await httpContext.Response.WriteAsync(json + "\n", cancellationToken);
                        await httpContext.Response.Body.FlushAsync(cancellationToken);
                    }
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("Streaming cancelled by client.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred during chat streaming.");
                }
            })
            .WithName("CompleteChatStreaming")
            .Produces<ChatResponse>()
            .WithOpenApi();
        
        return app;
    }
}