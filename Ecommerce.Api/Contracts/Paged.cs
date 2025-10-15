namespace Ecommerce.Api.Contracts;

/// <summary>
/// Represents a paginated result set
/// </summary>
/// <typeparam name="T">The type of items in the result set</typeparam>
public class Paged<T>
{
    /// <summary>
    /// The items in the current page
    /// </summary>
    public IEnumerable<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

    /// <summary>
    /// Indicates if there is a previous page
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Indicates if there is a next page
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Creates a new paginated result
    /// </summary>
    /// <param name="items">The items for the current page</param>
    /// <param name="page">Current page number</param>
    /// <param name="pageSize">Items per page</param>
    /// <param name="totalItems">Total items across all pages</param>
    public Paged(IEnumerable<T> items, int page, int pageSize, int totalItems)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalItems = totalItems;
    }

    /// <summary>
    /// Creates an empty paginated result
    /// </summary>
    public Paged()
    {
    }
}

/// <summary>
/// Represents pagination parameters for API requests
/// </summary>
public class PagedRequest
{
    /// <summary>
    /// Page number (1-based, defaults to 1)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of items per page (defaults to 10, max 100)
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Field to sort by
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction (asc or desc)
    /// </summary>
    public string? SortDirection { get; set; } = "asc";

    /// <summary>
    /// Validates and normalizes the pagination parameters
    /// </summary>
    public void Validate()
    {
        if (Page < 1) Page = 1;
        if (PageSize < 1) PageSize = 10;
        if (PageSize > 100) PageSize = 100;
        
        SortDirection = SortDirection?.ToLowerInvariant() switch
        {
            "desc" or "descending" => "desc",
            _ => "asc"
        };
    }
}
