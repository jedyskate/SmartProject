# SmartConfig Solution

This repository contains the source code for the **SmartConfig** application suite, a comprehensive system designed for managing user configurations and settings.

## Solution Structure

The solution is organized into the following high-level folders:

-   **`/src`**: Contains the source code for the main application projects, following a Clean Architecture approach.
    -   **`SmartConfig.Api`**: The core backend REST API.
    -   **`SmartConfig.Application`**: The application layer containing business logic (CQRS).
    -   **`SmartConfig.Core`**: The domain layer with core models and business rules.
    -   **`SmartConfig.Data`**: The infrastructure layer for data persistence (Entity Framework).
    -   **`SmartConfig.Blazor`**: The Blazor-based frontend application.
    -   **`SmartConfig.NextJs`**: The Next.js-based frontend application.
    -   **`SmartConfig.Orleans.Silo`**: Hosts the Orleans grains for distributed, stateful logic.

-   **`/sdks`**: Contains th~~~~e Software Development Kit for interacting with the API.
    -   **`SmartConfig.BE.Sdk`**: A strongly-typed client for the `SmartConfig.Api`.

-   **`/tools`**: Contains development and operational tools.
    -   **`SmartConfig.Host`**: A **.NET Aspire** application host that orchestrates the entire application stack for local development.
    -   **`SmartConfig.Migration`**: Handles database migrations.

-   **`/tests`**: Contains the automated tests for the solution.
    -   **`SmartConfig.UnitTests`**: Unit tests for the application logic.
    -   **`SmartConfig.IntegrationTests`**: Integration tests for the API.
    -   **`SmartConfig.E2ETests`**: End-to-end tests that simulate user interaction with the frontends.
    -   **`SmartConfig.LoadTests`**: Load tests for measuring the performance and stability of the API.

## Getting Started

The easiest way to run the entire application for development is to use the **.NET Aspire Application Host**.

From the root of the repository, run:

```bash
  dotnet run --project tools/SmartConfig.Host/SmartConfig.Host.csproj
```

This single command will launch all the necessary services, including the database, backend, and frontends, and provide a developer dashboard for monitoring the system.
