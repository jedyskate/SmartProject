# SmartConfig MCP Server

This project is an **MCP Server** built with .NET. It consumes an external API and exposes data to multiple MCP clients.

## What it Does

- **API Integration**: Connects to the SmartConfig API and transforms responses for MCP clients.
- **Multi-client Support**: Can be used by several MCP clients at once.

## How to Run

Run the server with:

```bash
  dotnet run --project src/SmartConfig.Mcp/SmartConfig.McpServer.csproj
```

## MCP Client Setup

Point your MCP client to the server:

```json
{
  "mcpServers": {
    "SmartConfig-mcp": {
      "type": "sse",
      "url": "http://host.docker.internal:3011/sse"
    }
  }
}
```

## ⚠️ Note on Local Development

- **Use http:// (not https://)** when connecting locally.
- The .NET server may listen on both, e.g.:

```json
{
  "applicationUrl": "https://localhost:7133;http://localhost:3011"
}
```

## Resources

This video shows how to set up a simple MCP Server:
https://www.youtube.com/watch?v=f1ginn9853o&ab_channel=Balaone

How to invoke MCP Server in AnythingLLM:
https://docs.anythingllm.com/mcp-compatibility/desktop#startup-sequence
