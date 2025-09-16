from __future__ import annotations

from typing import Annotated, Literal, TypedDict

from .utils.model import get_chat_model
from langgraph.graph import StateGraph, START, END, MessagesState
from langgraph.types import Command
from langgraph.graph.message import add_messages

from .agents.content_planner import content_planner
from .agents.research_agent import research_agent
from .agents.writer_agent import writer_agent
from .agents.publisher import publisher


class State(TypedDict):
    messages: Annotated[list, add_messages]
    current_stage: str
    completed_stages: list[str]
    session_id: str  # Add session ID for isolation


# LLM-based supervisor that intelligently routes between workflow stages
def supervisor_node(state: State) -> Command:
    """LLM-based supervisor that decides which agent to call next."""
    
    model = get_chat_model()
    
    # Get current state info
    completed = state.get("completed_stages", [])
    current = state.get("current_stage", "")
    session_id = state.get("session_id", "unknown")
    
    # Mark current stage as completed
    if current and current not in completed:
        completed.append(current)
        print(f"✅ Completed stage: {current} (Session: {session_id})")
    
    # Check if all stages are done
    all_stages = ["content_planner", "research_agent", "writer_agent", "publisher"]
    
    if len(completed) >= len(all_stages):
        print(f"🎉 All stages completed! Ending workflow. (Session: {session_id})")
        return Command(goto=END)
    
    # Create LLM prompt for routing decision with session context
    prompt = f"""You are a content creation supervisor for session {session_id}. Based on the current state, decide which agent to call next.

CURRENT STATE:
- Session ID: {session_id}
- Completed stages: {completed}
- Available stages: {[s for s in all_stages if s not in completed]}

WORKFLOW STAGES:
1. content_planner: Creates content strategy and outline
2. research_agent: Conducts research and gathers information  
3. writer_agent: Writes the complete, publishable blog post with facts and SEO
4. publisher: Saves final content to file

The workflow should follow this order: planning → research → writing → publishing.

DECISION: Which agent should be called next? Respond with ONLY the agent name (e.g., "writer_agent").
"""
    
    # Get LLM decision
    messages = state["messages"] + [{"role": "system", "content": prompt}]
    response = model.invoke(messages)
    next_stage = response.content.strip().lower()
    
    # Validate the decision
    if next_stage not in all_stages:
        print(f"⚠️ LLM chose invalid stage '{next_stage}', defaulting to next in sequence (Session: {session_id})")
        remaining = [s for s in all_stages if s not in completed]
        next_stage = remaining[0] if remaining else "publisher"
    
    print(f"🔄 LLM decided to move to: {next_stage} (Session: {session_id})")
    
    # Add supervisor's decision to messages
    decision_msg = {
        "role": "system", 
        "content": f"Supervisor decided to call {next_stage}. Completed stages: {completed}. Session: {session_id}"
    }
    
    return Command(
        goto=next_stage,
        update={ 
            "messages": [decision_msg],
            "current_stage": next_stage,
            "completed_stages": completed,
            "session_id": session_id
        }
    )


# Builds the LangGraph workflow with all agents and routing logic
def build_graph():
    """Build the streamlined content creation workflow using LangGraph."""
    
    builder = StateGraph(State)
    
    # Add nodes
    builder.add_node("supervisor", supervisor_node)
    builder.add_node("content_planner", content_planner)
    builder.add_node("research_agent", research_agent)
    builder.add_node("writer_agent", writer_agent)
    builder.add_node("publisher", publisher)
    
    # Add edges
    builder.add_edge(START, "supervisor")
    
    # All agents return to supervisor after completion
    for node in ["content_planner", "research_agent", "writer_agent"]:
        builder.add_edge(node, "supervisor")
    
    # Publisher ends the workflow
    builder.add_edge("publisher", END)
    
    return builder.compile()


# Create the graph
graph = build_graph()
