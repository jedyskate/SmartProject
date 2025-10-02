using Microsoft.SemanticKernel;

namespace SmartConfig.AiAgent.Agents;

public interface IWorkerAgent
{
    string Name { get; }
    string Description { get; }
    IAsyncEnumerable<string> ExecuteAsync(IEnumerable<ChatMessageContent> messages);
}