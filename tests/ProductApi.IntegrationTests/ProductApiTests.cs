using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Data;
using ProductApi.Models;

namespace ProductApi.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services
                .SingleOrDefault(
                    descriptor => descriptor.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (dbContextDescriptor is not null)
            {
                services.Remove(dbContextDescriptor);
            }

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            db.Database.EnsureCreated();

            db.Products.AddRange(
                new Product
                {
                    Id = 1,
                    Name = "CI Laptop",
                    Price = 1000m
                },
                new Product
                {
                    Id = 2,
                    Name = "Test Monitor",
                    Price = 500m
                });

            db.SaveChanges();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (_connection is not null)
        {
            _connection.Dispose();
        }
    }
}

public class ProductApiTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ShouldReturnSeededProducts()
    {
        // Arrange
        var requestUri = "/api/products";

        // Act
        var response = await _client.GetAsync(requestUri);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var products = await response.Content
            .ReadFromJsonAsync<List<Product>>();

        Assert.NotNull(products);
        Assert.Equal(2, products.Count);
        Assert.Equal("CI Laptop", products[0].Name);
        Assert.Equal(1000m, products[0].Price);
    }

    [Fact]
    public async Task PostProduct_WithInvalidPrice_ReturnsBadRequest()
    {
        // Arrange
        var product = new
        {
            Name = "Mouse",
            Price = -50
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/products",
            product);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostProduct_WithMissingName_ReturnsBadRequest()
    {
        // Arrange
        var product = new
        {
            Name = "",
            Price = 50
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/products",
            product);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

}