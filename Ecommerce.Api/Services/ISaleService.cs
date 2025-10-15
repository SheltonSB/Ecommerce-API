using Ecommerce.Api.Contracts;

namespace Ecommerce.Api.Services;

/// <summary>
/// Interface for sale service operations
/// </summary>
public interface ISaleService
{
    /// <summary>
    /// Gets all sales with pagination and filtering
    /// </summary>
    /// <param name="request">Pagination request parameters</param>
    /// <param name="status">Optional status filter</param>
    /// <param name="customerName">Optional customer name filter</param>
    /// <returns>Paginated list of sales</returns>
    Task<Paged<SaleListItemDto>> GetAllAsync(PagedRequest request, string? status = null, string? customerName = null);

    /// <summary>
    /// Gets a sale by ID
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <returns>Sale details or null if not found</returns>
    Task<SaleDto?> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new sale
    /// </summary>
    /// <param name="dto">Sale creation data</param>
    /// <returns>Created sale details</returns>
    Task<SaleDto> CreateAsync(CreateSaleDto dto);

    /// <summary>
    /// Updates an existing sale
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <param name="dto">Sale update data</param>
    /// <returns>Updated sale details</returns>
    Task<SaleDto> UpdateAsync(int id, UpdateSaleDto dto);

    /// <summary>
    /// Completes a sale
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <returns>True if completed successfully</returns>
    Task<bool> CompleteAsync(int id);

    /// <summary>
    /// Cancels a sale
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <returns>True if cancelled successfully</returns>
    Task<bool> CancelAsync(int id);

    /// <summary>
    /// Adds payment information to a sale
    /// </summary>
    /// <param name="saleId">Sale ID</param>
    /// <param name="dto">Payment information</param>
    /// <returns>True if added successfully</returns>
    Task<bool> AddPaymentAsync(int saleId, CreatePaymentInfoDto dto);

    /// <summary>
    /// Gets sales within a date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of sales in the date range</returns>
    Task<IEnumerable<SaleListItemDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets sales summary statistics
    /// </summary>
    /// <returns>Sales summary data</returns>
    Task<object> GetSalesSummaryAsync();
}
