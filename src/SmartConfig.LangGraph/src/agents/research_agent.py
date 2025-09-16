from __future__ import annotations

from typing import Literal
from ..utils.model import get_chat_model
from langchain_core.tools import tool
from langgraph.graph import MessagesState
from langgraph.types import Command
from tavily import TavilyClient
from ..utils.config import settings


_tavily = None
try:
    if settings.tavily_api_key:
        _tavily = TavilyClient(api_key=settings.tavily_api_key)
except Exception:
    _tavily = None


# Web search tool for gathering research data
@tool
def web_search(query: str) -> str:
    """Search the web for current information on a topic."""

    if _tavily is None:
        return "Tavily not configured. Please set TAVILY_API_KEY to enable web search."
    data = _tavily.search(query=query, search_depth="basic", max_results=2)
    return str(data)


# Conducts research and gathers information for the blog post
def research_agent(state: MessagesState) -> Command[Literal["supervisor"]]:
    """Conducts research and gathers information for the blog post.

    Uses web search to find relevant facts, statistics, and sources to support
    the content plan and provide accurate information for the blog post.
    """

    print("🔬 Research Agent: Starting research...")
    
    model = get_chat_model()
    system = (
        "You are a research specialist. Use the web search tool to gather relevant facts, statistics, "
        "and sources for the blog post. Focus on finding credible, current information that supports "
        "the content plan."
    )
    messages = state["messages"] + [{"role": "system", "content": system}]
    
    # Get research query from the content plan
    research_query = f"research {state['messages'][-1].content if state['messages'] else 'blog post topic'}"
    
    # Perform web search
    observation = web_search.invoke(research_query)
    
    # Create research summary
    research_prompt = f"Based on the web search results, create a research summary with key facts, statistics, and sources for the blog post:\n\n{observation}"
    research_summary = model.invoke([{"role": "user", "content": research_prompt}])
    
    print("🔬 Research Agent: Research completed!")
    
    return Command(
        goto="supervisor",
        update={
            "messages": [research_summary, {"role": "user", "content": "Research completed. Ready for writing phase."}],
        },
    )


