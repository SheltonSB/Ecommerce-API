using Ecommerce.Api.Contracts;
using Ecommerce.Api.Data;
using Ecommerce.Api.Domain;
using Ecommerce.Api.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.Tests;

public class ProductServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ProductService _service;
    private readonly Mock<ILogger<ProductService>> _loggerMock;

    public ProductServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _loggerMock = new Mock<ILogger<ProductService>>();
        _service = new ProductService(_context, _loggerMock.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var category = new Category { Id = 1, Name = "Electronics", Description = "Electronic devices" };
        _context.Categories.Add(category);

        var products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Smartphone",
                Description = "Latest smartphone",
                Price = 699.99m,
                Sku = "PHONE-001",
                StockQuantity = 50,
                IsActive = true,
                CategoryId = 1
            },
            new Product
            {
                Id = 2,
                Name = "Laptop",
                Description = "High-performance laptop",
                Price = 1299.99m,
                Sku = "LAPTOP-001",
                StockQuantity = 5,
                IsActive = true,
                CategoryId = 1
            },
            new Product
            {
                Id = 3,
                Name = "Tablet",
                Description = "Portable tablet",
                Price = 499.99m,
                Sku = "TABLET-001",
                StockQuantity = 30,
                IsActive = false,
                CategoryId = 1
            }
        };

        _context.Products.AddRange(products);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPaginatedProducts()
    {
        // Arrange
        var request = new PagedRequest { Page = 1, PageSize = 10 };

        // Act
        var result = await _service.GetAllAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.TotalItems.Should().Be(3);
    }

    [Fact]
    public async Task GetAllAsync_WithCategoryFilter_ShouldReturnFilteredProducts()
    {
        // Arrange
        var request = new PagedRequest { Page = 1, PageSize = 10 };
        var categoryId = 1;

        // Act
        var result = await _service.GetAllAsync(request, categoryId);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.Items.Should().OnlyContain(p => p.CategoryName == "Electronics");
    }

    [Fact]
    public async Task GetAllAsync_WithActiveFilter_ShouldReturnActiveProducts()
    {
        // Arrange
        var request = new PagedRequest { Page = 1, PageSize = 10 };

        // Act
        var result = await _service.GetAllAsync(request, isActive: true);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Items.Should().OnlyContain(p => p.IsActive);
    }

    [Fact]
    public async Task GetAllAsync_WithSearchTerm_ShouldReturnMatchingProducts()
    {
        // Arrange
        var request = new PagedRequest { Page = 1, PageSize = 10 };
        var searchTerm = "phone";

        // Act
        var result = await _service.GetAllAsync(request, searchTerm: searchTerm);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Contain("Smartphone");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnProduct()
    {
        // Arrange
        var productId = 1;

        // Act
        var result = await _service.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(productId);
        result.Name.Should().Be("Smartphone");
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
    public async Task GetBySkuAsync_WithValidSku_ShouldReturnProduct()
    {
        // Arrange
        var sku = "PHONE-001";

        // Act
        var result = await _service.GetBySkuAsync(sku);

        // Assert
        result.Should().NotBeNull();
        result!.Sku.Should().Be(sku);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateProduct()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "New Product",
            Description = "Test product",
            Price = 99.99m,
            Sku = "TEST-001",
            StockQuantity = 10,
            IsActive = true,
            CategoryId = 1
        };

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Product");
        result.Sku.Should().Be("TEST-001");
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateSku_ShouldThrowException()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Duplicate Product",
            Description = "Test product",
            Price = 99.99m,
            Sku = "PHONE-001",
            StockQuantity = 10,
            IsActive = true,
            CategoryId = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateProduct()
    {
        // Arrange
        var productId = 1;
        var dto = new UpdateProductDto
        {
            Name = "Updated Smartphone",
            Description = "Updated description",
            Price = 749.99m,
            Sku = "PHONE-001-UPDATED",
            StockQuantity = 75,
            IsActive = true,
            CategoryId = 1
        };

        // Act
        var result = await _service.UpdateAsync(productId, dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Smartphone");
        result.Price.Should().Be(749.99m);
        result.Sku.Should().Be("PHONE-001-UPDATED");

        var priceHistory = await _context.PriceHistories.SingleAsync();
        priceHistory.OldPrice.Should().Be(699.99m);
        priceHistory.NewPrice.Should().Be(749.99m);
    }

    [Fact]
    public async Task UpdateStockAsync_WithValidData_ShouldUpdateStock()
    {
        // Arrange
        var productId = 1;
        var dto = new UpdateStockDto { StockQuantity = 100 };

        // Act
        var result = await _service.UpdateStockAsync(productId, dto);

        // Assert
        result.Should().NotBeNull();
        result.StockQuantity.Should().Be(100);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldSoftDeleteProduct()
    {
        // Arrange
        var productId = 3;

        // Act
        var result = await _service.DeleteAsync(productId);

        // Assert
        result.Should().BeTrue();

        var product = await _context.Products.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == productId);
        product.Should().NotBeNull();
        product!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task GetLowStockProductsAsync_ShouldReturnLowStockProducts()
    {
        // Arrange
        var threshold = 10;

        // Act
        var result = await _service.GetLowStockProductsAsync(threshold);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Laptop");
    }

    [Fact]
    public async Task GetByCategoryAsync_ShouldReturnProductsInCategory()
    {
        // Arrange
        var categoryId = 1;

        // Act
        var result = await _service.GetByCategoryAsync(categoryId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2); // Only active products
    }

    [Fact]
    public async Task SkuExistsAsync_WithExistingSku_ShouldReturnTrue()
    {
        // Arrange
        var existingSku = "PHONE-001";

        // Act
        var result = await _service.SkuExistsAsync(existingSku);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task SkuExistsAsync_WithNonExistingSku_ShouldReturnFalse()
    {
        // Arrange
        var nonExistingSku = "NON-EXISTENT";

        // Act
        var result = await _service.SkuExistsAsync(nonExistingSku);

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}








