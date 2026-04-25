from __future__ import annotations

import re
import sqlite3
from datetime import datetime, timezone
from typing import Dict, Iterable, List, Tuple

from .config import DATA_DIR

DB_PATH = DATA_DIR / "chatbot.db"

PROFILE_KEYS = [
    "name",
    "location",
    "job",
    "timezone",
    "email",
    "phone",
    "likes",
    "dislikes",
]


def _utc_now() -> str:
    return datetime.now(timezone.utc).isoformat()


def _connect() -> sqlite3.Connection:
    return sqlite3.connect(DB_PATH)


def init_db() -> None:
    with _connect() as conn:
        conn.execute(
            """
            CREATE TABLE IF NOT EXISTS messages (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                session_id TEXT NOT NULL,
                role TEXT NOT NULL,
                content TEXT NOT NULL,
                created_at TEXT NOT NULL
            );
            """
        )
        conn.execute(
            """
            CREATE TABLE IF NOT EXISTS memories (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                mem_key TEXT NOT NULL,
                mem_value TEXT NOT NULL,
                created_at TEXT NOT NULL
            );
            """
        )
        conn.execute(
            "CREATE INDEX IF NOT EXISTS idx_messages_session ON messages(session_id);"
        )
        conn.execute(
            "CREATE INDEX IF NOT EXISTS idx_memories_key ON memories(mem_key);"
        )
        conn.commit()


def add_message(session_id: str, role: str, content: str) -> None:
    with _connect() as conn:
        conn.execute(
            "INSERT INTO messages(session_id, role, content, created_at) VALUES (?, ?, ?, ?)",
            (session_id, role, content, _utc_now()),
        )
        conn.commit()


def get_recent_messages(session_id: str, limit: int) -> List[Dict[str, str]]:
    with _connect() as conn:
        rows = conn.execute(
            "SELECT role, content FROM messages WHERE session_id = ? ORDER BY id DESC LIMIT ?",
            (session_id, limit),
        ).fetchall()
    rows.reverse()
    return [{"role": role, "content": content} for role, content in rows]


def clear_session(session_id: str) -> None:
    with _connect() as conn:
        conn.execute("DELETE FROM messages WHERE session_id = ?", (session_id,))
        conn.commit()


def set_memory(key: str, value: str, replace: bool = True) -> None:
    with _connect() as conn:
        if replace:
            conn.execute("DELETE FROM memories WHERE mem_key = ?", (key,))
        conn.execute(
            "INSERT INTO memories(mem_key, mem_value, created_at) VALUES (?, ?, ?)",
            (key, value, _utc_now()),
        )
        conn.commit()


def get_latest_memories(keys: Iterable[str]) -> Dict[str, str]:
    result: Dict[str, str] = {}
    with _connect() as conn:
        for key in keys:
            if key in {\"likes\", \"dislikes\"}:
                rows = conn.execute(
                    \"SELECT mem_value FROM memories WHERE mem_key = ? ORDER BY id DESC LIMIT 5\",
                    (key,),
                ).fetchall()
                if rows:
                    values = [row[0] for row in rows][::-1]
                    result[key] = \", \".join(values)
                continue
            row = conn.execute(
                \"SELECT mem_value FROM memories WHERE mem_key = ? ORDER BY id DESC LIMIT 1\",
                (key,),
            ).fetchone()
            if row:
                result[key] = row[0]
    return result


def get_recent_notes(limit: int) -> List[str]:
    with _connect() as conn:
        rows = conn.execute(
            "SELECT mem_value FROM memories WHERE mem_key = 'note' ORDER BY id DESC LIMIT ?",
            (limit,),
        ).fetchall()
    return [row[0] for row in rows]


def search_memories(query: str, limit: int = 5) -> List[str]:
    like = f"%{query}%"
    with _connect() as conn:
        rows = conn.execute(
            """
            SELECT mem_value FROM memories
            WHERE mem_value LIKE ?
            ORDER BY id DESC LIMIT ?
            """,
            (like, limit),
        ).fetchall()
    return [row[0] for row in rows]


def _clean_value(value: str) -> str:
    value = value.strip().strip("\"").strip("'")
    value = re.sub(r"\s+", " ", value)
    return value[:200]


def extract_memories_heuristic(text: str) -> List[Tuple[str, str, bool]]:
    memories: List[Tuple[str, str, bool]] = []

    name_match = re.search(r"\bmy name is\s+([^\n\.,!\?]+)", text, re.IGNORECASE)
    if name_match:
        memories.append(("name", _clean_value(name_match.group(1)), True))

    call_me_match = re.search(r"\bcall me\s+([^\n\.,!\?]+)", text, re.IGNORECASE)
    if call_me_match:
        memories.append(("name", _clean_value(call_me_match.group(1)), True))

    location_match = re.search(
        r"\b(i live in|i am in|i'm in|i am from|i'm from)\s+([^\n\.,!\?]+)",
        text,
        re.IGNORECASE,
    )
    if location_match:
        memories.append(("location", _clean_value(location_match.group(2)), True))

    job_match = re.search(
        r"\b(i work at|i work for|i work with|my job is)\s+([^\n\.,!\?]+)",
        text,
        re.IGNORECASE,
    )
    if job_match:
        memories.append(("job", _clean_value(job_match.group(2)), True))

    tz_match = re.search(r"\b(my time zone is|my timezone is)\s+([^\n\.,!\?]+)", text, re.IGNORECASE)
    if tz_match:
        memories.append(("timezone", _clean_value(tz_match.group(2)), True))

    email_match = re.search(r"\b([A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,})\b", text)
    if email_match:
        memories.append(("email", _clean_value(email_match.group(1)), True))

    phone_match = re.search(r"\b(\+?\d[\d\-\s\(\)]{7,}\d)\b", text)
    if phone_match:
        memories.append(("phone", _clean_value(phone_match.group(1)), True))

    like_match = re.search(r"\b(i like|i love|my favorite)\s+([^\n\.,!\?]+)", text, re.IGNORECASE)
    if like_match:
        memories.append(("likes", _clean_value(like_match.group(2)), False))

    dislike_match = re.search(r"\b(i dislike|i hate)\s+([^\n\.,!\?]+)", text, re.IGNORECASE)
    if dislike_match:
        memories.append(("dislikes", _clean_value(dislike_match.group(2)), False))

    remember_match = re.search(r"\bremember(?: that|:)\s+([^\n\.,!\?]+)", text, re.IGNORECASE)
    if remember_match:
        memories.append(("note", _clean_value(remember_match.group(1)), False))

    return memories
