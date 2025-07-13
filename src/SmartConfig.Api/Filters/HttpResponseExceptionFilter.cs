using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SmartConfig.Api.Models;
using SmartConfig.Common.Exceptions;

namespace SmartConfig.Api.Filters;

//https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-3.1
public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order { get; set; } = int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is SmartConfigException exception)
        {
            var apiResponse = new ApiErrorResponse
            {
                Errors = new List<ApiError> {new ApiError
                {
                    Message = exception.Message,
                    ErrorCode = exception.StatusCode.ToString()
                }}
            };

            context.Result = new ObjectResult(apiResponse)
            {
                StatusCode = exception.StatusCode
            };
            context.ExceptionHandled = true;
        }
        else if (context.Exception  != null)
        {
            var apiResponse = new ApiErrorResponse
            {
                Errors = new List<ApiError> { new ApiError
                {
                    Message = $"Unhandled exception. Details: {context.Exception.Message}",
                    ErrorCode = ((int)HttpStatusCode.InternalServerError).ToString()
                } }
            };

            context.Result = new ObjectResult(apiResponse)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
            context.ExceptionHandled = true;
        }
    }
}