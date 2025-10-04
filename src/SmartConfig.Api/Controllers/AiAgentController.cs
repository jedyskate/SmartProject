// using MediatR;
// using Microsoft.AspNetCore.Mvc;
// using SmartConfig.Agent.Application.Models;
// using SmartConfig.Application.Application.AiAgent.Commands;
//
// namespace SmartConfig.Api.Controllers;
//
// public class AiAgentController : BaseController
// {
//     private readonly IMediator _mediator;
//     private readonly ILogger<AiAgentController> _logger;
//
//     public AiAgentController(IMediator mediator, ILogger<AiAgentController> logger)
//     {
//         _mediator = mediator;
//         _logger = logger;
//     }
//     
//     [HttpPost]
//     [ProducesResponseType(typeof(ChatResponse), (int)HttpStatusCode.OK)]
//     public async Task CompleteChatStreaming([FromBody] CompleteChatCommand command)
//     {
//         Response.StatusCode = StatusCodes.Status200OK;
//         Response.ContentType = "application/json";
//         Response.Headers.Append("Cache-Control", "no-cache");
//         Response.Headers.Append("X-Content-Type-Options", "nosniff");
//         
//         var cancellationToken = HttpContext.RequestAborted;
//         try 
//         {
//             await foreach (var chunk in _mediator.CreateStream(command, cancellationToken))
//             {
//                 var json = System.Text.Json.JsonSerializer.Serialize(chunk);
//                 await Response.WriteAsync(json + "\n", cancellationToken: cancellationToken);
//                 await Response.Body.FlushAsync(cancellationToken);
//             }
//         }
//         catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
//         {
//             _logger.LogInformation("Streaming cancelled by client.");
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "An error occurred during chat streaming.");
//         }
//     }
// }