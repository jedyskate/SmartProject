using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Embeddings;
using SmartConfig.AiAgent.Agents.Models;
using SmartConfig.AiAgent.Agents.Workers;
using SmartConfig.AiAgent.Services;

namespace SmartConfig.AiAgent.Agents;

public class OrchestratorAgent
{
    private readonly List<IWorkerAgent> _agents;
    private readonly Kernel _kernel;
    private readonly IMemoryService _memoryService;

    public OrchestratorAgent(IEnumerable<IWorkerAgent> agents, Kernel kernel, IMemoryService memoryService)
    {
        _agents = agents.ToList();
        _kernel = kernel;
        _memoryService = memoryService;
        // Removed: Initialization of _embeddingService
    }

    public async IAsyncEnumerable<string> RouteAsync(Guid sessionId, string userId, IEnumerable<ChatMessageContent> messages)
    {
        var session = await _memoryService.GetSessionAsync(sessionId) ?? new Session { Id = sessionId, UserId = userId };
        await _memoryService.SaveSessionAsync(session);

        var messagesList = messages.ToList();
        var lastUserMessage = messagesList.LastOrDefault(m => m.Role == AuthorRole.User);

        if (lastUserMessage?.Content == null)
        {
            yield break;
        }

        // FIX: Use the Kernel extension method for embedding generation
        var embeddingService = _kernel.GetRequiredService<ITextEmbeddingGenerationService>();
        var userMessageEmbedding = await embeddingService.GenerateEmbeddingAsync(lastUserMessage.Content);
        var relevantHistory = await _memoryService.QueryMemoryAsync(sessionId, userMessageEmbedding.ToArray());

        var historyMessages = relevantHistory
            .Select(m => new ChatMessageContent(AuthorRole.User, m.Content)) // This is a simplification.
            .ToList();

        var messagesWithHistory = historyMessages.Concat(messagesList).ToList();

        var plan = await GetPlanAsync(lastUserMessage);
        if (plan?.Steps.Any() == true)
        {
            var executedSteps = new List<Step>();
            foreach (var step in plan.Steps)
            {
                var agent = _agents.FirstOrDefault(a => a.Name == step.Agent);
                if (agent == null) continue;

                var orchestratedMessages = messagesWithHistory.Where(m => m != lastUserMessage).ToList();
                orchestratedMessages.Add(new ChatMessageContent(AuthorRole.User, step.Request));

                var agentResponse = "";
                await foreach (var response in agent.ExecuteAsync(orchestratedMessages))
                {
                    agentResponse += response;
                    yield return response;
                }

                await _memoryService.SaveMemoryAsync(sessionId, lastUserMessage.Content, userMessageEmbedding.ToArray());
                // FIX: Use the Kernel extension method for embedding generation
                var agentResponseEmbedding = await embeddingService.GenerateEmbeddingAsync(agentResponse);
                await _memoryService.SaveMemoryAsync(sessionId, agentResponse, agentResponseEmbedding.ToArray());

                executedSteps.Add(step);
            }

            if (executedSteps.Count == plan.Steps.Count)
            {
                yield break;
            }
        }

        var defaultAgent = _agents.FirstOrDefault(a => a.Name == "GeneralPurposeAgent");
        if (defaultAgent != null)
        {
            var agentResponse = "";
            await foreach (var response in defaultAgent.ExecuteAsync(messagesWithHistory))
            {
                agentResponse += response;
                yield return response;
            }
            
            await _memoryService.SaveMemoryAsync(sessionId, lastUserMessage.Content, userMessageEmbedding.ToArray());
            // FIX: Use the Kernel extension method for embedding generation
            var agentResponseEmbedding = await embeddingService.GenerateEmbeddingAsync(agentResponse);
            await _memoryService.SaveMemoryAsync(sessionId, agentResponse, agentResponseEmbedding.ToArray());
        }
    }

    private async Task<Plan?> GetPlanAsync(ChatMessageContent? message)
    {
        if (message == null) return null;

        var agentList = string.Join("\n", _agents.Select(a => $"- {a.Name}: {a.Description}"));

        var jsonResponse = @"{
          ""steps"": [
            { ""agent"": ""AgentName"", ""request"": ""Subtask"" }
          ]
        }";

        var prompt = $"""
                          You are an orchestrator AI. Your job is to break down the user's request into a plan.
                          Each step in the plan contains:
                          - agent: The EXACT name of an "Available agents"
                          - request: The SPECIFIC PART of the user’s request that this agent should handle

                          Rules:
                          - Select agents only from the "Available agents" list below, using their EXACT names.
                          - If the user's request matches multiple relevant agents, include all of them in the correct logical order.
                          - ONLY add "GeneralPurposeAgent" when there is NOT other agent to take the request.
                          - Respond with ONLY valid JSON. No explanations, no extra text, no markdown.

                          Available agents:
                          {agentList}

                          User's request:
                          {message.Content}

                          Expected response format:
                          {jsonResponse}
                      """;

        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        var result = await chatCompletionService.GetChatMessageContentAsync(prompt);

        try
        {
            return JsonSerializer.Deserialize<Plan>(result.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException)
        {
            return null;
        }
    }
}