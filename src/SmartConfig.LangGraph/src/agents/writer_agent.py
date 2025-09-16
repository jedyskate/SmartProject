from __future__ import annotations

from ..utils.model import get_chat_model
from typing import Literal
from langgraph.graph import MessagesState
from langgraph.types import Command


# Writes complete blog post with facts, citations, and SEO optimization
def writer_agent(state: MessagesState) -> Command[Literal["supervisor"]]:
    """Writes a complete, publishable blog post with facts, citations, and SEO optimization.

    Creates a compelling, well-researched blog post that includes:
    - Compelling headline and introduction
    - Well-structured content with headings
    - Factual claims with citations
    - SEO optimization (keywords, meta description)
    - Engaging conclusion
    """

    print("✍️ Writer Agent: Starting content writing...")
    
    model = get_chat_model()
    system = (
        "You are a professional content writer. Write a complete, publishable blog post using the research brief and plan. "
        "Create a compelling, well-researched article that includes:"
        "\n\nCONTENT REQUIREMENTS:"
        "- Compelling headline that includes primary keywords"
        "- Introduction that hooks the reader and includes target keywords"
        "- Well-structured body with clear headings (H2, H3) and subheadings"
        "- Factual claims supported by the research with inline citations"
        "- Engaging, conversational tone that's easy to read"
        "- 800-1200 words of high-quality content"
        "- Conclusion with actionable takeaways"
        "\n\nSEO REQUIREMENTS:"
        "- Include primary and secondary keywords naturally throughout the text"
        "- Use proper heading structure (H1, H2, H3)"
        "- Write a compelling meta description (150-160 characters)"
        "- Include internal linking suggestions where relevant"
        "- Optimize for readability (short paragraphs, bullet points)"
        "\n\nFACT-CHECKING REQUIREMENTS:"
        "- Only include claims that are supported by the research"
        "- Add inline citations for statistics and facts"
        "- Use credible sources and data"
        "- Flag any uncertain claims or add disclaimers"
        "\n\nDO NOT write a research plan, outline, or analysis. Write the actual blog post that readers will see."
    )
    messages = state["messages"] + [{"role": "system", "content": system}]
    draft = model.invoke(messages)
    
    print("✍️ Writer Agent: Writing completed!")
    
    return Command(
        goto="supervisor",
        update={
            "messages": [draft, {"role": "user", "content": "Writing completed. Ready for publishing phase."}],
        },
    )


