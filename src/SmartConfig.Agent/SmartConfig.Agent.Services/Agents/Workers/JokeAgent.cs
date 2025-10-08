using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SmartConfig.Agent.Services.Agents.Workers;

public class JokeAgent(Kernel kernel) : IWorkerAgent
{
    public string Name => "JokeAgent";
    public string Description => "Tells a short, funny joke.";

    public async IAsyncEnumerable<string> ExecuteAsync(IEnumerable<ChatMessageContent> messages)
    {
        var history = new ChatHistory
        {
            new ChatMessageContent(
                AuthorRole.System,
                "You are a comedian AI. Tell a short, funny joke."
            )
        };
        history.AddRange(messages);

        var service = kernel.GetRequiredService<IChatCompletionService>();
        var result = service.GetStreamingChatMessageContentsAsync(history, null, kernel);

        await foreach (var text in result)
        {
            yield return text.ToString();
        }
    }
}