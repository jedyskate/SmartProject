using Microsoft.Agents.AI;
using SmartConfig.AiAgent.Models;

namespace SmartConfig.AiAgent.Agents.Workers;

public class HelloWorldAgent(AIAgent agent) : IWorkerAgent
{
    public string Name => "HelloWorldAgent";
    public string Description => "Greets the user or a person by name.";

    public async IAsyncEnumerable<string> ExecuteAsync(IEnumerable<ChatMessage> messages)
    {
        var history = new List<ChatMessage>
        {
            new(
                RoleType.System,
                """
                You are a helpful AI assistant. 
                When the user asks you to greet someone by name, you MUST call the 'say_hello' function in the 'HelloWorld' plugin.
                Return **only** the response from the plugin. Do not generate any greetings yourself.
                Do not respond with anything else unless explicitly instructed by the user.
                """
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