using MediatR;
using SmartConfig.Api.Models;

namespace SmartConfig.Api.Extensions;

public static class ApiResponseExtensions
{
    public static async Task<ApiResponse<T>> GetApiResponse<T>(this IMediator mediator, IRequest<T> request)
    {
        return new ApiResponse<T>
        {
            Response = await mediator.Send<T>(request)
        };
    }
}