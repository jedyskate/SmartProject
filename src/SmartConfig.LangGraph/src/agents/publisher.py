from __future__ import annotations

from pathlib import Path
from datetime import datetime
from typing import Literal
from langgraph.graph import MessagesState
from langgraph.types import Command
from langgraph.graph import END


OUTPUTS_DIR = Path(__file__).resolve().parents[2] / "outputs"


# Saves the final blog post content to a markdown file
def publisher(state: MessagesState) -> Command[Literal["supervisor", "__end__"]]:
    """Publishes the final blog post content to filesystem."""

    print("📄 Publisher: Starting content publication...")
    
    messages = state["messages"]
    
    # Find the writer agent's output (the actual blog post)
    final_content = "No content generated."
    
    # Look for the writer agent output (should be the main content)
    for msg in reversed(messages):  # Start from the end
        if hasattr(msg, "content") and msg.content:
            content = str(msg.content)
            
            # Skip system messages and transitions
            if (hasattr(msg, "type") and msg.type == "system") or \
               (hasattr(msg, "role") and msg.role == "system"):
                continue
            
            # Skip transition and completion messages
            if any(phrase in content for phrase in [
                "Transitioning to", "Moving to", "completed", "Ready for", 
                "Content planning", "Research completed", "Writing completed", 
                "Supervisor decided to call"
            ]):
                continue
            
            # Skip very short messages (likely just status updates)
            if len(content.strip()) < 100:
                continue
            
            # Skip research plans or outlines
            if any(phrase in content for phrase in [
                "What I'll do next", "Key data points I plan to include", "Preliminary findings",
                "How I'll structure the draft", "What I need from you", "Next step"
            ]):
                continue
            
            # This should be the actual written content from writer agent
            final_content = content
            print(f"📄 Found blog post content from: {getattr(msg, 'type', 'unknown')} message")
            break
    
    # If we still haven't found content, try to get the longest non-system message
    if final_content == "No content generated.":
        print("⚠️ Could not find specific blog post content, trying fallback...")
        longest_content = ""
        for msg in messages:
            if hasattr(msg, "content") and msg.content:
                content = str(msg.content)
                if (hasattr(msg, "type") and msg.type == "system") or \
                   (hasattr(msg, "role") and msg.role == "system"):
                    continue
                if len(content) > len(longest_content) and len(content) > 500:
                    longest_content = content
        
        if longest_content:
            final_content = longest_content
            print(f"📄 Using fallback content (longest message): {len(final_content)} characters")
    
    # Save to file
    OUTPUTS_DIR.mkdir(parents=True, exist_ok=True)
    ts = datetime.utcnow().strftime("%Y%m%d-%H%M%S")
    path = OUTPUTS_DIR / f"blog-post-{ts}.md"
    path.write_text(final_content)
    
    print(f"📄 Final blog post saved to: {path}")
    print(f"📝 Content length: {len(final_content)} characters")
    
    return Command(
        goto=END,
        update={
            "messages": [
                {"role": "system", "content": f"Published to {path}"},
                {"role": "user", "content": final_content}  # Add the final content as the last message
            ]
        },
    )


