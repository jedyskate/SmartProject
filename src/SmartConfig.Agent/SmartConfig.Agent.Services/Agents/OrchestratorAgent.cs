using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using SmartConfig.Agent.Services.Agents.Models;
using SmartConfig.Agent.Services.Agents.Workers;
using SmartConfig.Agent.Services.Models;
using ChatMessage = SmartConfig.Agent.Services.Models.ChatMessage;

namespace SmartConfig.Agent.Services.Agents;

public class OrchestratorAgent(IEnumerable<IWorkerAgent> agents, OpenAIClient openAiClient, IConfiguration configuration)
{
    private readonly List<IWorkerAgent> _agents = agents.ToList();

    public async IAsyncEnumerable<string> RouteAsync(IEnumerable<ChatMessage> messages)
    {
        var lastUserMessage = messages.LastOrDefault(m => m.Role == RoleType.User);
        
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
                orchestratedMessages.Add(new ChatMessage(RoleType.User, step.Request));
                
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

    private async Task<Plan?> GetPlanAsync(ChatMessage? message)
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

        var client = openAiClient.GetChatClient(configuration["Agent:OpenRouter:Model"]).AsIChatClient();
        var agent = new ChatClientAgent(client,
            new ChatClientAgentOptions
            {
                Name = nameof(OrchestratorAgent),
                Instructions = "You are an orchestrator AI Agent",
                ChatOptions = new ChatOptions
                {
                    ResponseFormat = ChatResponseFormat.ForJsonSchema<Plan>()
                }
            });
        var result = await agent.RunAsync(prompt);

        try
        {
            return JsonSerializer.Deserialize<Plan>(result.Text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException)
        {
            return null;
        }
    }
}