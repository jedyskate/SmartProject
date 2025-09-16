from __future__ import annotations

import os
from dataclasses import dataclass
from typing import Optional

from dotenv import load_dotenv


load_dotenv()


@dataclass(frozen=True)
class Settings:
    tavily_api_key: Optional[str] = os.getenv("TAVILY_API_KEY")


settings = Settings()


