using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartConfig.Api.Extensions;
using SmartConfig.Api.Models;
using SmartConfig.Application.Application.UserConfig.Commands;
using SmartConfig.Application.Application.UserConfig.Queries;
using SmartConfig.Common.Models;
using SmartConfig.Core.Models;

namespace SmartConfig.Api.Controllers;

public class UserConfigController : BaseController
{
    private readonly IMediator _mediator;

    public UserConfigController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    // [Authorize(Policy = "User")]
    [ProducesResponseType(typeof(ApiResponse<UserConfig>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreateUserConfig([FromBody]CreateUserConfigCommand command)
    {
        return Ok(await _mediator.GetApiResponse<UserConfig>(command));
    }

    [HttpPost]
    // [Authorize(Policy = "User")]
    [ProducesResponseType(typeof(ApiResponse<UserConfig>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpsertUserConfig([FromBody]UpsertUserConfigCommand command)
    {
        return Ok(await _mediator.GetApiResponse<UserConfig>(command));
    }

    [HttpPost]
    // [Authorize(Policy = "User&SmartWorker")]
    [ProducesResponseType(typeof(ApiResponse<UserConfig>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetUserConfig([FromBody]GetUserConfigQuery query)
    {
        return Ok(await _mediator.GetApiResponse<UserConfig>(query));
    }

    [HttpPost]
    // [Authorize(Policy = "User")]
    [ProducesResponseType(typeof(ApiResponse<ResultSet<UserConfig>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SearchUserConfigs([FromBody]SearchUserConfigsQuery query)
    {
        return Ok(await _mediator.GetApiResponse<ResultSet<UserConfig>>(query));
    }

    [HttpPost]
    // [Authorize(Policy = "User")]
    [ProducesResponseType(typeof(ApiResponse<List<UserConfig>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetUserConfigList([FromBody]GetUserConfigListQuery query)
    {
        return Ok(await _mediator.GetApiResponse<List<UserConfig>>(query));
    }
}