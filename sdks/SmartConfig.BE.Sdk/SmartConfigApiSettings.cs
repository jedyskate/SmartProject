namespace SmartConfig.BE.Sdk;

public class SmartConfigApiSettings
{
    public string SmartConfigApiEndpoint { get; set; } = "https://localhost:5111/";
    public string ApplicationName { get; set; }
    public bool DryRun { get; set; } = false;
}