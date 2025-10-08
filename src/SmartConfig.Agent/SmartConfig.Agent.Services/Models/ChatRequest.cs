namespace SmartConfig.Agent.Services.Models;

public class ChatMessage(RoleType role, string? content)
{
    public RoleType Role { get; set; } = role;
    public string? Content { get; set; } = content;
}
