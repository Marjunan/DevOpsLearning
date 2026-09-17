using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Models;
using ProductApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=products.db"));

builder.Services.AddScoped<ProductService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    db.Database.EnsureCreated();

    if (!db.Products.Any())
    {
        db.Products.AddRange(
            new Product
            {
                Name = "Laptop",
                Price = 1200m
            },
            new Product
            {
                Name = "Monitor",
                Price = 400m
            },
            new Product
            {
                Name = "Keyboard",
                Price = 100m
            });

        db.SaveChanges();
    }
}

app.MapGet("/api/products", async (AppDbContext db) =>
{
    var products = await db.Products.ToListAsync();

    return Results.Ok(products);
});

app.MapGet("/api/products/{id:int}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);

    return product is null
        ? Results.NotFound()
        : Results.Ok(product);
});

app.MapPost("/api/products", async (Product product, AppDbContext db) =>
{
    db.Products.Add(product);

    await db.SaveChangesAsync();

    return Results.Created($"/api/products/{product.Id}", product);
});

app.Run();

public partial class Program
{
}