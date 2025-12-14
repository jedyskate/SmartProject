using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace SmartConfig.Agent.Services.Telemetry;

/// <summary>
/// Central telemetry configuration for AI/LLM operations in SmartConfig Agent.
/// Provides ActivitySource for distributed tracing and Meter for custom metrics.
/// </summary>
public static class AgentTelemetry
{
    /// <summary>
    /// Service name used for all telemetry
    /// </summary>
    public const string ServiceName = "SmartConfig.Agent";

    /// <summary>
    /// ActivitySource for creating custom spans and distributed traces
    /// </summary>
    public static readonly ActivitySource ActivitySource = new(ServiceName, "1.0.0");

    /// <summary>
    /// Meter for creating custom metrics
    /// </summary>
    public static readonly Meter Meter = new(ServiceName, "1.0.0");

    // Agent Execution Metrics
    public static readonly Counter<long> AgentInvocationCounter = Meter.CreateCounter<long>(
        "agent.invocations",
        unit: "{invocation}",
        description: "Total number of agent invocations");

    public static readonly Counter<long> AgentFailureCounter = Meter.CreateCounter<long>(
        "agent.failures",
        unit: "{failure}",
        description: "Total number of agent failures");

    public static readonly Histogram<double> AgentDurationHistogram = Meter.CreateHistogram<double>(
        "agent.duration",
        unit: "ms",
        description: "Duration of agent execution in milliseconds");

    // LLM Request Metrics
    public static readonly Counter<long> LlmRequestCounter = Meter.CreateCounter<long>(
        "llm.requests",
        unit: "{request}",
        description: "Total number of LLM API requests");

    public static readonly Counter<long> LlmFailureCounter = Meter.CreateCounter<long>(
        "llm.failures",
        unit: "{failure}",
        description: "Total number of LLM API failures");

    public static readonly Histogram<double> LlmDurationHistogram = Meter.CreateHistogram<double>(
        "llm.duration",
        unit: "ms",
        description: "Duration of LLM API calls in milliseconds");

    // Token Usage Metrics
    public static readonly Counter<long> TokensUsedCounter = Meter.CreateCounter<long>(
        "llm.tokens.used",
        unit: "{token}",
        description: "Total number of tokens used");

    public static readonly Counter<long> PromptTokensCounter = Meter.CreateCounter<long>(
        "llm.tokens.prompt",
        unit: "{token}",
        description: "Total number of prompt tokens");

    public static readonly Counter<long> CompletionTokensCounter = Meter.CreateCounter<long>(
        "llm.tokens.completion",
        unit: "{token}",
        description: "Total number of completion tokens");

    public static readonly Histogram<long> TokensPerRequestHistogram = Meter.CreateHistogram<long>(
        "llm.tokens.per_request",
        unit: "{token}",
        description: "Distribution of tokens per request");

    // Cost Tracking (estimated)
    public static readonly Histogram<double> EstimatedCostHistogram = Meter.CreateHistogram<double>(
        "llm.cost.estimated",
        unit: "USD",
        description: "Estimated cost per LLM request in USD");

    public static readonly Counter<double> TotalEstimatedCostCounter = Meter.CreateCounter<double>(
        "llm.cost.total",
        unit: "USD",
        description: "Total estimated cost of LLM usage in USD");

    // Tool Invocation Metrics
    public static readonly Counter<long> ToolInvocationCounter = Meter.CreateCounter<long>(
        "agent.tools.invocations",
        unit: "{invocation}",
        description: "Total number of tool invocations");

    public static readonly Counter<long> ToolFailureCounter = Meter.CreateCounter<long>(
        "agent.tools.failures",
        unit: "{failure}",
        description: "Total number of tool invocation failures");

    public static readonly Histogram<double> ToolDurationHistogram = Meter.CreateHistogram<double>(
        "agent.tools.duration",
        unit: "ms",
        description: "Duration of tool invocations in milliseconds");

    // Streaming Metrics
    public static readonly Counter<long> StreamingChunksCounter = Meter.CreateCounter<long>(
        "llm.streaming.chunks",
        unit: "{chunk}",
        description: "Total number of streaming response chunks");

    public static readonly Histogram<double> TimeToFirstTokenHistogram = Meter.CreateHistogram<double>(
        "llm.streaming.time_to_first_token",
        unit: "ms",
        description: "Time to first token in streaming responses (latency)");

    public static readonly Histogram<double> StreamingDurationHistogram = Meter.CreateHistogram<double>(
        "llm.streaming.total_duration",
        unit: "ms",
        description: "Total duration of streaming responses");

    // Orchestrator Metrics
    public static readonly Counter<long> OrchestratorRequestCounter = Meter.CreateCounter<long>(
        "orchestrator.requests",
        unit: "{request}",
        description: "Total number of orchestrator requests");

    public static readonly Histogram<double> OrchestratorDurationHistogram = Meter.CreateHistogram<double>(
        "orchestrator.duration",
        unit: "ms",
        description: "Duration of orchestrator execution in milliseconds");

    public static readonly Histogram<long> WorkerAgentsInvokedHistogram = Meter.CreateHistogram<long>(
        "orchestrator.workers_invoked",
        unit: "{worker}",
        description: "Number of worker agents invoked per orchestrator request");
}
