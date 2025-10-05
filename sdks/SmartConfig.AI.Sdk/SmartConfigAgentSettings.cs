namespace SmartConfig.AI.Sdk;

public class SmartConfigAgentSettings
{
    public string SmartConfigApiEndpoint { get; set; } = "https://localhost:7153/";
    public string ApplicationName { get; set; }
    public bool DryRun { get; set; } = false;
}