using NUnit.Framework;
using SmartConfig.AI.Sdk;
using SmartConfig.BE.Sdk;

namespace SmartConfig.IntegrationTests.Infrastructure;

[SetUpFixture]
public class TestBase
{
    private static CustomWebApplicationFactory<Api.Program> _factoryApi;
    private static CustomWebApplicationFactory<Agent.Program> _factoryAgent;

    protected SmartConfigApiClient SmartConfigApiClient { get; }
    protected SmartConfigAgentClient SmartConfigAgentClient { get; }
    protected string AccessToken { get; } = "bearer eyJhbAAAXVCJ9.eyJjbAAA5In0.Ms_vYB2AAA_hPKWthgMPO8w";

    protected TestBase()
    {
        // API CLIENT
        _factoryApi = new CustomWebApplicationFactory<Api.Program>();

        var apiSettings = new SmartConfigApiSettings();
        var apiHttpClient = _factoryApi.CreateClient();
        apiHttpClient.BaseAddress = new Uri(apiSettings.SmartConfigApiEndpoint);
        apiHttpClient.DefaultRequestHeaders.Add("Authorization", AccessToken);

        SmartConfigApiClient = new SmartConfigApiClient(apiHttpClient);
        
        
        // AGENT CLIENT
        _factoryAgent = new CustomWebApplicationFactory<Agent.Program>();

        var agentSettings = new SmartConfigAgentSettings();
        var agentHttpClient = _factoryAgent.CreateClient();
        agentHttpClient.BaseAddress = new Uri(agentSettings.SmartConfigAgentEndpoint);

        SmartConfigAgentClient = new SmartConfigAgentClient(agentHttpClient);
    }
}