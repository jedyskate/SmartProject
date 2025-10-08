using System.Runtime.CompilerServices;
using MediatR;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
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
        private readonly IKernelService _kernelService;

        public Handler(IKernelService kernelService)
        {
            _kernelService = kernelService;
        }

        public async IAsyncEnumerable<ChatResponse> Handle(CompleteChatCommand request,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            // Map incoming messages to SK ChatMessageContent
            var messages = new List<ChatMessageContent>();
            foreach (var msg in request.Messages)
            {
                ChatMessageContent message = msg.Role switch
                {
                    RoleType.System    => new ChatMessageContent(AuthorRole.System, msg.Content),
                    RoleType.User      => new ChatMessageContent(AuthorRole.User, msg.Content),
                    RoleType.Assistant => new ChatMessageContent(AuthorRole.Assistant, msg.Content),
                    RoleType.Tool      => new ChatMessageContent(AuthorRole.Tool, msg.Content),
                    _ => throw new ArgumentException($"Invalid role: {msg.Role}")
                };
                messages.Add(message);
            }

            // Stream responses from KernelService
            await foreach (var chunk in _kernelService.CompleteChatStreamingAsync(messages)
                               .WithCancellation(cancellationToken))
            {
                yield return new ChatResponse(chunk);
            }
        }
    }
}