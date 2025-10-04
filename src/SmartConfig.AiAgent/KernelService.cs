using Microsoft.SemanticKernel;
using SmartConfig.AiAgent.Agents;

namespace SmartConfig.AiAgent;

public interface IKernelService
{
    IAsyncEnumerable<string> CompleteChatStreamingAsync(IEnumerable<ChatMessageContent> messages);
}

public class KernelService(OrchestratorAgent orchestratorAgent) : IKernelService
{
    public async IAsyncEnumerable<string> CompleteChatStreamingAsync(IEnumerable<ChatMessageContent> messages)
    {
        // TODO: Get the user id from the authenticated user
        var userId = "user";
        var sessionId = Guid.NewGuid();
        await foreach (var response in orchestratorAgent.RouteAsync(sessionId, userId, messages))
        {
            yield return response;
        }
    }
}