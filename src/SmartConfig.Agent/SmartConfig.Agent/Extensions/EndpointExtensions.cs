using MediatR;
using SmartConfig.Agent.Endpoints.Queries;

namespace SmartConfig.Agent.Extensions;

public static class EndpointExtensions
{
    public static WebApplication AddApiEndpoints(this WebApplication app)
    {
        app.MapGet("/weatherforecast", async (IMediator mediator) => await mediator.Send(new WeatherForecastQuery()))
            .WithName("WeatherForecast")
            .WithOpenApi();
        
        return app;
    }
}