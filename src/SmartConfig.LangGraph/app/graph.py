import os
import operator
from typing import Annotated, List

from langchain_community.chat_models import ChatOllama
from langchain_core.messages import BaseMessage, HumanMessage, AIMessage
from langgraph.graph import StateGraph, END
from typing_extensions import TypedDict


# Define state using TypedDict + Annotated reducer
class AgentState(TypedDict):
    messages: Annotated[List[BaseMessage], operator.add]


# Node function
def call_model(state: AgentState):
    ollama_base_url = os.getenv("OLLAMA_BASE_URL", "http://host.docker.internal:11434")
    model = ChatOllama(model="llama3.2", base_url=ollama_base_url)

    # Convert all messages to supported types for Ollama
    messages = []
    for msg in state["messages"]:
        if isinstance(msg, (HumanMessage, AIMessage)):
            messages.append(msg)
        else:
            # Wrap any unsupported message as HumanMessage
            messages.append(HumanMessage(content=str(msg)))

    response = model.invoke(messages)
    return {"messages": [response]}


# Build workflow
workflow = StateGraph(AgentState)
workflow.add_node("agent", call_model)
workflow.set_entry_point("agent")
workflow.add_edge("agent", END)

# Compile into app
app = workflow.compile()
