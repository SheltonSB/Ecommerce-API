from __future__ import annotations

from dataclasses import dataclass
import threading
from typing import List, Dict

import requests

from .config import Settings


class LLMError(RuntimeError):
    pass


@dataclass
class LLMResponse:
    content: str


class BaseBackend:
    def chat(self, messages: List[Dict[str, str]], temperature: float, max_tokens: int) -> LLMResponse:
        raise NotImplementedError


class OllamaBackend(BaseBackend):
    def __init__(self, settings: Settings) -> None:
        self.base_url = settings.ollama_base_url.rstrip("/")
        self.model_name = settings.model_name
        self.n_ctx = settings.n_ctx
        self._lock = threading.Lock()

    def chat(self, messages: List[Dict[str, str]], temperature: float, max_tokens: int) -> LLMResponse:
        url = f"{self.base_url}/api/chat"
        payload = {
            "model": self.model_name,
            "messages": messages,
            "stream": False,
            "options": {
                "temperature": temperature,
                "num_ctx": self.n_ctx,
                "num_predict": max_tokens,
            },
        }
        with self._lock:
            try:
                response = requests.post(url, json=payload, timeout=120)
            except requests.RequestException as exc:
                raise LLMError(f"Ollama request failed: {exc}") from exc
        if response.status_code != 200:
            raise LLMError(f"Ollama error {response.status_code}: {response.text}")
        data = response.json()
        message = data.get("message", {})
        content = message.get("content", "")
        return LLMResponse(content=content.strip())


class LlamaCppBackend(BaseBackend):
    def __init__(self, settings: Settings) -> None:
        if not settings.model_path:
            raise LLMError("MODEL_PATH is required for llama_cpp backend.")
        try:
            from llama_cpp import Llama
        except Exception as exc:
            raise LLMError(
                "llama-cpp-python is not installed. Install requirements-llama-cpp.txt"
            ) from exc
        self._lock = threading.Lock()
        self.llm = Llama(
            model_path=settings.model_path,
            n_ctx=settings.n_ctx,
            n_gpu_layers=settings.n_gpu_layers,
            logits_all=False,
            verbose=False,
        )

    def chat(self, messages: List[Dict[str, str]], temperature: float, max_tokens: int) -> LLMResponse:
        with self._lock:
            result = self.llm.create_chat_completion(
                messages=messages,
                temperature=temperature,
                max_tokens=max_tokens,
            )
        content = (
            result.get("choices", [{}])[0]
            .get("message", {})
            .get("content", "")
        )
        return LLMResponse(content=content.strip())


def get_backend(settings: Settings) -> BaseBackend:
    backend = settings.backend.lower()
    if backend == "ollama":
        return OllamaBackend(settings)
    if backend == "llama_cpp":
        return LlamaCppBackend(settings)
    raise LLMError(f"Unknown BACKEND: {settings.backend}")
