# Personal Chatbot (Python + Llama)

This is a local personal chatbot that runs as:
- CLI
- Web app
- Desktop app (webview)

Memory is stored locally in SQLite under `personal_chatbot/data/chatbot.db`.

## Requirements
- Python 3.10+
- A local Llama backend
  - Recommended: Ollama (easy model management)
  - Alternative: llama.cpp with a local GGUF model

## Setup
```
cd personal_chatbot
python -m venv .venv
source .venv/bin/activate
pip install -r requirements.txt
```

Copy and edit the environment file:
```
cp .env.example .env
```

### Option A: Ollama (recommended)
1. Install Ollama
2. Pull a model (example: `llama3.1`)
3. Set in `.env`:
```
BACKEND=ollama
MODEL_NAME=llama3.1
```

### Option B: llama.cpp (GGUF model)
```
pip install -r requirements-llama-cpp.txt
```
Then set in `.env`:
```
BACKEND=llama_cpp
MODEL_PATH=/absolute/path/to/model.gguf
```

## Run
CLI:
```
python -m personal_chatbot.cli
```

Web app:
```
python -m personal_chatbot.web
```

Desktop app:
```
python -m personal_chatbot.desktop
```

## Notes
- Memory extraction is heuristic by default (`MEMORY_MODE=heuristic`).
- You can reset a session from the web UI using the "New Session" button.
