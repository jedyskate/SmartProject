# SmartConfig E2E Tests

This project contains the end-to-end (E2E) tests for the SmartConfig application. It uses [.NET 9](https://dotnet.microsoft.com/download/dotnet/9.0), [NUnit](https://nunit.org/), and [Microsoft Playwright](https://playwright.dev/dotnet/) for browser automation.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

## How to Run

### 1. Start the Application Host

The E2E tests require the entire application stack to be running. The easiest way to achieve this is by running the `SmartConfig.Host` project.

From the root of the repository, run:

```bash
  dotnet run --project tools/SmartConfig.Host/SmartConfig.Host.csproj
```

This will start the API, the Blazor frontend, the Next.js frontend, and all required backend services.

### 2. Install Playwright Browsers

Before running the tests for the first time, you need to install the necessary browser binaries for Playwright.

From the `tests/SmartConfig.E2ETests` directory, run:

```bash
  pwsh bin/Debug/net9.0/playwright.ps1 install
```

### 3. Execute the Tests

Once the application host is running, open a **new terminal** and run the following command from the `tests/SmartConfig.E2ETests` directory:

```bash
  dotnet test
```

The tests will execute against the live application, simulating user interactions with the Blazor UI and the API's Swagger page.