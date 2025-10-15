using Ecommerce.Api.Contracts;

namespace Ecommerce.Api.Services;

/// <summary>
/// Interface for category service operations
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Gets all categories with pagination
    /// </summary>
    /// <param name="request">Pagination request parameters</param>
    /// <returns>Paginated list of categories</returns>
    Task<Paged<CategoryListItemDto>> GetAllAsync(PagedRequest request);

    /// <summary>
    /// Gets all categories without pagination
    /// </summary>
    /// <returns>List of all categories</returns>
    Task<IEnumerable<CategoryListItemDto>> GetAllSimpleAsync();

    /// <summary>
    /// Gets a category by ID
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>Category details or null if not found</returns>
    Task<CategoryDto?> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new category
    /// </summary>
    /// <param name="dto">Category creation data</param>
    /// <returns>Created category details</returns>
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto);

    /// <summary>
    /// Updates an existing category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="dto">Category update data</param>
    /// <returns>Updated category details</returns>
    Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto);

    /// <summary>
    /// Soft deletes a category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Restores a soft-deleted category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>True if restored successfully</returns>
    Task<bool> RestoreAsync(int id);

    /// <summary>
    /// Checks if a category name already exists
    /// </summary>
    /// <param name="name">Category name to check</param>
    /// <param name="excludeId">Category ID to exclude from check (for updates)</param>
    /// <returns>True if name exists</returns>
    Task<bool> NameExistsAsync(string name, int? excludeId = null);
}
