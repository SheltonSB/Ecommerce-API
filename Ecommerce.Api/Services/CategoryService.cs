using AutoMapper;
using Ecommerce.Api.Contracts;
using Ecommerce.Api.Data;
using Ecommerce.Api.Domain;
using Ecommerce.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Services;

/// <summary>
/// Service for category operations
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(AppDbContext context, IMapper mapper, ILogger<CategoryService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Paged<CategoryListItemDto>> GetAllAsync(PagedRequest request)
    {
        request.Validate();

        var query = _context.Categories
            .Include(c => c.Products)
            .AsQueryable();

        // Apply sorting
        query = query.SortBy(request.SortBy, request.SortDirection);

        // Get total count
        var totalItems = await query.CountAsync();

        // Apply pagination
        var items = await query
            .Paginate(request.Page, request.PageSize)
            .Select(c => new CategoryListItemDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ProductCount = c.Products.Count(p => !p.IsDeleted)
            })
            .ToListAsync();

        return new Paged<CategoryListItemDto>(items, request.Page, request.PageSize, totalItems);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CategoryListItemDto>> GetAllSimpleAsync()
    {
        return await _context.Categories
            .Include(c => c.Products)
            .Select(c => new CategoryListItemDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ProductCount = c.Products.Count(p => !p.IsDeleted)
            })
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return null;

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ProductCount = category.Products.Count(p => !p.IsDeleted),
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    /// <inheritdoc />
    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        // Check if name already exists
        if (await NameExistsAsync(dto.Name))
        {
            throw new InvalidOperationException($"A category with the name '{dto.Name}' already exists.");
        }

        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created category {CategoryName} with ID {CategoryId}", category.Name, category.Id);

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ProductCount = 0,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    /// <inheritdoc />
    public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID {id} not found.");
        }

        // Check if name already exists (excluding current category)
        if (await NameExistsAsync(dto.Name, id))
        {
            throw new InvalidOperationException($"A category with the name '{dto.Name}' already exists.");
        }

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.UpdateTimestamp();

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated category {CategoryName} with ID {CategoryId}", category.Name, category.Id);

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ProductCount = await _context.Products.CountAsync(p => p.CategoryId == id && !p.IsDeleted),
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return false;

        // Check if category has active products
        var hasActiveProducts = category.Products.Any(p => !p.IsDeleted);
        if (hasActiveProducts)
        {
            throw new InvalidOperationException("Cannot delete a category that has active products.");
        }

        category.MarkAsDeleted();
        await _context.SaveChangesAsync();

        _logger.LogInformation("Soft deleted category {CategoryName} with ID {CategoryId}", category.Name, category.Id);

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> RestoreAsync(int id)
    {
        var category = await _context.Categories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted);

        if (category == null)
            return false;

        category.Restore();
        await _context.SaveChangesAsync();

        _logger.LogInformation("Restored category {CategoryName} with ID {CategoryId}", category.Name, category.Id);

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
    {
        var query = _context.Categories.Where(c => c.Name.ToLower() == name.ToLower());
        
        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
