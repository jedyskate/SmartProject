# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Commands

### Running the Application

The entire stack runs via **.NET Aspire** orchestration:

```bash
dotnet run --project tools/SmartConfig.Host/SmartConfig.Host.csproj
```

This launches all services (databases, backend APIs, frontends, AI infrastructure) with a developer dashboard.

### Testing

Run unit tests:
```bash
dotnet test tests/SmartConfig.UnitTests
```

Run integration tests:
```bash
dotnet test tests/SmartConfig.IntegrationTests
```

Run a specific test:
```bash
dotnet test tests/SmartConfig.UnitTests --filter "TestMethodName"
```

### Database Migrations

Migrations are handled by `SmartConfig.Migration` and run automatically via Aspire on startup.

### Frontend Development

**Next.js:**
```bash
cd src/SmartConfig.NextJs
npm run dev
```

**React (Vite):**
```bash
cd src/SmartConfig.React
npm run dev
```

**Angular:**
```bash
cd src/SmartConfig.Angular
npm start
```

**Blazor:** Runs through Aspire host (no standalone command needed)

## Architecture Overview

### Clean Architecture Layers

The solution follows **Clean Architecture** with clear separation:

1. **SmartConfig.Core** - Domain entities and business rules
2. **SmartConfig.Application** - CQRS handlers using MediatR with pipeline behaviors:
   - `RequestLoggerBehaviour` - Logs all requests
   - `RequestPerformanceBehaviour` - Tracks performance
   - `RequestValidationBehavior` - Validates commands/queries using FluentValidation
3. **SmartConfig.Data** - EF Core persistence with SQL Server
4. **SmartConfig.Api** - REST API with controllers and extension methods for dependency injection

### Distributed State with Orleans

**SmartConfig.Silo** hosts Orleans grains for distributed, stateful computations:
- Grains are located in `src/SmartConfig.Api/SmartConfig.Silo/Grains/`
- Example: `HelloWorldGrain`, `SuspiciousEmailDomainsGrain`
- Commands in Application layer interact with grains via `IClusterClient`
- API wires up Orleans via `OrleansExtensions.cs`

### AI Agent Infrastructure

**SmartConfig.Agent** is a separate microservice hosting AI agents:
- **Architecture:** Minimal API + MediatR pattern
- **OrchestratorAgent** routes user requests to specialized worker agents based on capability matching
- **Worker Agents** (implement `IWorkerAgent`):
  - `GeneralPurposeAgent` - Fallback for general queries
  - `JokeAgent`, `HelloWorldAgent` - Example specialized agents
- **Agent Routing:** OrchestratorAgent uses structured output (JSON schema) to create execution plans
- Tools are registered via `IWorkerAgent` implementations and can be shared across agents

**AI Services** (configured in `ResourceAiExtensions.cs`):
- **Ollama** - Local LLM hosting (models like llama3.2)
- **n8n** - Workflow automation with AI capabilities
- **AnythingLLM** - Optional LLM UI/orchestration

### Model Context Protocol (MCP)

**SmartConfig.McpServer** exposes MCP-compliant tools:
- Located in `src/SmartConfig.Mcp/SmartConfig.McpServer/Tools/`
- Example tools: `EchoTool`, `OrleansTool`
- Integrated via `.mcp.json` configuration in repository root
- Can be consumed by Claude Desktop or other MCP clients

### Multi-Frontend Architecture

The solution supports **four frontend frameworks** simultaneously:
1. **Blazor** (Hybrid Server + WASM) - `SmartConfig.App.Web` + `SmartConfig.App.Web.Client`
2. **Next.js** - `SmartConfig.NextJs` (React with SSR)
3. **React** - `SmartConfig.React` (SPA with Vite)
4. **Angular** - `SmartConfig.Angular` (SPA)

All frontends:
- Consume the same backend API (`SmartConfig.Api`)
- Include OpenTelemetry instrumentation for observability
- Are orchestrated via Aspire (`ResourceFrontendExtensions.cs`)
- Frontend selection is controlled via `appsettings.json` in `SmartConfig.Host`

### Infrastructure Orchestration

**Aspire Host** (`tools/SmartConfig.Host/AppHost.cs`) orchestrates:
- **Databases:** SQL Server with `SmartConfig` and `TickerQ` databases
- **Message Queue:** RabbitMQ for async messaging
- **Observability:** OpenTelemetry collector (optional, see `ResourceInfraExtensions.cs`)
- **Dependency Graph:** Migration → API → Agent/Frontends

Resource configuration files:
- `ResourceInfraExtensions.cs` - Databases, RabbitMQ, OTel
- `ResourceAiExtensions.cs` - AI services (Agent, Ollama, n8n, AnythingLLM)
- `ResourceFrontendExtensions.cs` - Frontend apps with environment configuration

### SDKs

Strongly-typed clients for API consumption:
- **SmartConfig.BE.Sdk** - Client for `SmartConfig.Api`
- **SmartConfig.AI.Sdk** - Client for `SmartConfig.Agent`

These SDKs are used by frontends and can be used by external consumers.

### Scheduler

**SmartConfig.Scheduler** runs background jobs:
- Uses its own database (`TickerQ`)
- Integrated with RabbitMQ for job queue management
- Enabled via configuration flag: `SmartConfig:Clients:Scheduler`

## Configuration

### Environment Variables

Configuration is loaded from:
1. `appsettings.json` in each project
2. `.env.local` in repository root (loaded via `DotNetEnv` in AppHost)
3. User secrets (configured per project)

### Feature Flags

Key configuration sections in `SmartConfig.Host/appsettings.json`:

```json
{
  "SmartConfig": {
    "Clients": {
      "Scheduler": false,
      "Frontends": ["blazor", "nextjs", "react", "angular"]
    },
    "Ai": {
      "Clients": ["agent", "n8n", "anythingLlm"],
      "n8nDefaultAgent": false
    }
  }
}
```

AI Agent requires `Agent:OpenRouter:ApiKey` to be configured (loaded from secrets/environment).

## Extension Method Patterns

The codebase uses extension methods extensively for dependency injection:

**API Layer:**
- `IocExtensions.cs` - Register services
- `SecurityExtensions.cs` - Auth/authz setup
- `CorsExtensions.cs` - CORS policies
- `SwaggerExtensions.cs` - API documentation
- `OrleansExtensions.cs` - Orleans cluster client setup
- `RabbitMqExtensions.cs` - Message queue configuration

**Application Layer:**
- `MediatRExtensions.cs` - Register MediatR with behaviors
- `ValidatorExtensions.cs` - FluentValidation setup
- `EntityFrameworkExtensions.cs` - EF Core and repository patterns

All extension methods follow the pattern of extending `WebApplicationBuilder` or `IServiceCollection`.

## Testing Strategy

### Unit Tests
- Use **NUnit**, **Moq**, **AutoFixture**, **Shouldly**
- Located in `tests/SmartConfig.UnitTests`
- In-memory database for data layer tests
- Mock grain clients for Orleans tests

### Integration Tests
- Located in `tests/SmartConfig.IntegrationTests`
- Test full API request/response cycles

### E2E Tests
- Located in `tests/SmartConfig.E2ETests`
- Simulate real user interactions with frontends

### Load Tests
- Located in `tests/SmartConfig.LoadTests`
- Performance and stability testing

## Orleans Implementation Notes

When working with Orleans:
1. Grain interfaces should be defined in `SmartConfig.Application` for reusability
2. Grain implementations live in `SmartConfig.Silo/Grains/`
3. Commands/Queries in Application layer use `IClusterClient` to invoke grains
4. Grains are stateful - state management is handled by Orleans runtime
5. API startup configures Orleans client, not silo (silo is in separate project)

## API Response Pattern

All API responses use `ApiResponse<T>` wrapper:
- Located in `SmartConfig.Api/Models/ApiResponse.cs`
- Extension methods in `ApiResponseExtensions.cs` for building responses
- Filters handle exceptions: `HttpResponseExceptionFilter.cs`

## Working with AI Agents

When modifying or adding agents:
1. Implement `IWorkerAgent` interface
2. Register in `AiIocExtensions.cs`
3. OrchestratorAgent automatically discovers registered agents
4. Agents can use Microsoft.Extensions.AI abstractions (`IChatClient`)
5. Tools can be shared across agents or agent-specific

Agent execution is streaming - use `IAsyncEnumerable<string>` for responses.

## Aspire Dashboard MCP (Claude Code & Cursor)

The Aspire host exposes an **MCP server** for monitoring and managing running resources:

**Configuration:** `.mcp.json` in repository root
```json
{
  "mcpServers": {
    "aspire-dashboard": {
      "type": "http",
      "url": "http://localhost:21006/mcp",
      "headers": {
        "x-mcp-api-key": "<COMPUTER_SPECIFIC_KEY>"
      }
    }
  }
}
```

**Prerequisites:**
- Aspire host MUST be running first: `dotnet run --project tools/SmartConfig.Host/SmartConfig.Host.csproj`
- MCP server is only available when Aspire dashboard is active
- Default MCP endpoint: `http://localhost:21006/mcp`

**Available MCP Tools:**

1. **`mcp__aspire-dashboard__list_resources`**
   - Lists all resources/services orchestrated by Aspire
   - Shows status, health, and endpoints for each service
   - Use to check if API, databases, frontends are running

2. **`mcp__aspire-dashboard__list_console_logs`**
   - View console output from specific resources
   - Filter by resource name and time range
   - Essential for debugging startup issues

3. **`mcp__aspire-dashboard__list_structured_logs`**
   - View structured telemetry logs with metadata
   - Filter by log level, resource, timestamp
   - Better for production debugging with log scopes

4. **`mcp__aspire-dashboard__list_traces`**
   - View distributed traces across services
   - Shows request flow through API → Orleans → Database
   - Essential for performance profiling

5. **`mcp__aspire-dashboard__list_trace_structured_logs`**
   - View structured logs associated with specific trace IDs
   - Correlate logs with distributed traces
   - Debug cross-service request failures

6. **`mcp__aspire-dashboard__execute_resource_command`**
   - Execute commands on running resources
   - Restart services, trigger health checks, etc.
   - Use with caution in production environments

**Common Usage Patterns:**

```markdown
# Check if all services are healthy
Use list_resources to see status of API, databases, frontends

# Debug API startup failure
Use list_console_logs for "smartconfig-api" resource

# Investigate slow API requests
Use list_traces filtered by duration > 1000ms

# Correlate errors across services
Use list_trace_structured_logs with trace ID from error
```

**Environment Variables (configured in `launchSettings.json`):**
- `ASPIRE_DASHBOARD_MCP_ENDPOINT_URL`: MCP server endpoint
- `ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL`: Telemetry collector (gRPC)
- `ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL`: Telemetry collector (HTTP)
- `ASPIRE_RESOURCE_SERVICE_ENDPOINT_URL`: Resource management endpoint
