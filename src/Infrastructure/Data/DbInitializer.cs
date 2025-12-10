using Domain.Entities;

namespace Infrastructure.Data;

public static class DbInitializer
{
	public static async Task InitializeAsync(ApplicationDbContext context)
	{
		await context.Database.EnsureCreatedAsync();

		if (context.Stores.Any())
		{
			return;
		}

		var store1 = new Store
		{
			Id = Guid.NewGuid(),
			Name = "Fresh Market",
			Description = "Your neighborhood grocery store",
			Address = "123 St, Dubai",
			IsActive = true
		};

		var store2 = new Store
		{
			Id = Guid.NewGuid(),
			Name = "Organic Foods",
			Description = "100% Organic products",
			Address = "456 Avenue, RAK",
			IsActive = true
		};

		context.Stores.AddRange(store1, store2);
		await context.SaveChangesAsync();

		var category1 = new Category
		{
			Id = Guid.NewGuid(),
			Name = "Fruits",
			Description = "Fresh fruits",
			StoreId = store1.Id
		};

		var category2 = new Category
		{
			Id = Guid.NewGuid(),
			Name = "Vegetables",
			Description = "Fresh vegetables",
			StoreId = store1.Id
		};

		var category3 = new Category
		{
			Id = Guid.NewGuid(),
			Name = "Dairy",
			Description = "Milk, cheese, yogurt",
			StoreId = store2.Id
		};

		context.Categories.AddRange(category1, category2, category3);
		await context.SaveChangesAsync();

		var products = new[]
		{
			new Product { Id = Guid.NewGuid(), Name = "Apple", Description = "Fresh red apples", Price = 2.99m, StockQuantity = 100, CategoryId = category1.Id, IsAvailable = true, ImageUrl = "https://example.com/apple.jpg" },
			new Product { Id = Guid.NewGuid(), Name = "Banana", Description = "Ripe bananas", Price = 1.99m, StockQuantity = 150, CategoryId = category1.Id, IsAvailable = true, ImageUrl = "https://example.com/banana.jpg" },
			new Product { Id = Guid.NewGuid(), Name = "Orange", Description = "Juicy oranges", Price = 3.49m, StockQuantity = 120, CategoryId = category1.Id, IsAvailable = true, ImageUrl = "https://example.com/orange.jpg" },
			new Product { Id = Guid.NewGuid(), Name = "Carrot", Description = "Organic carrots", Price = 2.49m, StockQuantity = 80, CategoryId = category2.Id, IsAvailable = true, ImageUrl = "https://example.com/carrot.jpg" },
			new Product { Id = Guid.NewGuid(), Name = "Broccoli", Description = "Fresh broccoli", Price = 3.99m, StockQuantity = 60, CategoryId = category2.Id, IsAvailable = true, ImageUrl = "https://example.com/broccoli.jpg" },
			new Product { Id = Guid.NewGuid(), Name = "Spinach", Description = "Leafy green spinach", Price = 2.99m, StockQuantity = 90, CategoryId = category2.Id, IsAvailable = true, ImageUrl = "https://example.com/spinach.jpg" },
			new Product { Id = Guid.NewGuid(), Name = "Milk", Description = "Whole milk 1L", Price = 4.49m, StockQuantity = 50, CategoryId = category3.Id, IsAvailable = true, ImageUrl = "https://example.com/milk.jpg" },
			new Product { Id = Guid.NewGuid(), Name = "Cheese", Description = "Cheddar cheese", Price = 5.99m, StockQuantity = 40, CategoryId = category3.Id, IsAvailable = true, ImageUrl = "https://example.com/cheese.jpg" }
		};

		context.Products.AddRange(products);
		await context.SaveChangesAsync();
	}
}