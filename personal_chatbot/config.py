from __future__ import annotations

from dataclasses import dataclass
import os
from pathlib import Path

from dotenv import load_dotenv

ROOT_DIR = Path(__file__).resolve().parent
DATA_DIR = ROOT_DIR / "data"
DATA_DIR.mkdir(parents=True, exist_ok=True)

# Load .env from package dir first, then fallback to CWD
load_dotenv(ROOT_DIR / ".env")
load_dotenv()


def _env_int(name: str, default: int) -> int:
    raw = os.getenv(name)
    if raw is None or raw == "":
        return default
    try:
        return int(raw)
    except ValueError:
        return default


def _env_float(name: str, default: float) -> float:
    raw = os.getenv(name)
    if raw is None or raw == "":
        return default
    try:
        return float(raw)
    except ValueError:
        return default


@dataclass(frozen=True)
class Settings:
    backend: str
    ollama_base_url: str
    model_name: str
    model_path: str
    n_ctx: int
    n_gpu_layers: int
    temperature: float
    max_tokens: int
    memory_mode: str
    memory_last_n: int
    memory_notes_limit: int
    web_host: str
    web_port: int
    system_prompt: str


def load_settings() -> Settings:
    return Settings(
        backend=os.getenv("BACKEND", "ollama").strip(),
        ollama_base_url=os.getenv("OLLAMA_BASE_URL", "http://localhost:11434").strip(),
        model_name=os.getenv("MODEL_NAME", "llama3.1").strip(),
        model_path=os.getenv("MODEL_PATH", "").strip(),
        n_ctx=_env_int("N_CTX", 4096),
        n_gpu_layers=_env_int("N_GPU_LAYERS", 0),
        temperature=_env_float("TEMPERATURE", 0.7),
        max_tokens=_env_int("MAX_TOKENS", 512),
        memory_mode=os.getenv("MEMORY_MODE", "heuristic").strip(),
        memory_last_n=_env_int("MEMORY_LAST_N", 12),
        memory_notes_limit=_env_int("MEMORY_NOTES_LIMIT", 6),
        web_host=os.getenv("WEB_HOST", "127.0.0.1").strip(),
        web_port=_env_int("WEB_PORT", 8000),
        system_prompt=os.getenv("SYSTEM_PROMPT", "").strip(),
    )
