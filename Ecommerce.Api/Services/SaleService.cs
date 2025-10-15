using AutoMapper;
using Ecommerce.Api.Contracts;
using Ecommerce.Api.Data;
using Ecommerce.Api.Domain;
using Ecommerce.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Services;

/// <summary>
/// Service for sale operations
/// </summary>
public class SaleService : ISaleService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<SaleService> _logger;

    public SaleService(AppDbContext context, IMapper mapper, ILogger<SaleService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Paged<SaleListItemDto>> GetAllAsync(PagedRequest request, string? status = null, string? customerName = null)
    {
        request.Validate();

        var query = _context.Sales
            .Include(s => s.SaleItems)
            .AsQueryable();

        // Apply filters
        query = query.WhereIf(!string.IsNullOrWhiteSpace(status), s => s.Status.ToString() == status);
        query = query.WhereIf(!string.IsNullOrWhiteSpace(customerName), s => s.CustomerName != null && s.CustomerName.Contains(customerName));

        // Apply sorting
        query = query.SortBy(request.SortBy, request.SortDirection);

        // Get total count
        var totalItems = await query.CountAsync();

        // Apply pagination
        var items = await query
            .Paginate(request.Page, request.PageSize)
            .Select(s => new SaleListItemDto
            {
                Id = s.Id,
                SaleNumber = s.SaleNumber,
                SaleDate = s.SaleDate,
                FinalAmount = s.FinalAmount,
                Status = s.Status.ToString(),
                CustomerName = s.CustomerName,
                ItemCount = s.SaleItems.Count
            })
            .ToListAsync();

        return new Paged<SaleListItemDto>(items, request.Page, request.PageSize, totalItems);
    }

    /// <inheritdoc />
    public async Task<SaleDto?> GetByIdAsync(int id)
    {
        var sale = await _context.Sales
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
            .Include(s => s.PaymentInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sale == null)
            return null;

        return new SaleDto
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            TotalAmount = sale.TotalAmount,
            TaxAmount = sale.TaxAmount,
            DiscountAmount = sale.DiscountAmount,
            FinalAmount = sale.FinalAmount,
            Status = sale.Status.ToString(),
            CustomerName = sale.CustomerName,
            CustomerEmail = sale.CustomerEmail,
            Notes = sale.Notes,
            SaleItems = sale.SaleItems.Select(si => new SaleItemDto
            {
                Id = si.Id,
                Quantity = si.Quantity,
                UnitPrice = si.UnitPrice,
                TotalPrice = si.TotalPrice,
                Product = new ProductListItemDto
                {
                    Id = si.Product.Id,
                    Name = si.Product.Name,
                    Price = si.Product.Price,
                    Sku = si.Product.Sku,
                    StockQuantity = si.Product.StockQuantity,
                    IsActive = si.Product.IsActive,
                    CategoryName = si.Product.Category.Name
                }
            }).ToList(),
            PaymentInfo = sale.PaymentInfo != null ? new PaymentInfoDto
            {
                Id = sale.PaymentInfo.Id,
                PaymentMethod = sale.PaymentInfo.PaymentMethod.ToString(),
                Amount = sale.PaymentInfo.Amount,
                Status = sale.PaymentInfo.Status.ToString(),
                TransactionId = sale.PaymentInfo.TransactionId,
                PaymentReference = sale.PaymentInfo.PaymentReference,
                ProcessedAt = sale.PaymentInfo.ProcessedAt,
                Notes = sale.PaymentInfo.Notes
            } : null,
            CreatedAt = sale.CreatedAt,
            UpdatedAt = sale.UpdatedAt
        };
    }

    /// <inheritdoc />
    public async Task<SaleDto> CreateAsync(CreateSaleDto dto)
    {
        var sale = new Sale
        {
            SaleNumber = $"SALE-{DateTime.UtcNow:yyyyMMdd-HHmmss}",
            CustomerName = dto.CustomerName,
            CustomerEmail = dto.CustomerEmail,
            TaxAmount = dto.TaxAmount,
            DiscountAmount = dto.DiscountAmount,
            Notes = dto.Notes,
            Status = SaleStatus.Pending
        };

        // Add sale items
        foreach (var itemDto in dto.SaleItems)
        {
            var product = await _context.Products.FindAsync(itemDto.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {itemDto.ProductId} not found.");
            }

            sale.AddSaleItem(product, itemDto.Quantity);
        }

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created sale {SaleNumber} with ID {SaleId}", sale.SaleNumber, sale.Id);

        return await GetByIdAsync(sale.Id) ?? throw new InvalidOperationException("Failed to retrieve created sale");
    }

    /// <inheritdoc />
    public async Task<SaleDto> UpdateAsync(int id, UpdateSaleDto dto)
    {
        var sale = await _context.Sales.FindAsync(id);
        if (sale == null)
        {
            throw new KeyNotFoundException($"Sale with ID {id} not found.");
        }

        sale.CustomerName = dto.CustomerName;
        sale.CustomerEmail = dto.CustomerEmail;
        sale.TaxAmount = dto.TaxAmount;
        sale.DiscountAmount = dto.DiscountAmount;
        sale.Notes = dto.Notes;
        sale.CalculateFinalAmount();
        sale.UpdateTimestamp();

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated sale {SaleNumber} with ID {SaleId}", sale.SaleNumber, sale.Id);

        return await GetByIdAsync(sale.Id) ?? throw new InvalidOperationException("Failed to retrieve updated sale");
    }

    /// <inheritdoc />
    public async Task<bool> CompleteAsync(int id)
    {
        var sale = await _context.Sales
            .Include(s => s.SaleItems)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sale == null)
            return false;

        try
        {
            sale.CompleteSale();
            await _context.SaveChangesAsync();

            _logger.LogInformation("Completed sale {SaleNumber} with ID {SaleId}", sale.SaleNumber, sale.Id);
            return true;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Failed to complete sale {SaleId}: {Error}", id, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> CancelAsync(int id)
    {
        var sale = await _context.Sales.FindAsync(id);
        if (sale == null)
            return false;

        try
        {
            sale.CancelSale();
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cancelled sale {SaleNumber} with ID {SaleId}", sale.SaleNumber, sale.Id);
            return true;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Failed to cancel sale {SaleId}: {Error}", id, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> AddPaymentAsync(int saleId, CreatePaymentInfoDto dto)
    {
        var sale = await _context.Sales.FindAsync(saleId);
        if (sale == null)
            return false;

        var paymentInfo = new PaymentInfo
        {
            SaleId = saleId,
            PaymentMethod = Enum.Parse<PaymentMethod>(dto.PaymentMethod),
            Amount = dto.Amount,
            PaymentReference = dto.PaymentReference,
            Notes = dto.Notes,
            Status = PaymentStatus.Pending
        };

        _context.PaymentInfos.Add(paymentInfo);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Added payment info for sale {SaleId}", saleId);
        return true;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SaleListItemDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Include(s => s.SaleItems)
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
            .Select(s => new SaleListItemDto
            {
                Id = s.Id,
                SaleNumber = s.SaleNumber,
                SaleDate = s.SaleDate,
                FinalAmount = s.FinalAmount,
                Status = s.Status.ToString(),
                CustomerName = s.CustomerName,
                ItemCount = s.SaleItems.Count
            })
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<object> GetSalesSummaryAsync()
    {
        var summary = await _context.Sales
            .GroupBy(s => s.Status)
            .Select(g => new
            {
                Status = g.Key.ToString(),
                Count = g.Count(),
                TotalAmount = g.Sum(s => s.FinalAmount)
            })
            .ToListAsync();

        var totalSales = await _context.Sales.CountAsync();
        var totalRevenue = await _context.Sales.SumAsync(s => s.FinalAmount);

        return new
        {
            TotalSales = totalSales,
            TotalRevenue = totalRevenue,
            StatusBreakdown = summary
        };
    }
}
