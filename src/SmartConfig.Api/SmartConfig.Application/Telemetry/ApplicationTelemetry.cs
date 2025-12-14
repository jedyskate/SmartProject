using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace SmartConfig.Application.Telemetry;

/// <summary>
/// Central telemetry configuration for SmartConfig Application layer.
/// Provides ActivitySource for distributed tracing and Meter for custom metrics.
/// </summary>
public static class ApplicationTelemetry
{
    /// <summary>
    /// Application name used for all telemetry
    /// </summary>
    public const string ServiceName = "SmartConfig.Application";

    /// <summary>
    /// ActivitySource for creating custom spans and distributed traces
    /// </summary>
    public static readonly ActivitySource ActivitySource = new(ServiceName, "1.0.0");

    /// <summary>
    /// Meter for creating custom metrics
    /// </summary>
    public static readonly Meter Meter = new(ServiceName, "1.0.0");

    // Custom Counters
    public static readonly Counter<long> CommandExecutionCounter = Meter.CreateCounter<long>(
        "smartconfig.commands.executed",
        unit: "{command}",
        description: "Total number of commands executed");

    public static readonly Counter<long> QueryExecutionCounter = Meter.CreateCounter<long>(
        "smartconfig.queries.executed",
        unit: "{query}",
        description: "Total number of queries executed");

    public static readonly Counter<long> CommandFailureCounter = Meter.CreateCounter<long>(
        "smartconfig.commands.failed",
        unit: "{command}",
        description: "Total number of commands that failed");

    public static readonly Counter<long> QueryFailureCounter = Meter.CreateCounter<long>(
        "smartconfig.queries.failed",
        unit: "{query}",
        description: "Total number of queries that failed");

    // Custom Histograms
    public static readonly Histogram<double> CommandDurationHistogram = Meter.CreateHistogram<double>(
        "smartconfig.commands.duration",
        unit: "ms",
        description: "Duration of command execution in milliseconds");

    public static readonly Histogram<double> QueryDurationHistogram = Meter.CreateHistogram<double>(
        "smartconfig.queries.duration",
        unit: "ms",
        description: "Duration of query execution in milliseconds");

    public static readonly Histogram<long> ValidationErrorsHistogram = Meter.CreateHistogram<long>(
        "smartconfig.validation.errors",
        unit: "{error}",
        description: "Number of validation errors per request");

    // RabbitMQ Metrics
    public static readonly Counter<long> MessagesPublishedCounter = Meter.CreateCounter<long>(
        "smartconfig.rabbitmq.messages.published",
        unit: "{message}",
        description: "Total number of messages published to RabbitMQ");

    public static readonly Counter<long> MessagesPublishFailedCounter = Meter.CreateCounter<long>(
        "smartconfig.rabbitmq.messages.publish_failed",
        unit: "{message}",
        description: "Total number of failed message publishes to RabbitMQ");

    public static readonly Histogram<double> MessagePublishDurationHistogram = Meter.CreateHistogram<double>(
        "smartconfig.rabbitmq.publish.duration",
        unit: "ms",
        description: "Duration of message publish operations in milliseconds");

    // Orleans Metrics
    public static readonly Counter<long> GrainCallCounter = Meter.CreateCounter<long>(
        "smartconfig.orleans.grain.calls",
        unit: "{call}",
        description: "Total number of grain calls");

    public static readonly Counter<long> GrainCallFailureCounter = Meter.CreateCounter<long>(
        "smartconfig.orleans.grain.calls.failed",
        unit: "{call}",
        description: "Total number of failed grain calls");

    public static readonly Histogram<double> GrainCallDurationHistogram = Meter.CreateHistogram<double>(
        "smartconfig.orleans.grain.calls.duration",
        unit: "ms",
        description: "Duration of grain calls in milliseconds");
}
