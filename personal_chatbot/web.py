from __future__ import annotations

from typing import Optional

from fastapi import FastAPI, Request
from fastapi.responses import HTMLResponse
from fastapi.staticfiles import StaticFiles
from fastapi.templating import Jinja2Templates
from pydantic import BaseModel
import uvicorn

from .config import ROOT_DIR, load_settings
from .chat import Chatbot, new_session_id
from .memory import clear_session, get_recent_messages, get_latest_memories, get_recent_notes, PROFILE_KEYS

settings = load_settings()
bot = Chatbot(settings)

app = FastAPI(title="Personal Chatbot")

app.mount("/static", StaticFiles(directory=ROOT_DIR / "static"), name="static")

templates = Jinja2Templates(directory=ROOT_DIR / "templates")


class ChatRequest(BaseModel):
    message: str
    session_id: Optional[str] = None


@app.get("/", response_class=HTMLResponse)
def index(request: Request):
    return templates.TemplateResponse("index.html", {"request": request})


@app.get("/health")
def health():
    return {"ok": True}


@app.post("/api/chat")
def chat(req: ChatRequest):
    session_id = req.session_id or new_session_id()
    response = bot.chat(session_id, req.message)
    return {"response": response, "session_id": session_id}


@app.get("/api/history")
def history(session_id: str):
    messages = get_recent_messages(session_id, limit=200)
    return {"messages": messages}


@app.post("/api/reset")
def reset(session_id: str):
    clear_session(session_id)
    return {"ok": True}


@app.get("/api/profile")
def profile():
    profile_data = get_latest_memories(PROFILE_KEYS)
    notes = get_recent_notes(settings.memory_notes_limit)
    return {"profile": profile_data, "notes": notes}


def run() -> None:
    uvicorn.run(
        "personal_chatbot.web:app",
        host=settings.web_host,
        port=settings.web_port,
        reload=False,
    )


if __name__ == "__main__":
    run()
