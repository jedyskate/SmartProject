using NUnit.Framework;
using SmartConfig.BE.Sdk;

namespace SmartConfig.IntegrationTests.Infrastructure;

[SetUpFixture]
public class TestBase
{
    private static CustomWebApplicationFactory<Api.Program> _factory;

    protected SmartConfigClient SmartConfigClient { get; }
    protected string AccessToken { get; } = "bearer eyJhbAAAXVCJ9.eyJjbAAA5In0.Ms_vYB2AAA_hPKWthgMPO8w";

    protected TestBase()
    {
        _factory = new CustomWebApplicationFactory<Api.Program>();

        var settings = new SmartConfigSettings();
        var httpClient = _factory.CreateClient();
        httpClient.BaseAddress = new Uri(settings.SmartConfigApiEndpoint);
        httpClient.DefaultRequestHeaders.Add("Authorization", AccessToken);

        SmartConfigClient = new SmartConfigClient(httpClient);
    }
}