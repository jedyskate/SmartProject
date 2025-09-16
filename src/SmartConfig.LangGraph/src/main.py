#!/usr/bin/env python3
import uuid
from typing import List
from fastapi import FastAPI
from langserve import add_routes
from pydantic import BaseModel
from langchain_core.runnables import RunnableLambda

from .supervisor_graph import graph  # <- keep this import

api = FastAPI(
    title="LangGraph Content Creation Supervisor",
    version="1.0",
    description="An API for a multi-agent content creation workflow.",
)

class AgentInput(BaseModel):
    prompt: str

class Message(BaseModel):
    role: str
    content: str

class State(BaseModel):
    messages: List[Message]
    current_stage: str
    completed_stages: List[str]
    session_id: str

def create_initial_state(inputs: dict) -> dict:
    # Convert dict input to Pydantic model
    agent_input = AgentInput.model_validate(inputs)

    state = State(
        messages=[Message(role="user", content=agent_input.prompt)],
        current_stage="",
        completed_stages=[],
        session_id=str(uuid.uuid4()),
    )
    return state.model_dump()

# Chain input mapping into graph
api_runnable = RunnableLambda(create_initial_state) | graph

# Expose API endpoint
add_routes(
    api,
    api_runnable,
    path="/agent",
    input_type=AgentInput,
    output_type=State,  # BaseModel is fine here
)

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(api, host="0.0.0.0", port=8000)
