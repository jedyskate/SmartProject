# SmartConfig Scheduler

This project is a background service responsible for managing and executing scheduled tasks using the **TickerQ** library. It provides reliable, persistent job scheduling for the SmartConfig application.

## What it Does

-   **Manages Scheduled Jobs**: Uses TickerQ to handle both recurring (CRON-based) and one-time (fire-and-forget) background tasks.
-   **Persistent Storage**: Leverages a SQL Server database to persist job definitions and their execution history, ensuring reliability across application restarts.
-   **Provides a Monitoring Dashboard**: Hosts the TickerQ web dashboard, allowing developers to monitor job statuses, view execution history, and manually manage tasks.

## How to Access

The scheduler is automatically launched when running the main `SmartConfig.Host` project.

-   **Dashboard URL**: `https://localhost:7068/tickerQ`
-   **Default Username**: `admin`
-   **Default Password**: `admin`

You can also run the project standalone:

```bash
  dotnet run --project src/SmartConfig.Scheduler/SmartConfig.Scheduler.csproj
```
