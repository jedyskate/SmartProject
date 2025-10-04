using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartConfig.AiAgent.Agents.Models;

namespace SmartConfig.AiAgent.Services;

public interface IMemoryService
{
    Task SaveSessionAsync(Session session);
    Task<Session?> GetSessionAsync(Guid sessionId);
    Task SaveMemoryAsync(Guid sessionId, string content, IReadOnlyList<float> embedding);
    Task<List<MemoryItem>> QueryMemoryAsync(Guid sessionId, IReadOnlyList<float> embedding, int limit = 5);
}
