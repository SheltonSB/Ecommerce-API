import { apiRequest, debounce, escapeHtml, formatCurrency, paymentMethods, showToast } from "/assets/common.js";

const cartStorageKey = "northstar-cart";

const state = {
    categories: [],
    products: [],
    cart: loadCart(),
    filters: {
        searchTerm: "",
        categoryId: "",
        sortBy: "Name",
        sortDirection: "asc"
    }
};

const elements = {
    categoryChips: document.querySelector("#category-chips"),
    categoryFilter: document.querySelector("#category-filter"),
    checkoutButton: document.querySelector("#checkout-button"),
    checkoutForm: document.querySelector("#checkout-form"),
    checkoutStatus: document.querySelector("#checkout-status"),
    paymentMethod: document.querySelector("#payment-method"),
    productGrid: document.querySelector("#product-grid"),
    refreshProducts: document.querySelector("#refresh-products"),
    searchInput: document.querySelector("#search-input"),
    sortFilter: document.querySelector("#sort-filter"),
    stats: document.querySelector("#storefront-stats"),
    cartItems: document.querySelector("#cart-items")
};

document.addEventListener("DOMContentLoaded", initialize);

async function initialize() {
    elements.paymentMethod.innerHTML = paymentMethods
        .map(method => `<option value="${method}">${formatMethod(method)}</option>`)
        .join("");

    bindEvents();
    await Promise.all([loadCategories(), loadProducts()]);
    renderCart();
}

function bindEvents() {
    elements.refreshProducts.addEventListener("click", () => loadProducts());
    elements.categoryFilter.addEventListener("change", async event => {
        state.filters.categoryId = event.target.value;
        renderCategoryChips();
        await loadProducts();
    });

    elements.sortFilter.addEventListener("change", async event => {
        const [sortBy, sortDirection] = event.target.value.split("|");
        state.filters.sortBy = sortBy;
        state.filters.sortDirection = sortDirection;
        await loadProducts();
    });

    elements.searchInput.addEventListener("input", debounce(async event => {
        state.filters.searchTerm = event.target.value.trim();
        await loadProducts();
    }, 280));

    elements.categoryChips.addEventListener("click", async event => {
        const trigger = event.target.closest("[data-category-id]");
        if (!trigger) {
            return;
        }

        state.filters.categoryId = trigger.dataset.categoryId ?? "";
        elements.categoryFilter.value = state.filters.categoryId;
        renderCategoryChips();
        await loadProducts();
    });

    elements.productGrid.addEventListener("click", event => {
        const trigger = event.target.closest("[data-add-to-cart]");
        if (!trigger) {
            return;
        }

        const productId = Number(trigger.dataset.addToCart);
        const product = state.products.find(item => item.id === productId);
        if (!product) {
            return;
        }

        addToCart(product);
    });

    elements.cartItems.addEventListener("click", event => {
        const decrementTrigger = event.target.closest("[data-cart-decrement]");
        if (decrementTrigger) {
            updateCartQuantity(Number(decrementTrigger.dataset.cartDecrement), -1);
            return;
        }

        const incrementTrigger = event.target.closest("[data-cart-increment]");
        if (incrementTrigger) {
            updateCartQuantity(Number(incrementTrigger.dataset.cartIncrement), 1);
            return;
        }
    });

    elements.checkoutForm.addEventListener("submit", handleCheckout);
}

async function loadCategories() {
    const categories = await apiRequest("/api/categories/simple");
    state.categories = categories;

    elements.categoryFilter.innerHTML = `
        <option value="">All categories</option>
        ${categories.map(category => `<option value="${category.id}">${escapeHtml(category.name)}</option>`).join("")}
    `;
    elements.categoryFilter.value = state.filters.categoryId;

    renderCategoryChips();
    renderStats();
}

async function loadProducts() {
    elements.productGrid.innerHTML = `<div class="empty-state">Loading products...</div>`;

    try {
        const query = new URLSearchParams({
            page: "1",
            pageSize: "100",
            isActive: "true",
            sortBy: state.filters.sortBy,
            sortDirection: state.filters.sortDirection
        });

        if (state.filters.searchTerm) {
            query.set("searchTerm", state.filters.searchTerm);
        }

        if (state.filters.categoryId) {
            query.set("categoryId", state.filters.categoryId);
        }

        const response = await apiRequest(`/api/products?${query.toString()}`);
        state.products = response.items ?? [];
        syncCartWithCatalog();
        renderProducts();
        renderStats();
    }
    catch (error) {
        elements.productGrid.innerHTML = `<div class="empty-state">${escapeHtml(error.message)}</div>`;
        showToast(error.message, "danger");
    }
}

function renderStats() {
    const itemCount = state.products.length;
    const inCart = state.cart.reduce((sum, item) => sum + item.quantity, 0);
    const totalCategories = state.categories.length;

    elements.stats.innerHTML = `
        <div class="stat-card">
            <div class="stat-label">Visible products</div>
            <div class="stat-value">${itemCount}</div>
        </div>
        <div class="stat-card">
            <div class="stat-label">Categories</div>
            <div class="stat-value">${totalCategories}</div>
        </div>
        <div class="stat-card">
            <div class="stat-label">Items in cart</div>
            <div class="stat-value">${inCart}</div>
        </div>
    `;
}

function renderCategoryChips() {
    const chips = [
        `<button class="chip ${state.filters.categoryId ? "" : "active"}" type="button" data-category-id="">All</button>`,
        ...state.categories.map(category => `
            <button
                class="chip ${String(category.id) === state.filters.categoryId ? "active" : ""}"
                type="button"
                data-category-id="${category.id}">
                ${escapeHtml(category.name)}
                <span class="meta">${category.productCount}</span>
            </button>
        `)
    ];

    elements.categoryChips.innerHTML = chips.join("");
}

function renderProducts() {
    if (!state.products.length) {
        elements.productGrid.innerHTML = `
            <div class="empty-state">
                No products matched the current filters. Try a broader search or switch categories.
            </div>
        `;
        return;
    }

    elements.productGrid.innerHTML = state.products.map(product => `
        <article class="product-card">
            <header>
                <div>
                    <div class="eyebrow">${escapeHtml(product.categoryName)}</div>
                    <h3>${escapeHtml(product.name)}</h3>
                </div>
                <span class="status-pill ${product.stockQuantity > 10 ? "tone-success" : "tone-warning"}">
                    ${product.stockQuantity} in stock
                </span>
            </header>
            <p>${escapeHtml(product.sku)}</p>
            <div class="price-row">
                <div class="price-tag">${formatCurrency(product.price)}</div>
                <button
                    class="button"
                    type="button"
                    data-add-to-cart="${product.id}"
                    ${product.stockQuantity < 1 ? "disabled" : ""}>
                    ${product.stockQuantity < 1 ? "Unavailable" : "Add to cart"}
                </button>
            </div>
        </article>
    `).join("");
}

function renderCart() {
    if (!state.cart.length) {
        elements.cartItems.innerHTML = `
            <div class="empty-state">
                Your cart is empty. Add a product from the catalog to begin checkout.
            </div>
        `;
        return;
    }

    const subtotal = getSubtotal();

    elements.cartItems.innerHTML = `
        ${state.cart.map(item => `
            <article class="cart-item">
                <div class="cart-item-top">
                    <div>
                        <strong>${escapeHtml(item.name)}</strong>
                        <div class="meta">${escapeHtml(item.sku)}</div>
                    </div>
                    <strong>${formatCurrency(item.price * item.quantity)}</strong>
                </div>
                <div class="line-between">
                    <div class="qty-controls">
                        <button type="button" data-cart-decrement="${item.productId}">-</button>
                        <span>${item.quantity}</span>
                        <button type="button" data-cart-increment="${item.productId}">+</button>
                    </div>
                    <span class="meta">${formatCurrency(item.price)} each</span>
                </div>
            </article>
        `).join("")}
        <div class="detail-block">
            <div class="split-list">
                <div class="line-between">
                    <span>Subtotal</span>
                    <strong>${formatCurrency(subtotal)}</strong>
                </div>
                <div class="line-between">
                    <span>Estimated tax</span>
                    <strong>${formatCurrency(0)}</strong>
                </div>
                <div class="line-between">
                    <span>Order total</span>
                    <strong>${formatCurrency(subtotal)}</strong>
                </div>
            </div>
        </div>
    `;
}

function addToCart(product) {
    const existing = state.cart.find(item => item.productId === product.id);
    if (existing) {
        if (existing.quantity >= product.stockQuantity) {
            showToast("Cart quantity already matches available stock.", "warning");
            return;
        }

        existing.quantity += 1;
    }
    else {
        state.cart.push({
            productId: product.id,
            name: product.name,
            price: product.price,
            quantity: 1,
            sku: product.sku,
            stockQuantity: product.stockQuantity
        });
    }

    persistCart();
    renderCart();
    renderStats();
    showToast(`${product.name} added to cart.`, "success");
}

function updateCartQuantity(productId, delta) {
    const item = state.cart.find(entry => entry.productId === productId);
    if (!item) {
        return;
    }

    const nextQuantity = item.quantity + delta;
    if (nextQuantity < 1) {
        state.cart = state.cart.filter(entry => entry.productId !== productId);
    }
    else if (nextQuantity <= item.stockQuantity) {
        item.quantity = nextQuantity;
    }
    else {
        showToast("No more stock is available for that item.", "warning");
    }

    persistCart();
    renderCart();
    renderStats();
}

async function handleCheckout(event) {
    event.preventDefault();

    if (!state.cart.length) {
        setCheckoutStatus("Add at least one product before checking out.", "warning");
        return;
    }

    const formData = new FormData(elements.checkoutForm);
    const customerName = String(formData.get("customerName") ?? "").trim();
    const paymentMethod = String(formData.get("paymentMethod") ?? "").trim();

    if (!customerName || !paymentMethod) {
        setCheckoutStatus("Customer name and payment method are required.", "warning");
        return;
    }

    let sale = null;
    elements.checkoutButton.disabled = true;
    elements.checkoutButton.textContent = "Submitting...";

    try {
        sale = await apiRequest("/api/sales", {
            method: "POST",
            body: {
                customerName,
                customerEmail: String(formData.get("customerEmail") ?? "").trim() || null,
                notes: String(formData.get("notes") ?? "").trim() || null,
                taxAmount: 0,
                discountAmount: 0,
                saleItems: state.cart.map(item => ({
                    productId: item.productId,
                    quantity: item.quantity
                }))
            }
        });

        await apiRequest(`/api/sales/${sale.id}/payment`, {
            method: "POST",
            body: {
                paymentMethod,
                amount: sale.finalAmount,
                paymentReference: `WEB-${Date.now()}`,
                notes: "Storefront checkout"
            }
        });

        await apiRequest(`/api/sales/${sale.id}/complete`, {
            method: "POST"
        });

        state.cart = [];
        persistCart();
        renderCart();
        renderStats();
        elements.checkoutForm.reset();
        elements.paymentMethod.value = paymentMethods[0];
        setCheckoutStatus(`Order ${sale.saleNumber} completed successfully.`, "success");
        showToast(`Order ${sale.saleNumber} completed.`, "success");
        await loadProducts();
    }
    catch (error) {
        if (sale?.saleNumber) {
            setCheckoutStatus(`Order ${sale.saleNumber} was created but needs follow-up: ${error.message}`, "warning");
        }
        else {
            setCheckoutStatus(error.message, "error");
        }

        showToast(error.message, "danger");
    }
    finally {
        elements.checkoutButton.disabled = false;
        elements.checkoutButton.textContent = "Place order";
    }
}

function setCheckoutStatus(message, tone) {
    elements.checkoutStatus.textContent = message;
    elements.checkoutStatus.className = `checkout-status visible ${tone}`;
}

function getSubtotal() {
    return state.cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);
}

function syncCartWithCatalog() {
    state.cart = state.cart
        .map(item => {
            const current = state.products.find(product => product.id === item.productId);
            if (!current) {
                return null;
            }

            return {
                ...item,
                name: current.name,
                price: current.price,
                sku: current.sku,
                stockQuantity: current.stockQuantity,
                quantity: Math.min(item.quantity, current.stockQuantity)
            };
        })
        .filter(item => item && item.quantity > 0);

    persistCart();
}

function loadCart() {
    try {
        const raw = window.localStorage.getItem(cartStorageKey);
        return raw ? JSON.parse(raw) : [];
    }
    catch {
        return [];
    }
}

function persistCart() {
    window.localStorage.setItem(cartStorageKey, JSON.stringify(state.cart));
}

function formatMethod(method) {
    return method.replace(/([a-z])([A-Z])/g, "$1 $2");
}
