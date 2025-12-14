using System.Diagnostics;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using OllamaSharp;
using SmartConfig.Agent.Services.Models;
using SmartConfig.Agent.Services.Telemetry;
using ChatMessage = SmartConfig.Agent.Services.Models.ChatMessage;

namespace SmartConfig.Agent.Services.Agents.Workers;

public interface IWorkerAgent
{
    string Name { get; }
    string Description { get; }
    IAsyncEnumerable<string> ExecuteAsync(IEnumerable<ChatMessage> messages);
}

public class GeneralPurposeAgent(IOllamaApiClient ollamaApiClient, ILogger<GeneralPurposeAgent> logger) : IWorkerAgent
{
    public string Name => "GeneralPurposeAgent";
    public string Description => "Provides a general AI response to the user's question.";

    public async IAsyncEnumerable<string> ExecuteAsync(IEnumerable<ChatMessage> messages)
    {
        using var activity = AgentTelemetry.ActivitySource.StartActivity($"Agent.{Name}");
        var overallStopwatch = Stopwatch.StartNew();
        var timeToFirstToken = Stopwatch.StartNew();

        activity?.SetTag("agent.name", Name);
        activity?.SetTag("llm.provider", "ollama");

        AgentTelemetry.AgentInvocationCounter.Add(1,
            new KeyValuePair<string, object?>("agent", Name));

        logger.LogInformation("Agent {AgentName} executing request", Name);

        var history = new List<ChatMessage>
        {
            new(
                RoleType.System,
                "You are a helpful AI assistant. Answer the user's question."
            )
        };
        history.AddRange(messages);

        var agent = new ChatClientAgent((IChatClient)ollamaApiClient,
            new ChatClientAgentOptions
            {
                Name = nameof(GeneralPurposeAgent),
                Instructions = "You are a helpful AI assistant."
            });
        var prompt = string.Join("\n", history.Select(m => $"{m.Role}: {m.Content}"));

        long chunkCount = 0;
        bool isFirstToken = true;

        await foreach (var response in agent.RunStreamingAsync(prompt))
        {
            if (isFirstToken)
            {
                timeToFirstToken.Stop();
                AgentTelemetry.TimeToFirstTokenHistogram.Record(timeToFirstToken.Elapsed.TotalMilliseconds,
                    new KeyValuePair<string, object?>("agent", Name),
                    new KeyValuePair<string, object?>("provider", "ollama"));

                logger.LogInformation("Agent {AgentName} received first token in {ElapsedMs}ms",
                    Name, timeToFirstToken.Elapsed.TotalMilliseconds);
                isFirstToken = false;
            }

            chunkCount++;
            AgentTelemetry.StreamingChunksCounter.Add(1,
                new KeyValuePair<string, object?>("agent", Name),
                new KeyValuePair<string, object?>("provider", "ollama"));

            yield return response.Text;
        }

        overallStopwatch.Stop();

        AgentTelemetry.AgentDurationHistogram.Record(overallStopwatch.Elapsed.TotalMilliseconds,
            new KeyValuePair<string, object?>("agent", Name));

        AgentTelemetry.StreamingDurationHistogram.Record(overallStopwatch.Elapsed.TotalMilliseconds,
            new KeyValuePair<string, object?>("agent", Name),
            new KeyValuePair<string, object?>("provider", "ollama"));

        logger.LogInformation("Agent {AgentName} completed in {ElapsedMs}ms with {ChunkCount} chunks",
            Name, overallStopwatch.Elapsed.TotalMilliseconds, chunkCount);
    }
}