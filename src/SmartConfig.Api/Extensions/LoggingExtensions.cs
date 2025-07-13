using OpenTelemetry.Logs;

namespace SmartConfig.Api.Extensions;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddConfigureLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
            options.AddOtlpExporter();
        });

        builder.Logging.AddConsole();

        return builder;
    }
}