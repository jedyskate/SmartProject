using Microsoft.Agents.AI;
using SmartConfig.AiAgent.Models;
using ChatMessage = SmartConfig.AiAgent.Models.ChatMessage;

namespace SmartConfig.AiAgent.Agents.Workers;

public class JokeAgent(AIAgent agent) : IWorkerAgent
{
    public string Name => "JokeAgent";
    public string Description => "Tells a short, funny joke.";

    public async IAsyncEnumerable<string> ExecuteAsync(IEnumerable<ChatMessage> messages)
    {
        var history = new List<ChatMessage>
        {
            new(
                RoleType.System,
                "You are a comedian AI. Tell a short, funny joke."
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