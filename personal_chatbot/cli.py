from __future__ import annotations

from .chat import Chatbot, new_session_id
from .config import load_settings


def main() -> None:
    settings = load_settings()
    bot = Chatbot(settings)
    session_id = new_session_id()

    print("Personal Chatbot (CLI)")
    print("Commands: /new (new session), /exit")

    while True:
        try:
            user_input = input("You> ").strip()
        except (EOFError, KeyboardInterrupt):
            print("\nBye.")
            break

        if not user_input:
            continue
        if user_input.lower() in {"/exit", "exit", "quit"}:
            print("Bye.")
            break
        if user_input.lower() == "/new":
            session_id = new_session_id()
            print("Started a new session.")
            continue

        response = bot.chat(session_id, user_input)
        print(f"Bot> {response}")


if __name__ == "__main__":
    main()
