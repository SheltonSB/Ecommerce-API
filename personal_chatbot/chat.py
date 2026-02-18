from __future__ import annotations

import uuid
from typing import List, Dict

from .config import Settings
from .llm import get_backend, LLMError
from .memory import (
    init_db,
    add_message,
    get_recent_messages,
    set_memory,
    get_latest_memories,
    get_recent_notes,
    extract_memories_heuristic,
    PROFILE_KEYS,
)

DEFAULT_SYSTEM_PROMPT = (
    "You are a personal assistant. Be concise, helpful, and accurate. "
    "Use the provided user profile and notes as long-term memory. "
    "If something is unknown, ask a brief clarifying question."
)


def new_session_id() -> str:
    return uuid.uuid4().hex


class Chatbot:
    def __init__(self, settings: Settings) -> None:
        self.settings = settings
        self.backend = None
        self.backend_error: str | None = None
        try:
            self.backend = get_backend(settings)
        except LLMError as exc:
            self.backend_error = str(exc)
        init_db()

    def _build_memory_block(self, user_message: str) -> str | None:
        profile = get_latest_memories(PROFILE_KEYS)
        notes = get_recent_notes(self.settings.memory_notes_limit)

        if not profile and not notes:
            return None

        lines: List[str] = []
        if profile:
            lines.append("User Profile:")
            for key in PROFILE_KEYS:
                value = profile.get(key)
                if value:
                    label = key.replace("_", " ").title()
                    lines.append(f"- {label}: {value}")
        if notes:
            lines.append("Recent Notes:")
            for note in notes:
                lines.append(f"- {note}")
        return "\n".join(lines)

    def _build_messages(self, session_id: str, user_message: str) -> List[Dict[str, str]]:
        system_prompt = self.settings.system_prompt or DEFAULT_SYSTEM_PROMPT
        messages: List[Dict[str, str]] = [{"role": "system", "content": system_prompt}]

        memory_block = self._build_memory_block(user_message)
        if memory_block:
            messages.append({"role": "system", "content": memory_block})

        history = get_recent_messages(session_id, self.settings.memory_last_n)
        messages.extend(history)
        messages.append({"role": "user", "content": user_message})
        return messages

    def _update_memory(self, user_message: str) -> None:
        if self.settings.memory_mode != "heuristic":
            return
        memories = extract_memories_heuristic(user_message)
        for key, value, replace in memories:
            if value:
                set_memory(key, value, replace=replace)

    def chat(self, session_id: str, user_message: str) -> str:
        user_message = user_message.strip()
        if not user_message:
            return "Please enter a message."
        if self.backend_error or self.backend is None:
            return f"LLM backend not available: {self.backend_error}"

        messages = self._build_messages(session_id, user_message)

        try:
            response = self.backend.chat(
                messages=messages,
                temperature=self.settings.temperature,
                max_tokens=self.settings.max_tokens,
            )
        except LLMError as exc:
            return f"LLM error: {exc}"

        add_message(session_id, "user", user_message)
        add_message(session_id, "assistant", response.content)
        self._update_memory(user_message)
        return response.content
