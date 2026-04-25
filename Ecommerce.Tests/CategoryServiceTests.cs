using Ecommerce.Api.Contracts;
using Ecommerce.Api.Data;
using Ecommerce.Api.Domain;
using Ecommerce.Api.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.Tests;

public class CategoryServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CategoryService _service;
    private readonly Mock<ILogger<CategoryService>> _loggerMock;

    public CategoryServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _loggerMock = new Mock<ILogger<CategoryService>>();
        _service = new CategoryService(_context, _loggerMock.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Electronics", Description = "Electronic devices" },
            new Category { Id = 2, Name = "Clothing", Description = "Apparel items" },
            new Category { Id = 3, Name = "Books", Description = "Books and media" }
        };

        _context.Categories.AddRange(categories);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPaginatedCategories()
    {
        // Arrange
        var request = new PagedRequest { Page = 1, PageSize = 10 };

        // Act
        var result = await _service.GetAllAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.TotalItems.Should().Be(3);
        result.Page.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnCategory()
    {
        // Arrange
        var categoryId = 1;

        // Act
        var result = await _service.GetByIdAsync(categoryId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(categoryId);
        result.Name.Should().Be("Electronics");
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
    public async Task CreateAsync_WithValidData_ShouldCreateCategory()
    {
        // Arrange
        var dto = new CreateCategoryDto
        {
            Name = "Test Category",
            Description = "Test Description"
        };

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Category");
        result.Description.Should().Be("Test Description");

        var savedCategory = await _context.Categories.FindAsync(result.Id);
        savedCategory.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ShouldThrowException()
    {
        // Arrange
        var dto = new CreateCategoryDto
        {
            Name = "Electronics",
            Description = "Duplicate category"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateCategory()
    {
        // Arrange
        var categoryId = 1;
        var dto = new UpdateCategoryDto
        {
            Name = "Updated Electronics",
            Description = "Updated description"
        };

        // Act
        var result = await _service.UpdateAsync(categoryId, dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Electronics");
        result.Description.Should().Be("Updated description");
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        var invalidId = 999;
        var dto = new UpdateCategoryDto
        {
            Name = "Updated Category",
            Description = "Updated description"
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(invalidId, dto));
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldSoftDeleteCategory()
    {
        // Arrange
        var categoryId = 3;

        // Act
        var result = await _service.DeleteAsync(categoryId);

        // Assert
        result.Should().BeTrue();

        var category = await _context.Categories.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == categoryId);
        category.Should().NotBeNull();
        category!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var invalidId = 999;

        // Act
        var result = await _service.DeleteAsync(invalidId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task RestoreAsync_WithDeletedCategory_ShouldRestoreCategory()
    {
        // Arrange
        var categoryId = 2;
        await _service.DeleteAsync(categoryId);

        // Act
        var result = await _service.RestoreAsync(categoryId);

        // Assert
        result.Should().BeTrue();

        var category = await _context.Categories.FindAsync(categoryId);
        category.Should().NotBeNull();
        category!.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task NameExistsAsync_WithExistingName_ShouldReturnTrue()
    {
        // Arrange
        var existingName = "Electronics";

        // Act
        var result = await _service.NameExistsAsync(existingName);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task NameExistsAsync_WithNonExistingName_ShouldReturnFalse()
    {
        // Arrange
        var nonExistingName = "NonExistent";

        // Act
        var result = await _service.NameExistsAsync(nonExistingName);

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}


