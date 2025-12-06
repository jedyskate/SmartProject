# SmartConfig Solution

The SmartConfig Solution provides a comprehensive and modular foundation for exploring and building within the .NET AI ecosystem.
It’s designed to help developers get started easily by combining modern .NET technologies, AI agent infrastructure, and cloud-native development practices — all in one cohesive setup.

## Solution Structure

The solution is organized into the following high-level folders:

-   **`/src/SmartConfig.Api`**: Contains the source code for the main API application projects, following a Clean Architecture approach.
    -   **`SmartConfig.Api`**: The core backend REST API.
    -   **`SmartConfig.Application`**: The application layer containing business logic (CQRS).
    -   **`SmartConfig.Core`**: The domain layer with core models and business rules.
    -   **`SmartConfig.Data`**: The infrastructure layer for data persistence (Entity Framework).
    -   **`SmartConfig.Orleans.Silo`**: Hosts the Orleans grains for distributed, stateful logic.

-   **`/src/SmartConfig.Agent`**: A dotnet micro-service that hosts the AI agents logic.
    -   **`SmartConfig.Agent`**: Minimal API with MediatR.
    -   **`SmartConfig.Services`**: Business/Agent layer.

-   **`/src/SmartConfig.App`**: Contains the web-only Blazor application with hybrid rendering (Server + WebAssembly).
    -   **`SmartConfig.App.Shared`**: The shared Razor components and pages used by both server and client.
    -   **`SmartConfig.App.Web`**: The Blazor Server application with interactive rendering modes.
    -   **`SmartConfig.App.Web.Client`**: The Blazor WebAssembly client-side application.

-   **`/src/SmartConfig.Mcp`**: A .NET application for managing and coordinating other parts of the system.
-   **`/src/SmartConfig.Scheduler`**: A .NET application for running scheduled jobs.
-   **`/src/SmartConfig.NextJs`**: The Next.js-based frontend application.
-   **`/src/SmartConfig.Angular`**: The Angular-based frontend application.
-   **`/src/SmartConfig.React`**: The React-based frontend application.

-   **`/sdks`**: Contains the Software Development Kit for interacting with the API and Agent.
    -   **`SmartConfig.BE.Sdk`**: A strongly-typed client for the `SmartConfig.Api`.
    -   **`SmartConfig.AI.Sdk`**: A strongly-typed client for the `SmartConfig.Agent`.

-   **`/tools`**: Contains development and operational tools.
    -   **`SmartConfig.Host`**: A **.NET Aspire** application host that orchestrates the entire application stack for local development.
    -   **`SmartConfig.Migration`**: Handles database migrations.
    -   **`SmartConfig.ServiceDefaults`**: Provides default service configurations for the solution.

-   **`/tests`**: Contains the automated tests for the solution.
    -   **`SmartConfig.UnitTests`**: Unit tests for the application logic.
    -   **`SmartConfig.IntegrationTests`**: Integration tests for the API and Agent.
    -   **`SmartConfig.E2ETests`**: End-to-end tests that simulate user interaction with the frontends.
    -   **`SmartConfig.LoadTests`**: Load tests for measuring the performance and stability of the API.

## Getting Started

The easiest way to run the entire application for development is to use the **.NET Aspire Application Host**.

From the root of the repository, run:

```bash
  dotnet run --project tools/SmartConfig.Host/SmartConfig.Host.csproj
```

This single command will launch all the necessary services, including the database, backend, and frontends, and provide a developer dashboard for monitoring the system.
