using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SmartConfig.AiAgent.Agents.Models;
using SmartConfig.AiAgent.Agents.Workers;

namespace SmartConfig.AiAgent.Agents;

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
        var lastUserMessage = messages.LastOrDefault(m => m.Role == AuthorRole.User);

        var plan = await GetPlanAsync(lastUserMessage);
        if (plan?.Steps.Any() == true)
        {
            var executedSteps = new List<Step>();
            foreach (var step in plan.Steps)
            {
                var agent = _agents.FirstOrDefault(a => a.Name == step.Agent);
                if (agent == null) continue;
                
                // Replacing User Message for Orchestrator Subtask
                var orchestratedMessages = messages.Where(m => m != lastUserMessage).ToList();
                orchestratedMessages.Add(new ChatMessageContent(AuthorRole.User, step.Request));
                
                // Stream agent output
                await foreach (var response in agent.ExecuteAsync(orchestratedMessages))
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

    private async Task<Plan?> GetPlanAsync(ChatMessageContent? message)
    {
        if  (message == null) return null;
        
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