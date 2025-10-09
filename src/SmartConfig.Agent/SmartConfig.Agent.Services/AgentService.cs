using SmartConfig.Agent.Services.Agents;
using SmartConfig.Agent.Services.Models;

namespace SmartConfig.Agent.Services;

public interface IAgentService
{
    IAsyncEnumerable<string> CompleteChatStreamingAsync(IEnumerable<ChatMessage> messages);
}

public class AgentService(OrchestratorAgent orchestratorAgent) : IAgentService
{
    public async IAsyncEnumerable<string> CompleteChatStreamingAsync(IEnumerable<ChatMessage> messages)
    {
        await foreach (var response in orchestratorAgent.RouteAsync(messages))
        {
            yield return response;
        }
    }
}