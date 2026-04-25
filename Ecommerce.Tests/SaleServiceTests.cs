using Ecommerce.Api.Contracts;
using Ecommerce.Api.Data;
using Ecommerce.Api.Domain;
using Ecommerce.Api.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.Tests;

public class SaleServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly SaleService _service;
    private readonly Mock<ILogger<SaleService>> _loggerMock;

    public SaleServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _loggerMock = new Mock<ILogger<SaleService>>();
        _service = new SaleService(_context, _loggerMock.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var category = new Category { Id = 1, Name = "Electronics", Description = "Electronic devices" };
        _context.Categories.Add(category);

        var product = new Product
        {
            Id = 1,
            Name = "Smartphone",
            Description = "Latest smartphone",
            Price = 699.99m,
            Sku = "PHONE-001",
            StockQuantity = 50,
            IsActive = true,
            CategoryId = 1
        };
        _context.Products.Add(product);

        var sale = new Sale
        {
            Id = 1,
            SaleNumber = "SALE-000001",
            SaleDate = DateTime.UtcNow,
            TotalAmount = 699.99m,
            TaxAmount = 69.99m,
            DiscountAmount = 0,
            FinalAmount = 769.98m,
            Status = SaleStatus.Pending,
            CustomerName = "John Doe",
            CustomerEmail = "john@example.com"
        };
        _context.Sales.Add(sale);

        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPaginatedSales()
    {
        // Arrange
        var request = new PagedRequest { Page = 1, PageSize = 10 };

        // Act
        var result = await _service.GetAllAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.TotalItems.Should().Be(1);
    }

    [Fact]
    public async Task GetAllAsync_WithStatusFilter_ShouldReturnFilteredSales()
    {
        // Arrange
        var request = new PagedRequest { Page = 1, PageSize = 10 };
        var status = "Pending";

        // Act
        var result = await _service.GetAllAsync(request, status);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Status.Should().Be("Pending");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnSale()
    {
        // Arrange
        var saleId = 1;

        // Act
        var result = await _service.GetByIdAsync(saleId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(saleId);
        result.SaleNumber.Should().Be("SALE-000001");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var invalidId = 999;

        // Act
        var result = await _service.GetByIdAsync(invalidId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateSale()
    {
        // Arrange
        var dto = new CreateSaleDto
        {
            CustomerName = "Jane Doe",
            CustomerEmail = "jane@example.com",
            TaxAmount = 10.00m,
            DiscountAmount = 5.00m,
            Notes = "Test sale",
            SaleItems = new List<CreateSaleItemDto>
            {
                new CreateSaleItemDto { ProductId = 1, Quantity = 1 }
            }
        };

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.CustomerName.Should().Be("Jane Doe");
        result.SaleItems.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateSale()
    {
        // Arrange
        var saleId = 1;
        var dto = new UpdateSaleDto
        {
            CustomerName = "Updated Name",
            CustomerEmail = "updated@example.com",
            TaxAmount = 80.00m,
            DiscountAmount = 10.00m,
            Notes = "Updated notes"
        };

        // Act
        var result = await _service.UpdateAsync(saleId, dto);

        // Assert
        result.Should().NotBeNull();
        result.CustomerName.Should().Be("Updated Name");
        result.TaxAmount.Should().Be(80.00m);
    }

    [Fact]
    public async Task CancelAsync_WithPendingSale_ShouldCancelSale()
    {
        // Arrange
        var saleId = 1;

        // Act
        var result = await _service.CancelAsync(saleId);

        // Assert
        result.Should().BeTrue();

        var sale = await _context.Sales.FindAsync(saleId);
        sale.Should().NotBeNull();
        sale!.Status.Should().Be(SaleStatus.Cancelled);
    }

    [Fact]
    public async Task CompleteAsync_WithPendingSale_ShouldCompleteSaleAndReduceStock()
    {
        // Arrange
        var sale = await _context.Sales.FindAsync(1);
        sale!.AddSaleItem(await _context.Products.FindAsync(1) ?? throw new InvalidOperationException(), 2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.CompleteAsync(1);

        // Assert
        result.Should().BeTrue();

        var savedSale = await _context.Sales.FindAsync(1);
        var product = await _context.Products.FindAsync(1);

        savedSale!.Status.Should().Be(SaleStatus.Completed);
        product!.StockQuantity.Should().Be(48);
    }

    [Fact]
    public async Task GetByDateRangeAsync_ShouldReturnSalesInRange()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-1);
        var endDate = DateTime.UtcNow.AddDays(1);

        // Act
        var result = await _service.GetByDateRangeAsync(startDate, endDate);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetSalesSummaryAsync_ShouldReturnSummary()
    {
        // Act
        var result = await _service.GetSalesSummaryAsync();

        // Assert
        result.Should().NotBeNull();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}








