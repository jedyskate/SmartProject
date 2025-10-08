using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SmartConfig.Api.Models;

namespace SmartConfig.Api.Filters;

public class CustomValidationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState.Values.Where(v => v.Errors.Count > 0)
                .SelectMany(v => v.Errors)
                .ToList();

            var apiResponse = new ApiErrorResponse
            {
                Errors = errors.Select(r => new ApiError
                {
                    Message = r.ErrorMessage,
                    ErrorCode = ((int)HttpStatusCode.BadRequest).ToString()
                }).ToList()
            };

            context.Result = new JsonResult(apiResponse)
            {
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }
    }
}