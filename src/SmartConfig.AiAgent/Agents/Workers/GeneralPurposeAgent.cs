using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SmartConfig.AiAgent.Agents.Workers;

public interface IWorkerAgent
{
    string Name { get; }
    string Description { get; }
    IAsyncEnumerable<string> ExecuteAsync(IEnumerable<ChatMessageContent> messages);
}

public class GeneralPurposeAgent(Kernel kernel) : IWorkerAgent
{
    public string Name => "GeneralPurposeAgent";
    public string Description => "Provides a general AI response to the user's question.";

    public async IAsyncEnumerable<string> ExecuteAsync(IEnumerable<ChatMessageContent> messages)
    {
        var history = new ChatHistory
        {
            new ChatMessageContent(
                AuthorRole.System,
                "You are a helpful AI assistant. Answer the user's question."
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