using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartConfig.Application.Telemetry;

namespace SmartConfig.Application.Behaviours;

public class RequestLoggerBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger _logger;

    public RequestLoggerBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestType = typeof(TRequest);
        var requestName = requestType.Name;
        var isCommand = requestName.EndsWith("Command");
        var isQuery = requestName.EndsWith("Query");

        using var activity = ApplicationTelemetry.ActivitySource.StartActivity($"MediatR.{requestName}");
        activity?.SetTag("request.type", requestName);
        activity?.SetTag("request.kind", isCommand ? "Command" : isQuery ? "Query" : "Request");

        _logger.LogInformation("Request started: {RequestType} {@Request}", requestName, request);

        try
        {
            var response = await next();

            _logger.LogInformation("Request completed: {RequestType}", requestName);

            // Increment success counters
            if (isCommand)
            {
                ApplicationTelemetry.CommandExecutionCounter.Add(1, new KeyValuePair<string, object?>("command", requestName));
            }
            else if (isQuery)
            {
                ApplicationTelemetry.QueryExecutionCounter.Add(1, new KeyValuePair<string, object?>("query", requestName));
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Request failed: {RequestType}", requestName);

            // Increment failure counters
            if (isCommand)
            {
                ApplicationTelemetry.CommandFailureCounter.Add(1,
                    new KeyValuePair<string, object?>("command", requestName),
                    new KeyValuePair<string, object?>("error", ex.GetType().Name));
            }
            else if (isQuery)
            {
                ApplicationTelemetry.QueryFailureCounter.Add(1,
                    new KeyValuePair<string, object?>("query", requestName),
                    new KeyValuePair<string, object?>("error", ex.GetType().Name));
            }

            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}