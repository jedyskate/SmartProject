using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartConfig.AiAgent.Models;
using SmartConfig.Application.Application.AiAgent.Commands;

namespace SmartConfig.Api.Controllers;

public class AiAgentController : BaseController
{
    private readonly IMediator _mediator;

    public AiAgentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(IAsyncEnumerable<ChatResponse>), (int)HttpStatusCode.OK)]
    public IAsyncEnumerable<ChatResponse> CompleteChatStreaming([FromBody] CompleteChatCommand command)
    {
        return _mediator.CreateStream(command);
    }
}