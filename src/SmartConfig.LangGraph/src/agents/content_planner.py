from __future__ import annotations

from typing import Annotated, Literal

from ..utils.model import get_chat_model
from langgraph.graph import MessagesState
from langgraph.types import Command


# Creates content strategy and outline for the blog post
def content_planner(state: MessagesState) -> Command[Literal["supervisor"]]:
    """Creates a content strategy and outline for the blog post.

    Analyzes the user's request and creates a structured plan including:
    - Target audience and tone
    - Key topics and subtopics
    - Research areas to explore
    - Content structure and flow
    """

    print("📝 Content Planner: Starting content planning...")
    
    model = get_chat_model()
    system = (
        "You are a content strategist. Create a comprehensive content plan and outline for the requested blog post. "
        "Include: target audience, key topics, research areas, content structure, and tone recommendations."
    )
    messages = state["messages"] + [{"role": "system", "content": system}]
    plan = model.invoke(messages)
    
    print("📝 Content Planner: Planning completed!")
    
    return Command(
        goto="supervisor",
        update={
            "messages": [plan, {"role": "user", "content": "Content planning completed. Ready for research phase."}],
        },
    )


