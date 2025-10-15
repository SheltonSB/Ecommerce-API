using Ecommerce.Api.Contracts;

namespace Ecommerce.Api.Services;

/// <summary>
/// Interface for product service operations
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Gets all products with pagination and filtering
    /// </summary>
    /// <param name="request">Pagination request parameters</param>
    /// <param name="categoryId">Optional category filter</param>
    /// <param name="searchTerm">Optional search term</param>
    /// <param name="isActive">Optional active status filter</param>
    /// <returns>Paginated list of products</returns>
    Task<Paged<ProductListItemDto>> GetAllAsync(PagedRequest request, int? categoryId = null, string? searchTerm = null, bool? isActive = null);

    /// <summary>
    /// Gets a product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product details or null if not found</returns>
    Task<ProductDto?> GetByIdAsync(int id);

    /// <summary>
    /// Gets a product by SKU
    /// </summary>
    /// <param name="sku">Product SKU</param>
    /// <returns>Product details or null if not found</returns>
    Task<ProductDto?> GetBySkuAsync(string sku);

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="dto">Product creation data</param>
    /// <returns>Created product details</returns>
    Task<ProductDto> CreateAsync(CreateProductDto dto);

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="dto">Product update data</param>
    /// <returns>Updated product details</returns>
    Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto);

    /// <summary>
    /// Updates product stock quantity
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="dto">Stock update data</param>
    /// <returns>Updated product details</returns>
    Task<ProductDto> UpdateStockAsync(int id, UpdateStockDto dto);

    /// <summary>
    /// Soft deletes a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Restores a soft-deleted product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>True if restored successfully</returns>
    Task<bool> RestoreAsync(int id);

    /// <summary>
    /// Gets price history for a product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <returns>List of price history entries</returns>
    Task<IEnumerable<PriceHistoryDto>> GetPriceHistoryAsync(int productId);

    /// <summary>
    /// Checks if a product SKU already exists
    /// </summary>
    /// <param name="sku">Product SKU to check</param>
    /// <param name="excludeId">Product ID to exclude from check (for updates)</param>
    /// <returns>True if SKU exists</returns>
    Task<bool> SkuExistsAsync(string sku, int? excludeId = null);

    /// <summary>
    /// Gets products with low stock
    /// </summary>
    /// <param name="threshold">Stock threshold (default 10)</param>
    /// <returns>List of products with low stock</returns>
    Task<IEnumerable<ProductListItemDto>> GetLowStockProductsAsync(int threshold = 10);

    /// <summary>
    /// Gets products by category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <returns>List of products in the category</returns>
    Task<IEnumerable<ProductListItemDto>> GetByCategoryAsync(int categoryId);
}
