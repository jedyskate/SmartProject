from fastapi import FastAPI
from langserve import add_routes
from app.graph import app as langgraph_app

api = FastAPI(
    title="SmartConfig LangGraph Server",
    version="1.0",
    description="A simple LangGraph server.",
)

add_routes(
    api,
    langgraph_app,
    path="/agent",
)

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(api, host="0.0.0.0", port=8000)
