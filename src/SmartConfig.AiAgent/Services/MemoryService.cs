using System.Text.Json;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Memory;
using Npgsql;
using Pgvector;
using Pgvector.Npgsql;
using SmartConfig.AiAgent.Agents.Models;

namespace SmartConfig.AiAgent.Services;

public class MemoryService : IMemoryService
{
    private readonly string _connectionString;

    public MemoryService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        AppContext.SetSwitch("Npgsql.EnableSqlMapper", true);
    }

    public async Task SaveSessionAsync(Session session)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        var sql = @"
            INSERT INTO sessions (id, user_id, data, created_at, updated_at)
            VALUES (@Id, @UserId, @Data::jsonb, @CreatedAt, @UpdatedAt)
            ON CONFLICT (id) DO UPDATE SET
                data = @Data::jsonb,
                updated_at = now();";
        await connection.ExecuteAsync(sql, new { session.Id, session.UserId, Data = JsonSerializer.Serialize(session.Data), session.CreatedAt, session.UpdatedAt });
    }

    public async Task<Session?> GetSessionAsync(Guid sessionId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        var sql = "SELECT id, user_id, data, created_at, updated_at FROM sessions WHERE id = @sessionId";
        return await connection.QuerySingleOrDefaultAsync<Session>(sql, new { sessionId });
    }

    public async Task SaveMemoryAsync(Guid sessionId, string content, IReadOnlyList<float> embedding)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        var sql = @"
            INSERT INTO memory (id, session_id, content, embedding)
            VALUES (@Id, @SessionId, @Content, @Embedding);";
        await connection.ExecuteAsync(sql, new { Id = Guid.NewGuid(), SessionId = sessionId, Content = content, Embedding = new Vector(embedding.ToArray()) });
    }

    public async Task<List<MemoryItem>> QueryMemoryAsync(Guid sessionId, IReadOnlyList<float> embedding, int limit = 5)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        var sql = @"
            SELECT id, session_id, content, embedding, created_at
            FROM memory
            WHERE session_id = @sessionId
            ORDER BY embedding <-> @queryEmbedding
            LIMIT @limit;";
        var result = await connection.QueryAsync<MemoryItem>(sql, new { sessionId, queryEmbedding = new Vector(embedding.ToArray()), limit });
        return result.ToList();
    }
}
