import os
import operator
from typing import Annotated, List

from langchain_ollama.chat_models import ChatOllama
from langchain_core.messages import BaseMessage, HumanMessage, AIMessage, SystemMessage
from langgraph.graph import StateGraph, END
from typing_extensions import TypedDict

# Define state using TypedDict + Annotated reducer
class AgentState(TypedDict):
    messages: Annotated[List[BaseMessage], operator.add]

# Mapping from type string to LangChain message class
MESSAGE_TYPE_MAP = {
    "human": HumanMessage,
    "ai": AIMessage,
    "system": SystemMessage,
}

def convert_to_ollama_message(msg) -> BaseMessage:
    """Convert any message to a supported Ollama message type."""
    if isinstance(msg, (HumanMessage, AIMessage, SystemMessage)):
        return msg
    if hasattr(msg, "type") and hasattr(msg, "content"):
        cls = MESSAGE_TYPE_MAP.get(msg.type.lower(), HumanMessage)
        return cls(content=str(msg.content))
    # fallback for plain strings or unknown objects
    return HumanMessage(content=str(msg))

# Node function
def call_model(state: AgentState):
    ollama_base_url = os.getenv(
        "OLLAMA_BASE_URL", "http://host.docker.internal:11434"
    )
    model = ChatOllama(model="llama3.2", base_url=ollama_base_url)

    # Convert all messages to Ollama-supported types
    messages_for_ollama = [convert_to_ollama_message(m) for m in state["messages"]]

    response = model.invoke(messages_for_ollama)
    return {"messages": [response]}  # accumulator

# Build workflow
workflow = StateGraph(AgentState)
workflow.add_node("agent", call_model)
workflow.set_entry_point("agent")
workflow.add_edge("agent", END)

# Compile into app
app = workflow.compile()
