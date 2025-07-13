using Microsoft.AspNetCore.Mvc;
using SmartConfig.Api.Models;

namespace SmartConfig.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[ProducesResponseType(typeof(ApiErrorResponse), (int)HttpStatusCode.InternalServerError)]
[ProducesResponseType(typeof(ApiErrorResponse), (int)HttpStatusCode.NotFound)]
public class BaseController : ControllerBase
{
}