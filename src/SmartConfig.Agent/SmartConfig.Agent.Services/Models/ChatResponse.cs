namespace SmartConfig.Agent.Services.Models;

public class ChatResponse(string? content)
{
    public string? Content { get; set; } = content;
}