using Ecommerce.Api.Domain;
using FluentAssertions;

namespace Ecommerce.Tests;

public class DomainEntityTests
{
    [Fact]
    public void Product_IsValid_WithValidData_ShouldReturnTrue()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Sku = "TEST-001",
            Price = 99.99m,
            StockQuantity = 10
        };

        // Act
        var result = product.IsValid();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Product_IsValid_WithInvalidData_ShouldReturnFalse()
    {
        // Arrange
        var product = new Product
        {
            Name = "",
            Sku = "TEST-001",
            Price = -10,
            StockQuantity = -5
        };

        // Act
        var result = product.IsValid();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Product_UpdateStock_WithValidQuantity_ShouldUpdateStock()
    {
        // Arrange
        var product = new Product { StockQuantity = 10 };

        // Act
        product.UpdateStock(20);

        // Assert
        product.StockQuantity.Should().Be(20);
    }

    [Fact]
    public void Product_UpdateStock_WithNegativeQuantity_ShouldThrowException()
    {
        // Arrange
        var product = new Product { StockQuantity = 10 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => product.UpdateStock(-5));
    }

    [Fact]
    public void Product_ReduceStock_WithValidQuantity_ShouldReduceStock()
    {
        // Arrange
        var product = new Product { StockQuantity = 10 };

        // Act
        product.ReduceStock(5);

        // Assert
        product.StockQuantity.Should().Be(5);
    }

    [Fact]
    public void Product_ReduceStock_WithInsufficientStock_ShouldThrowException()
    {
        // Arrange
        var product = new Product { StockQuantity = 5 };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => product.ReduceStock(10));
    }

    [Fact]
    public void Entity_MarkAsDeleted_ShouldSetIsDeletedToTrue()
    {
        // Arrange
        var product = new Product();

        // Act
        product.MarkAsDeleted();

        // Assert
        product.IsDeleted.Should().BeTrue();
        product.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public void Entity_Restore_ShouldSetIsDeletedToFalse()
    {
        // Arrange
        var product = new Product();
        product.MarkAsDeleted();

        // Act
        product.Restore();

        // Assert
        product.IsDeleted.Should().BeFalse();
        product.DeletedAt.Should().BeNull();
    }

    [Fact]
    public void Category_IsValid_WithValidData_ShouldReturnTrue()
    {
        // Arrange
        var category = new Category
        {
            Name = "Electronics",
            Description = "Electronic devices"
        };

        // Act
        var result = category.IsValid();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Sale_CalculateTotalAmount_ShouldCalculateCorrectly()
    {
        // Arrange
        var product = new Product { Id = 1, Price = 100m };
        var sale = new Sale { TaxAmount = 10m, DiscountAmount = 5m };
        var saleItem = new SaleItem
        {
            ProductId = 1,
            Product = product,
            Quantity = 2,
            UnitPrice = 100m
        };
        saleItem.CalculateTotalPrice();
        sale.SaleItems.Add(saleItem);

        // Act
        sale.CalculateTotalAmount();

        // Assert
        sale.TotalAmount.Should().Be(200m);
        sale.FinalAmount.Should().Be(205m); // 200 + 10 - 5
    }

    [Fact]
    public void Sale_CancelSale_WithPendingSale_ShouldCancel()
    {
        // Arrange
        var sale = new Sale { Status = SaleStatus.Pending };

        // Act
        sale.CancelSale();

        // Assert
        sale.Status.Should().Be(SaleStatus.Cancelled);
    }

    [Fact]
    public void Sale_CancelSale_WithCompletedSale_ShouldThrowException()
    {
        // Arrange
        var sale = new Sale { Status = SaleStatus.Completed };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => sale.CancelSale());
    }

    [Fact]
    public void PriceHistory_GetPercentageChange_ShouldCalculateCorrectly()
    {
        // Arrange
        var priceHistory = new PriceHistory
        {
            OldPrice = 100m,
            NewPrice = 120m
        };

        // Act
        var result = priceHistory.GetPercentageChange();

        // Assert
        result.Should().Be(20m);
    }

    [Fact]
    public void PaymentInfo_MarkAsProcessed_ShouldUpdateStatus()
    {
        // Arrange
        var paymentInfo = new PaymentInfo { Status = PaymentStatus.Pending };

        // Act
        paymentInfo.MarkAsProcessed("TXN-12345");

        // Assert
        paymentInfo.Status.Should().Be(PaymentStatus.Completed);
        paymentInfo.TransactionId.Should().Be("TXN-12345");
        paymentInfo.ProcessedAt.Should().NotBeNull();
    }
}

