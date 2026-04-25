import { apiRequest, debounce, escapeHtml, formatCurrency, formatDate, paymentMethods, showToast, statusTone } from "/assets/common.js";

const state = {
    categories: [],
    lowStock: [],
    productQuery: "",
    products: [],
    sales: [],
    salesFilters: {
        status: "",
        customerName: ""
    },
    selectedSale: null,
    summary: null
};

const elements = {
    categoryDescription: document.querySelector("#category-description"),
    categoryForm: document.querySelector("#category-form"),
    categoryId: document.querySelector("#category-id"),
    categoryList: document.querySelector("#category-list"),
    categoryName: document.querySelector("#category-name"),
    lowStockList: document.querySelector("#low-stock-list"),
    productActive: document.querySelector("#product-active"),
    productCategory: document.querySelector("#product-category"),
    productDescription: document.querySelector("#product-description"),
    productForm: document.querySelector("#product-form"),
    productId: document.querySelector("#product-id"),
    productName: document.querySelector("#product-name"),
    productPrice: document.querySelector("#product-price"),
    productSearch: document.querySelector("#product-search"),
    productSku: document.querySelector("#product-sku"),
    productStock: document.querySelector("#product-stock"),
    productTable: document.querySelector("#product-table"),
    refreshAdmin: document.querySelector("#refresh-admin"),
    resetCategoryForm: document.querySelector("#reset-category-form"),
    resetProductForm: document.querySelector("#reset-product-form"),
    saleDetail: document.querySelector("#sale-detail"),
    salesSearch: document.querySelector("#sales-search"),
    salesStatus: document.querySelector("#sales-status"),
    salesTable: document.querySelector("#sales-table"),
    summaryGrid: document.querySelector("#summary-grid")
};

document.addEventListener("DOMContentLoaded", initialize);

async function initialize() {
    bindEvents();
    await refreshDashboard();
}

function bindEvents() {
    elements.refreshAdmin.addEventListener("click", () => refreshDashboard());
    elements.resetCategoryForm.addEventListener("click", resetCategoryForm);
    elements.resetProductForm.addEventListener("click", resetProductForm);

    elements.categoryForm.addEventListener("submit", handleCategorySubmit);
    elements.productForm.addEventListener("submit", handleProductSubmit);

    elements.productSearch.addEventListener("input", debounce(event => {
        state.productQuery = event.target.value.trim().toLowerCase();
        renderProducts();
    }, 180));

    elements.salesStatus.addEventListener("change", async event => {
        state.salesFilters.status = event.target.value;
        await loadSales();
    });

    elements.salesSearch.addEventListener("input", debounce(async event => {
        state.salesFilters.customerName = event.target.value.trim();
        await loadSales();
    }, 250));

    elements.productTable.addEventListener("click", handleProductTableClick);
    elements.categoryList.addEventListener("click", handleCategoryListClick);
    elements.salesTable.addEventListener("click", handleSalesTableClick);
    elements.saleDetail.addEventListener("click", handleSaleDetailClick);
    elements.saleDetail.addEventListener("submit", handleSaleDetailSubmit);
}

async function refreshDashboard() {
    try {
        await Promise.all([loadCategories(), loadProducts(), loadLowStock(), loadSales(), loadSummary()]);
        renderSummary();
        renderCategories();
        renderProducts();
        renderLowStock();
        renderSaleDetail();
    }
    catch (error) {
        showToast(error.message, "danger");
    }
}

async function loadCategories() {
    state.categories = await apiRequest("/api/categories/simple");
    elements.productCategory.innerHTML = [
        `<option value="">Select a category</option>`,
        ...state.categories.map(category => `<option value="${category.id}">${escapeHtml(category.name)}</option>`)
    ].join("");
}

async function loadProducts() {
    const response = await apiRequest("/api/products?page=1&pageSize=100&sortBy=Name&sortDirection=asc");
    state.products = response.items ?? [];
}

async function loadLowStock() {
    state.lowStock = await apiRequest("/api/products/low-stock?threshold=10");
}

async function loadSales() {
    const query = new URLSearchParams({
        page: "1",
        pageSize: "100",
        sortBy: "SaleDate",
        sortDirection: "desc"
    });

    if (state.salesFilters.status) {
        query.set("status", state.salesFilters.status);
    }

    if (state.salesFilters.customerName) {
        query.set("customerName", state.salesFilters.customerName);
    }

    const response = await apiRequest(`/api/sales?${query.toString()}`);
    state.sales = response.items ?? [];
    renderSales();
}

async function loadSummary() {
    state.summary = await apiRequest("/api/sales/summary");
}

function renderSummary() {
    const pendingSales = state.summary?.statusBreakdown?.find(item => item.status === "Pending")?.count ?? 0;

    elements.summaryGrid.innerHTML = `
        <div class="stat-card">
            <div class="stat-label">Revenue</div>
            <div class="stat-value">${formatCurrency(state.summary?.totalRevenue ?? 0)}</div>
        </div>
        <div class="stat-card">
            <div class="stat-label">Total sales</div>
            <div class="stat-value">${state.summary?.totalSales ?? 0}</div>
        </div>
        <div class="stat-card">
            <div class="stat-label">Active products</div>
            <div class="stat-value">${state.products.length}</div>
        </div>
        <div class="stat-card">
            <div class="stat-label">Pending orders</div>
            <div class="stat-value">${pendingSales}</div>
        </div>
    `;
}

function renderCategories() {
    if (!state.categories.length) {
        elements.categoryList.innerHTML = `<div class="empty-state">No categories are available yet.</div>`;
        return;
    }

    elements.categoryList.innerHTML = state.categories.map(category => `
        <article class="inline-card">
            <div class="line-between">
                <div>
                    <strong>${escapeHtml(category.name)}</strong>
                    <div class="detail-copy">${escapeHtml(category.description || "No description")}</div>
                </div>
                <span class="status-pill tone-neutral">${category.productCount} products</span>
            </div>
            <div class="table-actions">
                <button class="button-ghost" type="button" data-category-edit="${category.id}">Edit</button>
                <button class="button-ghost" type="button" data-category-delete="${category.id}">Archive</button>
            </div>
        </article>
    `).join("");
}

function renderProducts() {
    const filteredProducts = state.products.filter(product => {
        if (!state.productQuery) {
            return true;
        }

        const haystack = `${product.name} ${product.sku} ${product.categoryName}`.toLowerCase();
        return haystack.includes(state.productQuery);
    });

    if (!filteredProducts.length) {
        elements.productTable.innerHTML = `<div class="empty-state">No products matched the current filter.</div>`;
        return;
    }

    elements.productTable.innerHTML = `
        <table>
            <thead>
                <tr>
                    <th>Product</th>
                    <th>Category</th>
                    <th>Price</th>
                    <th>Stock</th>
                    <th>Status</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                ${filteredProducts.map(product => `
                    <tr>
                        <td>
                            <strong>${escapeHtml(product.name)}</strong>
                            <div class="meta">${escapeHtml(product.sku)}</div>
                        </td>
                        <td>${escapeHtml(product.categoryName)}</td>
                        <td>${formatCurrency(product.price)}</td>
                        <td>${product.stockQuantity}</td>
                        <td>
                            <span class="status-pill ${product.isActive ? "tone-success" : "tone-neutral"}">
                                ${product.isActive ? "Active" : "Inactive"}
                            </span>
                        </td>
                        <td>
                            <div class="table-actions">
                                <button class="button-ghost" type="button" data-product-edit="${product.id}">Edit</button>
                                <button class="button-ghost" type="button" data-product-delete="${product.id}">Archive</button>
                            </div>
                        </td>
                    </tr>
                `).join("")}
            </tbody>
        </table>
    `;
}

function renderLowStock() {
    if (!state.lowStock.length) {
        elements.lowStockList.innerHTML = `<div class="empty-state">No low-stock products right now.</div>`;
        return;
    }

    elements.lowStockList.innerHTML = state.lowStock.map(product => `
        <article class="list-item">
            <strong>${escapeHtml(product.name)}</strong>
            <div class="line-between">
                <span>${escapeHtml(product.categoryName)}</span>
                <span class="status-pill tone-warning">${product.stockQuantity} left</span>
            </div>
        </article>
    `).join("");
}

function renderSales() {
    if (!state.sales.length) {
        elements.salesTable.innerHTML = `<div class="empty-state">No sales matched the current filters.</div>`;
        return;
    }

    elements.salesTable.innerHTML = `
        <table>
            <thead>
                <tr>
                    <th>Sale</th>
                    <th>Customer</th>
                    <th>Status</th>
                    <th>Amount</th>
                    <th>Placed</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
                ${state.sales.map(sale => `
                    <tr>
                        <td>
                            <strong>${escapeHtml(sale.saleNumber)}</strong>
                            <div class="meta">${sale.itemCount} items</div>
                        </td>
                        <td>${escapeHtml(sale.customerName || "Guest checkout")}</td>
                        <td>
                            <span class="status-pill tone-${statusTone(sale.status)}">${escapeHtml(sale.status)}</span>
                        </td>
                        <td>${formatCurrency(sale.finalAmount)}</td>
                        <td>${formatDate(sale.saleDate)}</td>
                        <td>
                            <button class="button-ghost" type="button" data-sale-view="${sale.id}">View</button>
                        </td>
                    </tr>
                `).join("")}
            </tbody>
        </table>
    `;
}

function renderSaleDetail() {
    if (!state.selectedSale) {
        elements.saleDetail.innerHTML = `
            <div class="empty-state">
                Select an order from the table to inspect items, add payment, or complete the sale.
            </div>
        `;
        return;
    }

    elements.saleDetail.innerHTML = `
        <div class="stack">
            <article class="detail-block selection-card">
                <div class="line-between">
                    <div>
                        <strong>${escapeHtml(state.selectedSale.saleNumber)}</strong>
                        <div class="detail-copy">${escapeHtml(state.selectedSale.customerName || "Guest checkout")}</div>
                    </div>
                    <span class="status-pill tone-${statusTone(state.selectedSale.status)}">
                        ${escapeHtml(state.selectedSale.status)}
                    </span>
                </div>
                <div class="split-list">
                    <div class="line-between">
                        <span>Final amount</span>
                        <strong>${formatCurrency(state.selectedSale.finalAmount)}</strong>
                    </div>
                    <div class="line-between">
                        <span>Placed</span>
                        <strong>${formatDate(state.selectedSale.saleDate)}</strong>
                    </div>
                    <div class="line-between">
                        <span>Email</span>
                        <strong>${escapeHtml(state.selectedSale.customerEmail || "n/a")}</strong>
                    </div>
                </div>
                ${state.selectedSale.notes ? `<div class="detail-copy">${escapeHtml(state.selectedSale.notes)}</div>` : ""}
                ${state.selectedSale.status === "Pending" ? `
                    <div class="detail-actions">
                        <button class="button" type="button" data-sale-complete="${state.selectedSale.id}">Complete order</button>
                        <button class="button-danger button" type="button" data-sale-cancel="${state.selectedSale.id}">Cancel order</button>
                    </div>
                ` : ""}
            </article>

            <article class="detail-block">
                <strong>Items</strong>
                <div class="line-list">
                    ${state.selectedSale.saleItems.map(item => `
                        <div class="inline-card">
                            <strong>${escapeHtml(item.product.name)}</strong>
                            <div class="line-between">
                                <span>${item.quantity} x ${formatCurrency(item.unitPrice)}</span>
                                <strong>${formatCurrency(item.totalPrice)}</strong>
                            </div>
                        </div>
                    `).join("")}
                </div>
            </article>

            ${state.selectedSale.paymentInfo ? `
                <article class="detail-block">
                    <strong>Payment</strong>
                    <div class="split-list">
                        <div class="line-between">
                            <span>Method</span>
                            <strong>${escapeHtml(state.selectedSale.paymentInfo.paymentMethod)}</strong>
                        </div>
                        <div class="line-between">
                            <span>Status</span>
                            <strong>${escapeHtml(state.selectedSale.paymentInfo.status)}</strong>
                        </div>
                        <div class="line-between">
                            <span>Processed</span>
                            <strong>${formatDate(state.selectedSale.paymentInfo.processedAt)}</strong>
                        </div>
                    </div>
                </article>
            ` : `
                <article class="detail-block">
                    <strong>Add payment</strong>
                    <form id="sale-payment-form" class="stack">
                        <div class="field">
                            <label for="sale-payment-method">Method</label>
                            <select id="sale-payment-method" name="paymentMethod">
                                ${paymentMethods.map(method => `<option value="${method}">${formatMethod(method)}</option>`).join("")}
                            </select>
                        </div>
                        <div class="field">
                            <label for="sale-payment-reference">Reference</label>
                            <input id="sale-payment-reference" name="paymentReference" type="text" maxlength="50" placeholder="MANUAL-12345">
                        </div>
                        <div class="field">
                            <label for="sale-payment-notes">Notes</label>
                            <textarea id="sale-payment-notes" name="notes" maxlength="500"></textarea>
                        </div>
                        <button class="button" type="submit">Record payment</button>
                    </form>
                </article>
            `}
        </div>
    `;
}

async function handleCategorySubmit(event) {
    event.preventDefault();

    try {
        const payload = {
            name: elements.categoryName.value.trim(),
            description: elements.categoryDescription.value.trim() || null
        };

        const categoryId = elements.categoryId.value;
        await apiRequest(categoryId ? `/api/categories/${categoryId}` : "/api/categories", {
            method: categoryId ? "PUT" : "POST",
            body: payload
        });

        resetCategoryForm();
        await refreshDashboard();
        showToast(categoryId ? "Category updated." : "Category created.", "success");
    }
    catch (error) {
        showToast(error.message, "danger");
    }
}

async function handleProductSubmit(event) {
    event.preventDefault();

    try {
        const payload = {
            name: elements.productName.value.trim(),
            description: elements.productDescription.value.trim() || null,
            price: Number(elements.productPrice.value),
            sku: elements.productSku.value.trim(),
            stockQuantity: Number(elements.productStock.value),
            isActive: elements.productActive.checked,
            categoryId: Number(elements.productCategory.value)
        };

        const productId = elements.productId.value;
        await apiRequest(productId ? `/api/products/${productId}` : "/api/products", {
            method: productId ? "PUT" : "POST",
            body: payload
        });

        resetProductForm();
        await refreshDashboard();
        showToast(productId ? "Product updated." : "Product created.", "success");
    }
    catch (error) {
        showToast(error.message, "danger");
    }
}

async function handleProductTableClick(event) {
    const editTrigger = event.target.closest("[data-product-edit]");
    if (editTrigger) {
        const product = await apiRequest(`/api/products/${editTrigger.dataset.productEdit}`);
        populateProductForm(product);
        return;
    }

    const deleteTrigger = event.target.closest("[data-product-delete]");
    if (deleteTrigger) {
        try {
            await apiRequest(`/api/products/${deleteTrigger.dataset.productDelete}`, { method: "DELETE" });
            await refreshDashboard();
            showToast("Product archived.", "success");
        }
        catch (error) {
            showToast(error.message, "danger");
        }
    }
}

async function handleCategoryListClick(event) {
    const editTrigger = event.target.closest("[data-category-edit]");
    if (editTrigger) {
        const category = state.categories.find(item => item.id === Number(editTrigger.dataset.categoryEdit));
        if (!category) {
            return;
        }

        populateCategoryForm(category);
        return;
    }

    const deleteTrigger = event.target.closest("[data-category-delete]");
    if (deleteTrigger) {
        try {
            await apiRequest(`/api/categories/${deleteTrigger.dataset.categoryDelete}`, { method: "DELETE" });
            await refreshDashboard();
            showToast("Category archived.", "success");
        }
        catch (error) {
            showToast(error.message, "danger");
        }
    }
}

async function handleSalesTableClick(event) {
    const viewTrigger = event.target.closest("[data-sale-view]");
    if (!viewTrigger) {
        return;
    }

    try {
        state.selectedSale = await apiRequest(`/api/sales/${viewTrigger.dataset.saleView}`);
        renderSaleDetail();
    }
    catch (error) {
        showToast(error.message, "danger");
    }
}

async function handleSaleDetailClick(event) {
    const completeTrigger = event.target.closest("[data-sale-complete]");
    if (completeTrigger) {
        try {
            await apiRequest(`/api/sales/${completeTrigger.dataset.saleComplete}/complete`, { method: "POST" });
            state.selectedSale = await apiRequest(`/api/sales/${completeTrigger.dataset.saleComplete}`);
            await Promise.all([loadSales(), loadSummary(), loadProducts(), loadLowStock()]);
            renderSummary();
            renderProducts();
            renderLowStock();
            renderSaleDetail();
            showToast("Sale completed.", "success");
        }
        catch (error) {
            showToast(error.message, "danger");
        }
        return;
    }

    const cancelTrigger = event.target.closest("[data-sale-cancel]");
    if (!cancelTrigger) {
        return;
    }

    try {
        await apiRequest(`/api/sales/${cancelTrigger.dataset.saleCancel}/cancel`, { method: "POST" });
        state.selectedSale = await apiRequest(`/api/sales/${cancelTrigger.dataset.saleCancel}`);
        await Promise.all([loadSales(), loadSummary()]);
        renderSummary();
        renderSaleDetail();
        showToast("Sale cancelled.", "success");
    }
    catch (error) {
        showToast(error.message, "danger");
    }
}

async function handleSaleDetailSubmit(event) {
    if (event.target.id !== "sale-payment-form" || !state.selectedSale) {
        return;
    }

    event.preventDefault();

    try {
        const formData = new FormData(event.target);
        await apiRequest(`/api/sales/${state.selectedSale.id}/payment`, {
            method: "POST",
            body: {
                paymentMethod: String(formData.get("paymentMethod") ?? ""),
                amount: state.selectedSale.finalAmount,
                paymentReference: String(formData.get("paymentReference") ?? "").trim() || null,
                notes: String(formData.get("notes") ?? "").trim() || null
            }
        });

        state.selectedSale = await apiRequest(`/api/sales/${state.selectedSale.id}`);
        renderSaleDetail();
        showToast("Payment recorded.", "success");
    }
    catch (error) {
        showToast(error.message, "danger");
    }
}

function populateCategoryForm(category) {
    elements.categoryId.value = String(category.id);
    elements.categoryName.value = category.name;
    elements.categoryDescription.value = category.description ?? "";
}

function resetCategoryForm() {
    elements.categoryId.value = "";
    elements.categoryForm.reset();
}

function populateProductForm(product) {
    elements.productId.value = String(product.id);
    elements.productName.value = product.name;
    elements.productDescription.value = product.description ?? "";
    elements.productPrice.value = String(product.price);
    elements.productSku.value = product.sku;
    elements.productStock.value = String(product.stockQuantity);
    elements.productCategory.value = String(product.category.id);
    elements.productActive.checked = product.isActive;
}

function resetProductForm() {
    elements.productId.value = "";
    elements.productForm.reset();
    elements.productActive.checked = true;
}

function formatMethod(method) {
    return method.replace(/([a-z])([A-Z])/g, "$1 $2");
}
