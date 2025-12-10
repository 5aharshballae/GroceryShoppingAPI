using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
	private readonly ApplicationDbContext _context;

	public ProductRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<Product?> GetByIdAsync(Guid id)
	{
		return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
	}

	public async Task<IEnumerable<Product>> GetAllAsync()
	{
		return await _context.Products.Include(p => p.Category).ToListAsync();
	}

	public async Task<Product> AddAsync(Product entity)
	{
		await _context.Products.AddAsync(entity);
		return entity;
	}

	public Task UpdateAsync(Product entity)
	{
		_context.Products.Update(entity);
		return Task.CompletedTask;
	}

	public Task DeleteAsync(Product entity)
	{
		_context.Products.Remove(entity);
		return Task.CompletedTask;
	}

	public async Task<bool> SaveChangesAsync()
	{
		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId)
	{
		return await _context.Products
			.Include(p => p.Category)
			.Where(p => p.CategoryId == categoryId)
			.ToListAsync();
	}

	public async Task<IEnumerable<Product>> GetProductsByStoreAsync(Guid storeId)
	{
		return await _context.Products
			.Include(p => p.Category)
			.Where(p => p.Category.StoreId == storeId)
			.ToListAsync();
	}

	public async Task<bool> UpdateStockAsync(Guid productId, int quantity)
	{
		var product = await _context.Products.FindAsync(productId);
		if (product == null) return false;

		product.StockQuantity = quantity;
		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<Product?> GetProductWithCategoryAsync(Guid productId)
	{
		return await _context.Products
			.Include(p => p.Category)
			.FirstOrDefaultAsync(p => p.Id == productId);
	}
}