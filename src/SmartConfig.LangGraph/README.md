# SmartConfig LangGraph

This project is an AI agent service built with Python using **LangGraph** and **FastAPI**. It provides a flexible and observable foundation for building multi-agent and tool-using AI systems for the SmartConfig application.

## What it Does

-   **Hosts AI Agents**: Uses LangGraph to define and run stateful, graph-based AI agents.
-   **Exposes a REST API**: Serves the agent's functionality through a FastAPI endpoint, powered by LangServe.
-   **Integrates with Ollama**: Connects to a local Ollama instance to run large language models like Llama 3.2.
-   **Connects to LangSmith**: Provides observability and debugging capabilities by tracing graph execution in LangSmith.

## How to Access

The LangGraph service is automatically launched as a Docker container when running the main `SmartConfig.Host` project.

-   **API Base URL**: `http://localhost:8000`

You can test the service directly using cURL:

```bash
curl -X POST http://localhost:8000/agent/invoke \
-H "Content-Type: application/json" \
-d '{"input": {"messages": [{"type": "human", "content": "Hello! What is LangGraph?"}]}}'
```

You can also build and run the project standalone:

```bash
# Build the image
docker build -t langgraph-image ./src/SmartConfig.LangGraph

# Run the container
docker run -p 8000:8000 -e OLLAMA_BASE_URL=http://host.docker.internal:11434 langgraph-image
```

