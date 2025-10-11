# SmartConfig.Agent

A .NET microservice that hosts AI agent logic and exposes it via a minimal API.

## Overview

This service provides the core AI functionalities for the SmartConfig application. It uses MediatR to handle CQRS patterns and is configured to connect to various AI model providers like Ollama and OpenRouter.

## Key Features

-   **.NET 9 Minimal API**: Lightweight and fast API development.
-   **MediatR**: Implements CQRS for clean and decoupled logic.
-   **AI Integration**: Connects to AI providers (e.g., Ollama, OpenRouter) for chat completion.
-   **Streaming Endpoint**: Provides a real-time streaming chat experience.
-   **Swagger/OpenAPI**: Includes API documentation for clear and easy testing.

## Endpoints

The main endpoint provided by this service is:

-   `POST /agent/completechatstreaming`: Streams a response from the configured AI model based on the provided chat messages.

A sample endpoint is also included for health checks:

-   `GET /agent/weatherforecast`: Returns a sample weather forecast.

## Configuration

Service behavior can be configured in `appsettings.json`:

-   **`Agent`**: Settings for the AI providers, including URL, API keys, and model names.
-   **`RabbitMq`**: Configuration for connecting to RabbitMQ for message queuing.

## Getting Started

The recommended way to run this service for local development is through the **.NET Aspire Application Host** located in the `/tools` directory.

To run the service directly, use the following command from the solution root:

```bash
dotnet run --project src/SmartConfig.Agent/SmartConfig.Agent/SmartConfig.Agent.csproj
```
