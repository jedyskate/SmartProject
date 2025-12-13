using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SmartConfig.Silo.Extensions;

namespace SmartConfig.Application.Extensions;

public static class ApplicationExtensions
{
    public static WebApplicationBuilder AddApplication(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR();
        builder.Services.AddHttpClient();

        builder.Services.AddOrleansSiloIoc(builder.Configuration);
        
        return builder;
    }
}