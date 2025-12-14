using System.Diagnostics;
using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using SmartConfig.Agent.Services.Agents.Models;
using SmartConfig.Agent.Services.Agents.Workers;
using SmartConfig.Agent.Services.Models;
using SmartConfig.Agent.Services.Telemetry;
using ChatMessage = SmartConfig.Agent.Services.Models.ChatMessage;

namespace SmartConfig.Agent.Services.Agents;

public class OrchestratorAgent(IEnumerable<IWorkerAgent> agents, OpenAIClient openAiClient, IConfiguration configuration, ILogger<OrchestratorAgent> logger)
{
    private readonly List<IWorkerAgent> _agents = agents.ToList();

    public async IAsyncEnumerable<string> RouteAsync(IEnumerable<ChatMessage> messages)
    {
        using var activity = AgentTelemetry.ActivitySource.StartActivity("OrchestratorAgent.RouteAsync");
        var stopwatch = Stopwatch.StartNew();

        AgentTelemetry.OrchestratorRequestCounter.Add(1);

        var lastUserMessage = messages.LastOrDefault(m => m.Role == RoleType.User);
        activity?.SetTag("user.message", lastUserMessage?.Content);

        logger.LogInformation("Orchestrator routing request: {UserMessage}", lastUserMessage?.Content);

        var plan = await GetPlanAsync(lastUserMessage);

        if (plan?.Steps.Any() == true)
        {
            var executedSteps = new List<Step>();
            logger.LogInformation("Executing plan with {StepCount} steps", plan.Steps.Count);
            activity?.SetTag("plan.step_count", plan.Steps.Count);
            AgentTelemetry.WorkerAgentsInvokedHistogram.Record(plan.Steps.Count);

            foreach (var step in plan.Steps)
            {
                var agent = _agents.FirstOrDefault(a => a.Name == step.Agent);
                if (agent == null)
                {
                    logger.LogWarning("Agent not found: {AgentName}", step.Agent);
                    continue;
                }

                logger.LogInformation("Invoking worker agent: {AgentName} with request: {Request}", step.Agent, step.Request);

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
                stopwatch.Stop();
                AgentTelemetry.OrchestratorDurationHistogram.Record(stopwatch.Elapsed.TotalMilliseconds);
                logger.LogInformation("Orchestrator completed successfully in {ElapsedMs}ms", stopwatch.Elapsed.TotalMilliseconds);
                yield break;
            }
        }

        // Fallback to default agent if no plan or plan failed
        logger.LogInformation("Falling back to GeneralPurposeAgent");
        var defaultAgent = _agents.FirstOrDefault(a => a.Name == "GeneralPurposeAgent");
        if (defaultAgent != null)
        {
            AgentTelemetry.WorkerAgentsInvokedHistogram.Record(1);
            await foreach (var response in defaultAgent.ExecuteAsync(messages))
            {
                yield return response;
            }
        }

        stopwatch.Stop();
        AgentTelemetry.OrchestratorDurationHistogram.Record(stopwatch.Elapsed.TotalMilliseconds);
        logger.LogInformation("Orchestrator completed in {ElapsedMs}ms", stopwatch.Elapsed.TotalMilliseconds);
    }

    private async Task<Plan?> GetPlanAsync(ChatMessage? message)
    {
        if (message == null) return null;

        using var activity = AgentTelemetry.ActivitySource.StartActivity("OrchestratorAgent.GetPlan");
        var stopwatch = Stopwatch.StartNew();

        var modelName = configuration["Agent:OpenRouter:Model"] ?? "unknown";
        activity?.SetTag("llm.model", modelName);
        activity?.SetTag("llm.operation", "planning");

        AgentTelemetry.LlmRequestCounter.Add(1,
            new KeyValuePair<string, object?>("model", modelName),
            new KeyValuePair<string, object?>("operation", "planning"));

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
                          - request: The SPECIFIC PART of the user's request that this agent should handle

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

        try
        {
            var client = openAiClient.GetChatClient(modelName).AsIChatClient();
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

            stopwatch.Stop();
            AgentTelemetry.LlmDurationHistogram.Record(stopwatch.Elapsed.TotalMilliseconds,
                new KeyValuePair<string, object?>("model", modelName),
                new KeyValuePair<string, object?>("operation", "planning"));

            logger.LogInformation("LLM planning request completed in {ElapsedMs}ms using model {Model}",
                stopwatch.Elapsed.TotalMilliseconds, modelName);

            var plan = JsonSerializer.Deserialize<Plan>(result.Text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (plan != null)
            {
                logger.LogInformation("Plan created with {StepCount} steps: {Steps}",
                    plan.Steps.Count,
                    string.Join(", ", plan.Steps.Select(s => s.Agent)));
            }

            return plan;
        }
        catch (JsonException ex)
        {
            stopwatch.Stop();
            AgentTelemetry.LlmFailureCounter.Add(1,
                new KeyValuePair<string, object?>("model", modelName),
                new KeyValuePair<string, object?>("error", "JsonException"));

            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            logger.LogError(ex, "Failed to deserialize plan from LLM response");
            return null;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            AgentTelemetry.LlmFailureCounter.Add(1,
                new KeyValuePair<string, object?>("model", modelName),
                new KeyValuePair<string, object?>("error", ex.GetType().Name));

            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            logger.LogError(ex, "Failed to get plan from LLM");
            return null;
        }
    }
}