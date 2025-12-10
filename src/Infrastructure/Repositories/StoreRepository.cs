using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class StoreRepository : IStoreRepository
{
	private readonly ApplicationDbContext _context;

	public StoreRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<Store?> GetByIdAsync(Guid id)
	{
		return await _context.Stores.FindAsync(id);
	}

	public async Task<IEnumerable<Store>> GetAllAsync()
	{
		return await _context.Stores.ToListAsync();
	}

	public async Task<Store> AddAsync(Store entity)
	{
		await _context.Stores.AddAsync(entity);
		return entity;
	}

	public Task UpdateAsync(Store entity)
	{
		_context.Stores.Update(entity);
		return Task.CompletedTask;
	}

	public Task DeleteAsync(Store entity)
	{
		_context.Stores.Remove(entity);
		return Task.CompletedTask;
	}

	public async Task<bool> SaveChangesAsync()
	{
		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<IEnumerable<Store>> GetActiveStoresAsync()
	{
		return await _context.Stores.Where(s => s.IsActive).ToListAsync();
	}

	public async Task<Store?> GetStoreWithCategoriesAsync(Guid storeId)
	{
		return await _context.Stores
			.Include(s => s.Categories)
			.FirstOrDefaultAsync(s => s.Id == storeId);
	}
}
