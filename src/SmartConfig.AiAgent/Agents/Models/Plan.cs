namespace SmartConfig.AiAgent.Agents.Models;

public class Plan
{
    public List<Step> Steps { get; set; } = new();
}

public class Step
{
    public string Agent { get; set; } = string.Empty;
    public string Request { get; set; } = string.Empty;
}