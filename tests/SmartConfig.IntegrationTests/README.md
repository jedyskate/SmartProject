# SmartConfig Integration Tests

This project contains the integration tests for the `SmartConfig.Api`. It is designed to test the API's endpoints and business logic in a controlled environment.

## Overview

The integration tests use `Microsoft.AspNetCore.Mvc.Testing` to host an in-memory instance of the API. This allows the tests to run without needing to deploy the actual application.

API requests are made using a strongly-typed client, `SmartConfigClient`, which is auto-generated from the API's OpenAPI specification and is located in the `SmartConfig.BE.Sdk` project. This ensures that the tests are always in sync with the API's contract.

## How to Run

### 1. Build the Solution

Ensure all project dependencies are restored and the solution is built:

```bash
  dotnet build
```

### 2. Run the Tests

Execute the tests using the .NET CLI from the `tests/SmartConfig.IntegrationTests` directory or the solution root:

```bash
  dotnet test
```

The test runner will handle starting the in-memory server, executing the tests against it, and then tearing it down.