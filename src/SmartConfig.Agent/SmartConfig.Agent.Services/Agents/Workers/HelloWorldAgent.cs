using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;
using SmartConfig.Agent.Services.Models;
using SmartConfig.Agent.Services.Tools;
using ChatMessage = SmartConfig.Agent.Services.Models.ChatMessage;

namespace SmartConfig.Agent.Services.Agents.Workers;

public class HelloWorldAgent(IOllamaApiClient ollamaApiClient, HelloWorldTool helloWorldTool) : IWorkerAgent
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
                When the user asks you to greet someone by name, you MUST call the 'SayHelloAsync'.
                Return ONLY the response from the plugin. Do not generate any greetings yourself.
                Do not respond with anything else unless explicitly instructed by the user.
                """
            )
        };
        history.AddRange(messages);
        
        var agent = new ChatClientAgent((IChatClient)ollamaApiClient,
            new ChatClientAgentOptions
            {
                Name = nameof(HelloWorldAgent),
                Instructions = "You are an AI assistant that uses a hello tool.",
                ChatOptions = new ChatOptions
                {
                    Tools =
                    [
                        AIFunctionFactory.Create(helloWorldTool.SayHelloAsync)
                    ],
                    ToolMode = ChatToolMode.Auto,
                    ResponseFormat = ChatResponseFormat.Text
                }
            });
        
        var prompt = string.Join("\n", history.Select(m => $"{m.Role}: {m.Content}"));

        await foreach (var response in agent.RunStreamingAsync(prompt))
        {
            yield return response.Text;
        }
    }
}