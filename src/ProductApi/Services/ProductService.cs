using ProductApi.Models;

namespace ProductApi.Services;

public class ProductService
{
    public string GetProductName(Product product)
    {
        return product.Name;
    }

    public decimal CalculateDiscount(Product product, decimal discountPercentage)
    {
        return product.Price - (product.Price * discountPercentage / 100);
    }
}