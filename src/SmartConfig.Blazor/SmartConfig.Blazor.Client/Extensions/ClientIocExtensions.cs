namespace SmartConfig.Blazor.Client.Extensions;

public static class ClientIocExtensions
{
    public static IServiceCollection AddCommonClientIoc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("n8n", client =>
        {
            var n8nUrl = configuration["SmartConfig:n8nEndpoint"] ?? throw new ArgumentNullException();
            
            client.BaseAddress = new Uri(n8nUrl);
        });
        
        return services;
    }
}