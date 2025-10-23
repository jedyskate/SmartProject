namespace SmartConfig.App.Web.Extensions;

public static class YarpExtensions
{
    public static WebApplicationBuilder AddYarp(this WebApplicationBuilder builder)
    {
        builder.Services.AddReverseProxy()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
        
        return builder;
    }
}