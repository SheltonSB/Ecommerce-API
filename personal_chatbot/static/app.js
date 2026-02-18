const messagesEl = document.getElementById("messages");
const inputEl = document.getElementById("input");
const sendBtn = document.getElementById("send");
const newSessionBtn = document.getElementById("new-session");
const memoryBtn = document.getElementById("show-memory");
const memoryPanel = document.getElementById("memory-panel");
const memoryContent = document.getElementById("memory-content");

let sessionId = localStorage.getItem("session_id");

function createSession() {
  sessionId = crypto.randomUUID();
  localStorage.setItem("session_id", sessionId);
  messagesEl.innerHTML = "";
}

async function loadHistory() {
  if (!sessionId) {
    createSession();
    return;
  }
  try {
    const res = await fetch(`/api/history?session_id=${sessionId}`);
    const data = await res.json();
    messagesEl.innerHTML = "";
    data.messages.forEach((msg) => appendMessage(msg.role, msg.content));
  } catch (err) {
    console.error(err);
  }
}

function appendMessage(role, text) {
  const div = document.createElement("div");
  div.classList.add("message", role === "user" ? "user" : "bot");
  div.textContent = text;
  messagesEl.appendChild(div);
  messagesEl.scrollTop = messagesEl.scrollHeight;
}

async function sendMessage() {
  const text = inputEl.value.trim();
  if (!text) return;
  appendMessage("user", text);
  inputEl.value = "";

  const thinking = document.createElement("div");
  thinking.classList.add("message", "bot");
  thinking.textContent = "Thinking...";
  messagesEl.appendChild(thinking);
  messagesEl.scrollTop = messagesEl.scrollHeight;

  try {
    const res = await fetch("/api/chat", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ message: text, session_id: sessionId }),
    });
    const data = await res.json();
    sessionId = data.session_id;
    localStorage.setItem("session_id", sessionId);
    thinking.remove();
    appendMessage("assistant", data.response);
  } catch (err) {
    thinking.textContent = "Error contacting server.";
    console.error(err);
  }
}

async function toggleMemory() {
  memoryPanel.classList.toggle("hidden");
  if (memoryPanel.classList.contains("hidden")) {
    return;
  }
  try {
    const res = await fetch("/api/profile");
    const data = await res.json();
    const profile = data.profile || {};
    const notes = data.notes || [];

    const lines = [];
    if (Object.keys(profile).length) {
      lines.push("Profile:");
      Object.entries(profile).forEach(([key, value]) => {
        lines.push(`- ${key.replace(/_/g, " ")}: ${value}`);
      });
    }
    if (notes.length) {
      lines.push("Notes:");
      notes.forEach((note) => lines.push(`- ${note}`));
    }
    if (!lines.length) {
      lines.push("No memory saved yet.");
    }
    memoryContent.textContent = lines.join("\n");
  } catch (err) {
    memoryContent.textContent = "Unable to load memory.";
  }
}

sendBtn.addEventListener("click", sendMessage);
newSessionBtn.addEventListener("click", () => {
  createSession();
  appendMessage("assistant", "New session started. How can I help?");
});
memoryBtn.addEventListener("click", toggleMemory);

inputEl.addEventListener("keydown", (event) => {
  if (event.key === "Enter" && !event.shiftKey) {
    event.preventDefault();
    sendMessage();
  }
});

if (!sessionId) {
  createSession();
}
loadHistory();
