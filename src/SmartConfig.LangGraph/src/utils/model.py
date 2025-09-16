from __future__ import annotations

import os
from langchain_openai import ChatOpenAI


def get_chat_model() -> ChatOpenAI:
    """Return the chat model to use across agents and supervisor.

    Defaults to the requested model unless overridden via OPENAI_MODEL.
    """

    # Use a widely available default; override via OPENAI_MODEL
    model_name = os.getenv("OPENAI_MODEL", "gpt-5-nano-2025-08-07")
    return ChatOpenAI(model=model_name, temperature=0)


