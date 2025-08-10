# SmartConfig Application Host

This project uses **.NET Aspire** to simplify the development and orchestration of the entire SmartConfig application suite. It acts as a single entry point to launch and manage all the different services that make up the application.

## What it Does

When you run this project, it automatically performs the following actions:

1.  **Starts a SQL Server Container**: Launches a persistent SQL Server container for the application's database.
2.  **Runs Database Migrations**: Executes the `SmartConfig.Migration` project to ensure the database schema is up-to-date.
3.  **Launches Backend Services**:
    -   Starts the `SmartConfig.Api` project.
    -   Starts the `SmartConfig.Scheduler` for background job processing.
4.  **Launches the Frontend Applications**:
    -   Starts the **Next.js** frontend (`SmartConfig.NextJs`).
    -   Starts the **React** frontend (`SmartConfig.React`).
    -   Starts the **Angular** frontend (`SmartConfig.Angular`).
    -   Starts the **Blazor** frontend (`SmartConfig.Blazor`).
5.  **Orchestrates Service Discovery**: .NET Aspire handles the communication between the services, injecting the correct environment variables (like the API URL for the frontends) automatically.
6.  **Provides a Developer Dashboard**: Launches the .NET Aspire Dashboard, which provides a centralized view of all running services, their logs, metrics, and traces.

In short, this project is the **master conductor** for the entire application, making it incredibly easy to get a full development environment up and running with a single command.

## How to Run

1.  **Prerequisites**:
    -   .NET 9 SDK
    -   Docker Desktop (for running the SQL Server container)
    -   Node.js and npm (for the Next.js, React, and Angular frontends)

2.  **Run the Host Project**:
    From the root of the repository, execute the following command:

    ```bash
    dotnet run --project tools/SmartConfig.Host/SmartConfig.Host.csproj
    ```

This will start all the services and open the Aspire Dashboard in your browser, giving you a complete overview of the running application.
