using System;
using System.Collections.Generic;

namespace SmartConfig.AiAgent.Agents.Models;

public class MemoryItem
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string Content { get; set; }
    public IReadOnlyList<float> Embedding { get; set; }
    public DateTime CreatedAt { get; set; }
}
