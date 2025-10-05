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
        await foreach (var response in orchestratorAgent.RouteAsync(messages))
        {
            yield return response;
        }
    }
}