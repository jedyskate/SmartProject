using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SmartConfig.AiAgent.Agents.Workers;

public class HelloWorldAgent(IConfiguration configuration, Kernel kernel) : IWorkerAgent
{
    public string Name => "HelloWorldAgent";
    public string Description => "Greets the user or a person by name.";

    public async IAsyncEnumerable<string> ExecuteAsync(IEnumerable<ChatMessageContent> messages)
    {
        var history = new ChatHistory
        {
            new ChatMessageContent(
                AuthorRole.System,
                """
                You are a helpful AI assistant. 
                When the user asks you to greet someone by name, you MUST call the 'say_hello' function in the 'HelloWorld' plugin.
                Return **only** the response from the plugin. Do not generate any greetings yourself.
                Do not respond with anything else unless explicitly instructed by the user.
                """
            )
        };
        history.AddRange(messages);

        var service = kernel.GetRequiredService<IChatCompletionService>();
        var settings = new PromptExecutionSettings
        {
            ServiceId = configuration["SemanticKernel:ServiceId"]!,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        var result = service.GetStreamingChatMessageContentsAsync(history, settings, kernel);
        await foreach (var text in result)
        {
            yield return text.ToString();
        }
    }
}