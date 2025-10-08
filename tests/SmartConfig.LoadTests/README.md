# SmartConfig Load Tests

This project contains the load tests for the `SmartConfig.Api`. It is designed to simulate high-traffic scenarios and measure the performance and stability of the API under stress.

## Overview

The load tests are built using the [NBomber](https://nbomber.com/) framework. NBomber provides a declarative way to define scenarios, load simulations, and performance thresholds.

API requests are made using a strongly-typed client, `SmartConfigClient`, which is auto-generated from the API's OpenAPI specification and is located in the `SmartConfig.BE.Sdk` project.

## How to Run

### 1. Start the Application

Before running the load tests, the entire application stack must be running. The easiest way to do this is by using the .NET Aspire application host.

From the root of the repository, run:
```bash
  dotnet run --project tools/SmartConfig.Host/SmartConfig.Host.csproj
```

### 2. Run the Tests

Once the application is running, execute the tests using the .NET CLI from the `tests/SmartConfig.LoadTests` directory or the solution root:

```bash
  dotnet test
```

The test runner will execute the NBomber scenarios against the live API and report the results, including pass/fail statistics and latency metrics.
