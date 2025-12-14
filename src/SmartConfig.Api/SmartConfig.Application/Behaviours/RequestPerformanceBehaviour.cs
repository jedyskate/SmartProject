using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartConfig.Application.Telemetry;

namespace SmartConfig.Application.Behaviours;

public class RequestPerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<TRequest> _logger;
    private const int SlowRequestThresholdMs = 500;

    public RequestPerformanceBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var isCommand = requestName.EndsWith("Command");
        var isQuery = requestName.EndsWith("Query");

        var stopwatch = Stopwatch.StartNew();

        var response = await next();

        stopwatch.Stop();
        var elapsedMs = stopwatch.Elapsed.TotalMilliseconds;

        // Record metrics
        if (isCommand)
        {
            ApplicationTelemetry.CommandDurationHistogram.Record(elapsedMs,
                new KeyValuePair<string, object?>("command", requestName));
        }
        else if (isQuery)
        {
            ApplicationTelemetry.QueryDurationHistogram.Record(elapsedMs,
                new KeyValuePair<string, object?>("query", requestName));
        }

        // Log slow requests
        if (elapsedMs > SlowRequestThresholdMs)
        {
            _logger.LogWarning("Slow request detected: {RequestType} took {ElapsedMilliseconds}ms {@Request}",
                requestName, elapsedMs, request);
        }

        return response;
    }
}