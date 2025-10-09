using System.Runtime.CompilerServices;
using MediatR;
using SmartConfig.Agent.Services;
using SmartConfig.Agent.Services.Models;

namespace SmartConfig.Agent.Endpoints.Commands;

public class CompleteChatCommand : IStreamRequest<ChatResponse>
{
    #region Attributes
    
    public IEnumerable<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

    #endregion

    public class Handler : IStreamRequestHandler<CompleteChatCommand, ChatResponse>
    {
        private readonly IAgentService _agentService;

        public Handler(IAgentService agentService)
        {
            _agentService = agentService;
        }

        public async IAsyncEnumerable<ChatResponse> Handle(CompleteChatCommand request,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            // Map incoming messages to SK ChatMessageContent
            var messages = new List<ChatMessage>();
            foreach (var msg in request.Messages)
            {
                var message = msg.Role switch
                {
                    RoleType.System    => new ChatMessage(RoleType.System, msg.Content),
                    RoleType.User      => new ChatMessage(RoleType.User, msg.Content),
                    RoleType.Assistant => new ChatMessage(RoleType.Assistant, msg.Content),
                    RoleType.Tool      => new ChatMessage(RoleType.Tool, msg.Content),
                    _ => throw new ArgumentException($"Invalid role: {msg.Role}")
                };
                messages.Add(message);
            }

            // Stream responses from AgentService
            await foreach (var chunk in _agentService.CompleteChatStreamingAsync(messages)
                               .WithCancellation(cancellationToken))
            {
                yield return new ChatResponse(chunk);
            }
        }
    }
}