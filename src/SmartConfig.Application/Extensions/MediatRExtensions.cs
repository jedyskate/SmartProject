using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using SmartConfig.Application.Behaviours;

namespace SmartConfig.Application.Extensions;

public static class MediatRExtensions
{
    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        //PIPELINES
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<MediatRAssembly>());

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestLoggerBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

        return services;
    }
}

public class MediatRAssembly
{
}