using SmartConfig.AiAgent.Agents;
using SmartConfig.AiAgent.Models;

namespace SmartConfig.AiAgent;

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