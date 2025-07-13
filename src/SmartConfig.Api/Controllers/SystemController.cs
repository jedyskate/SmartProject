using Microsoft.AspNetCore.Mvc;
using SmartConfig.Api.Models;

namespace SmartConfig.Api.Controllers;

public class SystemController : BaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    public IActionResult GetSmartConfigEnvironment()
    {
        var result = new ApiResponse<string> { Response = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") };
        return Ok(result);
    }
}