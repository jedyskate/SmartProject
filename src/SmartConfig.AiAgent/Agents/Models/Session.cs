using System;
using System.Text.Json.Nodes;

namespace SmartConfig.AiAgent.Agents.Models;

public class Session
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public JsonObject? Data { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
