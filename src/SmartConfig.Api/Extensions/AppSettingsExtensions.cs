using VaultSharp.Extensions.Configuration;
using VaultSharp.V1.AuthMethods.UserPass;

namespace SmartConfig.Api.Extensions;

public static class AppSettingsExtensions
{
    public static WebApplicationBuilder AddAppSettingConfigurations(this WebApplicationBuilder builder)
    {
        var environment = builder.Environment.EnvironmentName;

        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

#if DEBUG
        builder.Configuration.AddJsonFile("appsettings.Debug.json", optional: true, reloadOnChange: true);
#endif

        builder.Configuration.AddEnvironmentVariables();

        var configuration = builder.Configuration;

        var client = configuration["Vault:Client"];
        var secret = configuration["Vault:Secret"];

        if (!string.IsNullOrEmpty(client) && !string.IsNullOrEmpty(secret))
        {
            var authMethod = new UserPassAuthMethodInfo(client, secret);

            builder.Configuration.AddVaultConfiguration(() =>
                {
                    var options = new VaultOptions(
                        configuration["Vault:Url"],
                        authMethod: authMethod,
                        reloadOnChange: true,
                        reloadCheckIntervalSeconds: 60);

                    return options;
                }, "SmartConfig", $"API-{environment}");
        }

        return builder;
    }

    public static IServiceCollection AddAppSettingsIoc(this IServiceCollection services, IConfiguration configuration)
    {
        var client = configuration["Vault:Client"];
        var secret = configuration["Vault:Secret"];

        if (!string.IsNullOrEmpty(client) && !string.IsNullOrEmpty(secret))
        {
            services.AddHostedService<VaultChangeWatcher>();
        }

        return services;
    }
}