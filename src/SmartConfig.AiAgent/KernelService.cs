using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SmartConfig.AiAgent;

public interface IKernelService
{
    IAsyncEnumerable<string> CompleteChatStreamingAsync(IEnumerable<ChatMessageContent> messages);
}

public class KernelService(Kernel kernel, IConfiguration configuration) : IKernelService
{
    public async IAsyncEnumerable<string> CompleteChatStreamingAsync(IEnumerable<ChatMessageContent> messages)
    {
        // 1️⃣ Add system instructions for the AI
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

        // 2️⃣ Add the user messages
        history.AddRange(messages);

        // 3️⃣ Get the chat completion service & settings
        var service = kernel.GetRequiredService<IChatCompletionService>();
        var settings = new PromptExecutionSettings
        {
            ServiceId = configuration["SemanticKernel:ServiceId"]!,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        // 4️⃣ Stream the AI response
        var result = service.GetStreamingChatMessageContentsAsync(history, settings, kernel);
        await foreach (var text in result)
        {
            yield return text.ToString();
        }
    }
}