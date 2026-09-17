using ProductApi.Models;
using ProductApi.Services;

namespace ProductApi.UnitTests;

public class ProductServiceTests
{
    [Fact]
    public void GetProductName_ShouldReturnProductName()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Price = 1200m
        };

        var service = new ProductService();

        // Act
        var result = service.GetProductName(product);

        // Assert
        Assert.Equal("Laptop", result);
    }

    [Fact]
    public void CalculateDiscount_ShouldReturnDiscountedPrice()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Price = 1000m
        };

        var service = new ProductService();

        // Act
        var result = service.CalculateDiscount(product, 10);

        // Assert
        Assert.Equal(900m, result);
    }
}