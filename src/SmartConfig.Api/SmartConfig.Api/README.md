# SmartConfig API

This project is the core backend API for the SmartConfig application suite. It is built using .NET 9, ASP.NET Core, and Microsoft Orleans to create a scalable, distributed, and maintainable system.

## Architecture Overview

The API follows the principles of **Clean Architecture** combined with the **CQRS (Command Query Responsibility Segregation)** pattern. This creates a clear separation of concerns, making the system easier to develop, test, and maintain.

The solution is structured into the following layers:

-   **`SmartConfig.Api` (Presentation Layer)**:
    -   Hosts the ASP.NET Core web server.
    -   Contains lightweight controllers that receive HTTP requests.
    -   Handles API-specific concerns like routing, authentication, serialization, and exception handling through custom filters.
    -   It does **not** contain business logic.

-   **`SmartConfig.Application` (Application Layer)**:
    -   Implements the CQRS pattern using the **MediatR** library. All business logic is encapsulated in `Command` and `Query` handlers.
    -   Controllers from the API layer dispatch requests to MediatR, which then invokes the appropriate handler here.
    -   Defines interfaces for infrastructure services (like data access or external services).

-   **`SmartConfig.Core` (Domain Layer)**:
    -   The heart of the application. Contains the core domain models, enums, and business rules.
    -   This layer has no dependencies on any other layer in the solution.

-   **`SmartConfig.Data` (Infrastructure Layer)**:
    -   Provides the implementation for data persistence using **Entity Framework Core**.
    -   Includes the `DbContext`, database migrations, and data seeding logic.

## Key Technologies & Features

-   **Microsoft Orleans**: Used as the backbone for building a distributed, actor-based system. Business logic that requires stateful, concurrent processing is implemented in Orleans Grains, providing high performance and scalability. The API is configured to run as an Orleans Silo.

-   **CQRS with MediatR**: Ensures a clean separation between read (Query) and write (Command) operations, simplifying the business logic.

-   **Asynchronous Messaging**: Integrates with **RabbitMQ** via a hosted service (`RabbitMqRequestHandler`) to process messages from a queue in the background, enabling event-driven scenarios.

-   **Configuration Management**: Uses a flexible configuration system that reads from `appsettings.json`, environment variables, and **HashiCorp Vault** for secure secret management.

-   **Observability**: Implements comprehensive observability using **OpenTelemetry** for collecting and exporting logs, traces, and metrics, providing deep insights into the application's behavior.

-   **API Documentation**: Automatically generates interactive API documentation using **Swagger (OpenAPI)**, which is available at the `/swagger` endpoint when the application is running.

## How to Run

1.  **Prerequisites**:
    -   .NET 9 SDK
    -   (Optional) SQL Server, Redis, and RabbitMQ instances for running with non-local configurations.

2.  **Running Locally**:
    -   The project is configured to run with a local, in-memory Orleans cluster and a local SQL Server database by default.
    -   Use the `SmartConfig.Api` launch profile in Visual Studio or run the following command from the solution root:

    ```bash
    dotnet run --project src/SmartConfig.Api/SmartConfig.Api.csproj
    ```

3.  **Accessing the API**:
    -   The API will be available at the URLs specified in `launchSettings.json` (e.g., `https://localhost:5111`).
    -   The interactive Swagger UI can be accessed at `https://localhost:5111/swagger`.
