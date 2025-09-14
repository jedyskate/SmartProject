import os
from langchain_community.chat_models import ChatOllama
from langgraph.graph import StateGraph, END
from typing import Annotated
from typing_extensions import TypedDict
import operator
from langchain_core.messages import AnyMessage

class AgentState(TypedDict):
    messages: Annotated[list[AnyMessage], operator.add]

def call_model(state):
    messages = state['messages']
    ollama_base_url = os.getenv("OLLAMA_BASE_URL", "http://localhost:11434")
    model = ChatOllama(model="llama3.2", base_url=ollama_base_url)
    response = model.invoke(messages)
    return {"messages": [response]}

workflow = StateGraph(AgentState)
workflow.add_node("agent", call_model)
workflow.set_entry_point("agent")
workflow.add_edge("agent", END)

app = workflow.compile()
