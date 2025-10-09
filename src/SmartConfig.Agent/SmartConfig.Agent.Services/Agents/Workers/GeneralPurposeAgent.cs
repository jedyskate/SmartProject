using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;
using SmartConfig.Agent.Services.Models;
using ChatMessage = SmartConfig.Agent.Services.Models.ChatMessage;

namespace SmartConfig.Agent.Services.Agents.Workers;

public interface IWorkerAgent
{
    string Name { get; }
    string Description { get; }
    IAsyncEnumerable<string> ExecuteAsync(IEnumerable<ChatMessage> messages);
}

public class GeneralPurposeAgent(IOllamaApiClient ollamaApiClient) : IWorkerAgent
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
        
        var agent = new ChatClientAgent((IChatClient)ollamaApiClient,
            new ChatClientAgentOptions
            {
                Name = nameof(GeneralPurposeAgent),
                Instructions = "You are a helpful AI assistant."
            });
        var prompt = string.Join("\n", history.Select(m => $"{m.Role}: {m.Content}"));

        await foreach (var response in agent.RunStreamingAsync(prompt))
        {
            yield return response.Text;
        }
    }
}