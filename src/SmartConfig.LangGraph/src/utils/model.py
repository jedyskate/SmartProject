from __future__ import annotations

import os
from langchain_ollama import ChatOllama


def get_chat_model() -> ChatOllama:
    """Return the chat model to use across agents and supervisor.

    Defaults to the requested model unless overridden via OLLAMA_MODEL.
    """

    # Use a widely available default; override via OLLAMA_MODEL
    model_name = os.getenv("OLLAMA_MODEL", "llama3.2")
    base_url = os.getenv("OLLAMA_BASE_URL")
    return ChatOllama(model=model_name, temperature=0, base_url=base_url)


