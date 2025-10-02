using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SmartConfig.AiAgent.Agents.Orchestrator;

public class OrchestratorAgent
{
    private readonly List<IWorkerAgent> _agents;
    private readonly Kernel _kernel;

    public OrchestratorAgent(IEnumerable<IWorkerAgent> agents, Kernel kernel)
    {
        _agents = agents.ToList();
        _kernel = kernel;
    }

    public async IAsyncEnumerable<string> RouteAsync(IEnumerable<ChatMessageContent> messages)
    {
        var lastUserMessage = messages.LastOrDefault(m => m.Role == AuthorRole.User)?.Content ?? string.Empty;

        var plan = await GetPlanAsync(lastUserMessage);
        if (plan?.Steps.Any() == true)
        {
            var executedSteps = new List<string>();
            foreach (var step in plan.Steps)
            {
                var agent = _agents.FirstOrDefault(a => a.Name == step);
                if (agent == null) continue;
                await foreach (var response in agent.ExecuteAsync(messages))
                {
                    yield return response;
                }
                executedSteps.Add(step);
            }

            // This is a simplification. A real implementation should handle unexecuted steps.
            if (executedSteps.Count == plan.Steps.Count)
            {
                yield break;
            }
        }

        // Fallback to default agent if no plan or plan failed
        var defaultAgent = _agents.FirstOrDefault(a => a.Name == "GeneralPurposeAgent");
        if (defaultAgent != null)
        {
            await foreach (var response in defaultAgent.ExecuteAsync(messages))
            {
                yield return response;
            }
        }
    }

    private async Task<Plan?> GetPlanAsync(string input)
    {
        var agentList = string.Join("\n", _agents.Select(a => $"- {a.Name}: {a.Description}"));
        
        var jsonResponse = @"{""steps"": [""AgentName_1"", ""AgentName_2""]}";
        var prompt = $"""
            You are an orchestrator AI. Your job is to create a plan to answer the user's request.
            You have a list of available agents and their descriptions.
            Based on the user's request, create a JSON plan with a list of steps, where each step is the name of an agent to execute.
            The agents will be executed in the order they appear in the list.
            
            Rules:
            - Select agents only from the "Available agents" list below, using their names.
            - If the user's request matches multiple relevant agents, include all of them in the correct logical order.
            - ONLY add "GeneralPurposeAgent" when there is NOT other agent to take the request.
            - Respond with ONLY valid JSON. No explanations, no extra text, no markdown.

            Available agents:
            {agentList}

            User's request:
            {input}

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

    private class Plan
    {
        public List<string> Steps { get; set; } = new();
    }
}