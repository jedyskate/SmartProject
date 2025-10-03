using Microsoft.Agents.AI;
using SmartConfig.AiAgent.Models;

namespace SmartConfig.AiAgent.Agents.Workers;

public interface IWorkerAgent
{
    string Name { get; }
    string Description { get; }
    IAsyncEnumerable<string> ExecuteAsync(IEnumerable<ChatMessage> messages);
}

public class GeneralPurposeAgent(AIAgent agent) : IWorkerAgent
{
    public string Name => "GeneralPurposeAgent";
    public string Description => "Provides a general AI response to the user's question.";

    public async IAsyncEnumerable<string> ExecuteAsync(IEnumerable<ChatMessage> messages)
    {
        var history = new List<ChatMessage>
        {
            new(
                RoleType.System,
                "You are a helpful AI assistant. Answer the user's question."
            )
        };
        history.AddRange(messages);

        var prompt = string.Join("\n", history.Select(m => $"{m.Role}: {m.Content}"));
        
        await foreach (var response in agent.RunStreamingAsync(prompt))
        {
            yield return response.Text;
        }
    }
}