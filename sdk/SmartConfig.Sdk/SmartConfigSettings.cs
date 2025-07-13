namespace SmartConfig.Sdk;

public class SmartConfigSettings
{
    public string SmartConfigApiEndpoint { get; set; } = "https://localhost:5111/";
    public string ApplicationName { get; set; }
    public bool DryRun { get; set; } = false;
}