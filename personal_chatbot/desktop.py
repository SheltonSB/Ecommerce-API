from __future__ import annotations

import threading
import time

import webview

from .web import run, settings


def main() -> None:
    server_thread = threading.Thread(target=run, daemon=True)
    server_thread.start()

    url = f"http://{settings.web_host}:{settings.web_port}"
    time.sleep(0.7)
    webview.create_window("Personal Chatbot", url, width=1100, height=760)
    webview.start()


if __name__ == "__main__":
    main()
