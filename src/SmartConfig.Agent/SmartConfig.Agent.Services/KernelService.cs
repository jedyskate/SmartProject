using Microsoft.SemanticKernel;
using SmartConfig.Agent.Services.Agents;

namespace SmartConfig.Agent.Services;

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