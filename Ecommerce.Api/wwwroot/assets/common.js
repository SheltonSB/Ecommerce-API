const paymentMethods = [
    "CreditCard",
    "DebitCard",
    "PayPal",
    "BankTransfer",
    "Cash",
    "DigitalWallet",
    "Cryptocurrency"
];

export { paymentMethods };

export async function apiRequest(path, options = {}) {
    const { body, headers = {}, ...rest } = options;

    const response = await fetch(path, {
        ...rest,
        headers: body
            ? {
                "Content-Type": "application/json",
                ...headers
            }
            : headers,
        body: body ? JSON.stringify(body) : undefined
    });

    if (response.status === 204) {
        return null;
    }

    const contentType = response.headers.get("content-type") ?? "";
    const payload = contentType.includes("application/json")
        ? await response.json()
        : await response.text();

    if (!response.ok) {
        const message = typeof payload === "string"
            ? payload
            : payload?.details || payload?.message || response.statusText;

        throw new Error(message);
    }

    return payload;
}

export function formatCurrency(value) {
    return new Intl.NumberFormat("en-US", {
        style: "currency",
        currency: "USD",
        maximumFractionDigits: 2
    }).format(Number(value ?? 0));
}

export function formatDate(value) {
    if (!value) {
        return "n/a";
    }

    return new Intl.DateTimeFormat("en-US", {
        dateStyle: "medium",
        timeStyle: "short"
    }).format(new Date(value));
}

export function debounce(callback, delay = 250) {
    let timeoutId = 0;

    return (...args) => {
        window.clearTimeout(timeoutId);
        timeoutId = window.setTimeout(() => callback(...args), delay);
    };
}

export function showToast(message, tone = "info") {
    const container = ensureToastContainer();
    const toast = document.createElement("div");
    toast.className = `toast toast-${tone}`;
    toast.textContent = message;
    container.append(toast);

    window.setTimeout(() => {
        toast.classList.add("toast-leave");
        window.setTimeout(() => toast.remove(), 220);
    }, 3600);
}

export function escapeHtml(value) {
    return String(value ?? "")
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll("\"", "&quot;")
        .replaceAll("'", "&#39;");
}

export function statusTone(status) {
    switch ((status ?? "").toLowerCase()) {
        case "completed":
            return "success";
        case "pending":
            return "warning";
        case "cancelled":
        case "failed":
            return "danger";
        default:
            return "neutral";
    }
}

function ensureToastContainer() {
    let container = document.querySelector("[data-toast-region]");
    if (container) {
        return container;
    }

    container = document.createElement("div");
    container.dataset.toastRegion = "true";
    container.className = "toast-region";
    document.body.append(container);
    return container;
}
