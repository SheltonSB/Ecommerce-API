using AutoMapper;
using Ecommerce.Api.Contracts;
using Ecommerce.Api.Data;
using Ecommerce.Api.Domain;
using Ecommerce.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Ecommerce.Api.Services;

/// <summary>
/// Service for product operations
/// </summary>
public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;
    private readonly ICacheProvider _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(2);

    public ProductService(AppDbContext context, IMapper mapper, ILogger<ProductService> logger, ICacheProvider cache)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task<Paged<ProductListItemDto>> GetAllAsync(PagedRequest request, int? categoryId = null, string? searchTerm = null, bool? isActive = null)
    {
        request.Validate();

        var cacheKey = $"products:{request.Page}:{request.PageSize}:{categoryId}:{searchTerm}:{isActive}:{request.SortBy}:{request.SortDirection}";
        var cached = await _cache.GetAsync<Paged<ProductListItemDto>>(cacheKey);
        if (cached != null) return cached;

        var query = _context.Products
            .Include(p => p.Category)
            .AsQueryable();

        // Apply filters
        query = query.WhereIf(categoryId.HasValue, p => p.CategoryId == categoryId!.Value);
        if (isActive.HasValue)
        {
            query = isActive.Value 
                ? query.Where(p => p.Status == ProductStatus.Active)
                : query.Where(p => p.Status != ProductStatus.Active);
        }
        query = query.SearchIn(searchTerm, p => p.Name, p => p.Description, p => p.Sku);

        // Apply sorting
        query = query.SortBy(request.SortBy, request.SortDirection);

        // Get total count
        var totalItems = await query.CountAsync();

        // Apply pagination
        var items = await query
            .Paginate(request.Page, request.PageSize)
            .Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Sku = p.Sku,
                StockQuantity = p.StockQuantity,
                IsActive = p.Status == ProductStatus.Active,
                CategoryName = p.Category.Name,
                ImageUrl = p.ImageUrl
            })
            .ToListAsync();

        var result = new Paged<ProductListItemDto>(items, request.Page, request.PageSize, totalItems);
        await _cache.SetAsync(cacheKey, result, CacheDuration, trackKey: true);
        return result;
    }

    /// <inheritdoc />
    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Sku = product.Sku,
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl,
            IsActive = product.Status == ProductStatus.Active,
            Category = new CategoryListItemDto
            {
                Id = product.Category.Id,
                Name = product.Category.Name,
                Description = product.Category.Description,
                ProductCount = await _context.Products.CountAsync(p => p.CategoryId == product.CategoryId && !p.IsDeleted)
            },
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    /// <inheritdoc />
    public async Task<ProductDto?> GetBySkuAsync(string sku)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Sku == sku);

        if (product == null)
            return null;

        return await GetByIdAsync(product.Id);
    }

    /// <inheritdoc />
    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        // Check if SKU already exists
        if (await SkuExistsAsync(dto.Sku))
        {
            throw new InvalidOperationException($"A product with SKU '{dto.Sku}' already exists.");
        }

        // Verify category exists
        var category = await _context.Categories.FindAsync(dto.CategoryId);
        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID {dto.CategoryId} not found.");
        }

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Sku = dto.Sku,
            StockQuantity = dto.StockQuantity,
            // Map Enterprise Fields
            Upc = dto.Upc,
            Gtin = dto.Gtin,
            Isbn = dto.Isbn,
            KeyFeatures = dto.KeyFeatures,
            InventoryLocation = dto.InventoryLocation,
            Weight = dto.Weight,
            Height = dto.Height,
            Width = dto.Width,
            Length = dto.Length,
            IsHazmat = dto.IsHazmat,
            SafetyDataSheetUrl = dto.SafetyDataSheetUrl,
            FloorPrice = dto.FloorPrice,
            CeilingPrice = dto.CeilingPrice,
            Status = ProductStatus.Draft, // Start as Draft by default
            CategoryId = dto.CategoryId
        };

        // Run automated checks
        product.UpdateQualityScore();
        product.RunComplianceCheck();

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        await InvalidateProductCache();

        _logger.LogInformation("Created product {ProductName} with SKU {Sku} and ID {ProductId}", product.Name, product.Sku, product.Id);

        return await GetByIdAsync(product.Id) ?? throw new InvalidOperationException("Failed to retrieve created product");
    }

    /// <inheritdoc />
    public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {id} not found.");
        }

        // Check if SKU already exists (excluding current product)
        if (await SkuExistsAsync(dto.Sku, id))
        {
            throw new InvalidOperationException($"A product with SKU '{dto.Sku}' already exists.");
        }

        // Verify category exists
        var category = await _context.Categories.FindAsync(dto.CategoryId);
        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID {dto.CategoryId} not found.");
        }

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Sku = dto.Sku;
        product.StockQuantity = dto.StockQuantity;
        
        // Map Enterprise Fields
        product.Upc = dto.Upc;
        product.Gtin = dto.Gtin;
        product.Isbn = dto.Isbn;
        product.KeyFeatures = dto.KeyFeatures;
        product.InventoryLocation = dto.InventoryLocation;
        product.Weight = dto.Weight;
        product.Height = dto.Height;
        product.Width = dto.Width;
        product.Length = dto.Length;
        product.IsHazmat = dto.IsHazmat;
        product.SafetyDataSheetUrl = dto.SafetyDataSheetUrl;
        product.FloorPrice = dto.FloorPrice;
        product.CeilingPrice = dto.CeilingPrice;
        
        // If the user requested to activate, we try to set it, but ComplianceCheck might block it
        if (dto.IsActive) product.Status = ProductStatus.Active;
        
        product.CategoryId = dto.CategoryId;
        product.UpdateTimestamp();

        // Run automated checks (might override Status to Blocked)
        product.UpdateQualityScore();
        product.RunComplianceCheck();

        await _context.SaveChangesAsync();
        await InvalidateProductCache();

        _logger.LogInformation("Updated product {ProductName} with SKU {Sku} and ID {ProductId}", product.Name, product.Sku, product.Id);

        return await GetByIdAsync(product.Id) ?? throw new InvalidOperationException("Failed to retrieve updated product");
    }

    /// <inheritdoc />
    public async Task<ProductDto> UpdateStockAsync(int id, UpdateStockDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {id} not found.");
        }

        product.UpdateStock(dto.StockQuantity);
        await _context.SaveChangesAsync();
        await InvalidateProductCache();

        _logger.LogInformation("Updated stock for product {ProductName} (ID: {ProductId}) to {StockQuantity}", product.Name, product.Id, dto.StockQuantity);

        return await GetByIdAsync(product.Id) ?? throw new InvalidOperationException("Failed to retrieve updated product");
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.SaleItems)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return false;

        // Check if product has been sold
        var hasSales = product.SaleItems.Any();
        if (hasSales)
        {
            throw new InvalidOperationException("Cannot delete a product that has been sold.");
        }

        product.MarkAsDeleted();
        await _context.SaveChangesAsync();
        await InvalidateProductCache();

        _logger.LogInformation("Soft deleted product {ProductName} with SKU {Sku} and ID {ProductId}", product.Name, product.Sku, product.Id);

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> RestoreAsync(int id)
    {
        var product = await _context.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted);

        if (product == null)
            return false;

        product.Restore();
        await _context.SaveChangesAsync();
        await InvalidateProductCache();

        _logger.LogInformation("Restored product {ProductName} with SKU {Sku} and ID {ProductId}", product.Name, product.Sku, product.Id);

        return true;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PriceHistoryDto>> GetPriceHistoryAsync(int productId)
    {
        return await _context.PriceHistories
            .Where(ph => ph.ProductId == productId)
            .OrderByDescending(ph => ph.ChangedAt)
            .Select(ph => new PriceHistoryDto
            {
                Id = ph.Id,
                OldPrice = ph.OldPrice,
                NewPrice = ph.NewPrice,
                ChangedAt = ph.ChangedAt,
                PercentageChange = ph.GetPercentageChange(),
                IsPriceIncrease = ph.IsPriceIncrease()
            })
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> SkuExistsAsync(string sku, int? excludeId = null)
    {
        var query = _context.Products.Where(p => p.Sku.ToLower() == sku.ToLower());
        
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    private Task InvalidateProductCache()
    {
        return _cache.RemoveByPrefixAsync("products:");
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProductListItemDto>> GetLowStockProductsAsync(int threshold = 10)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.StockQuantity <= threshold && p.Status == ProductStatus.Active)
            .Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Sku = p.Sku,
                StockQuantity = p.StockQuantity,
                IsActive = p.Status == ProductStatus.Active,
                CategoryName = p.Category.Name
            })
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProductListItemDto>> GetByCategoryAsync(int categoryId)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId && p.Status == ProductStatus.Active)
            .Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Sku = p.Sku,
                StockQuantity = p.StockQuantity,
                IsActive = p.Status == ProductStatus.Active,
                CategoryName = p.Category.Name
            })
            .ToListAsync();
    }
}
