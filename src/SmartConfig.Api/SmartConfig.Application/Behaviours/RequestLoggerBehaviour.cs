using MediatR;
using Microsoft.Extensions.Logging;

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
        var name = typeof(TRequest).Name;
        _logger.LogInformation($"SmartConfig request started: ClientId: AAAA UserId: AAAA Operation: {name} Request: {request}");

        var response = await next();

        _logger.LogInformation($"SmartConfig request ended: ClientId: AAAA UserId: AAAA Operation: {name} Request: {request}");
        return response;
    }
}