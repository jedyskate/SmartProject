using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;
using SmartConfig.Agent.Services.Models;
using ChatMessage = SmartConfig.Agent.Services.Models.ChatMessage;

namespace SmartConfig.Agent.Services.Agents.Workers;

public class JokeAgent(IOllamaApiClient ollamaApiClient) : IWorkerAgent
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
        
        var agent = new ChatClientAgent((IChatClient)ollamaApiClient,
            new ChatClientAgentOptions
            {
                Name = nameof(JokeAgent),
                Instructions = "You are an AI assistant that tell jokes."
            });
        
        var prompt = string.Join("\n", history.Select(m => $"{m.Role}: {m.Content}"));

        await foreach (var response in agent.RunStreamingAsync(prompt))
        {
            yield return response.Text;
        }
    }
}