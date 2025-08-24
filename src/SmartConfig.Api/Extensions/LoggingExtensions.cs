namespace SmartConfig.Api.Extensions;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddConfigureLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        builder.Logging.AddConsole();

        return builder;
    }
}