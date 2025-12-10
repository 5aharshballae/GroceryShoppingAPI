using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
	private readonly ApplicationDbContext _context;

	public CartRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<Cart?> GetByIdAsync(Guid id)
	{
		return await _context.Carts.FindAsync(id);
	}

	public async Task<IEnumerable<Cart>> GetAllAsync()
	{
		return await _context.Carts.ToListAsync();
	}

	public async Task<Cart> AddAsync(Cart entity)
	{
		await _context.Carts.AddAsync(entity);
		return entity;
	}

	public Task UpdateAsync(Cart entity)
	{
		_context.Carts.Update(entity);
		return Task.CompletedTask;
	}

	public Task DeleteAsync(Cart entity)
	{
		_context.Carts.Remove(entity);
		return Task.CompletedTask;
	}

	public async Task<bool> SaveChangesAsync()
	{
		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<Cart?> GetCartByUserIdAsync(Guid userId)
	{
		return await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
	}

	public async Task<Cart?> GetCartWithItemsAsync(Guid cartId)
	{
		return await _context.Carts
			.Include(c => c.Items)
			.ThenInclude(i => i.Product)
			.FirstOrDefaultAsync(c => c.Id == cartId);
	}

	public async Task ClearCartAsync(Guid cartId)
	{
		var cart = await GetCartWithItemsAsync(cartId);
		if (cart != null)
		{
			_context.CartItems.RemoveRange(cart.Items);
			await _context.SaveChangesAsync();
		}
	}
}