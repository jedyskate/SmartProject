    using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartConfig.Api.Extensions;
using SmartConfig.Api.Models;
using SmartConfig.Application.Application.Orleans;

namespace SmartConfig.Api.Controllers;

public class OrleansController : BaseController
{
    private readonly IMediator _mediator;

    public OrleansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> HelloWorld(HelloWorldCommand command)
    {
        var result = await _mediator.GetApiResponse(command);
        return Ok(result);
    }
        
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SuspiciousEmailDomains(SuspiciousEmailDomainsCommand command)
    {
        var result = await _mediator.GetApiResponse(command);
        return Ok(result);
    }
}