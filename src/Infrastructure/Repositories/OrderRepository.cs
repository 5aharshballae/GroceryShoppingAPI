using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
	private readonly ApplicationDbContext _context;

	public OrderRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<Order?> GetByIdAsync(Guid id)
	{
		return await _context.Orders.FindAsync(id);
	}

	public async Task<IEnumerable<Order>> GetAllAsync()
	{
		return await _context.Orders.ToListAsync();
	}

	public async Task<Order> AddAsync(Order entity)
	{
		await _context.Orders.AddAsync(entity);
		return entity;
	}

	public Task UpdateAsync(Order entity)
	{
		_context.Orders.Update(entity);
		return Task.CompletedTask;
	}

	public Task DeleteAsync(Order entity)
	{
		_context.Orders.Remove(entity);
		return Task.CompletedTask;
	}

	public async Task<bool> SaveChangesAsync()
	{
		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId)
	{
		return await _context.Orders
			.Include(o => o.Items)
			.Where(o => o.UserId == userId)
			.OrderByDescending(o => o.CreatedAt)
			.ToListAsync();
	}

	public async Task<Order?> GetOrderWithItemsAsync(Guid orderId)
	{
		return await _context.Orders
			.Include(o => o.Items)
			.FirstOrDefaultAsync(o => o.Id == orderId);
	}

	public async Task<Order?> GetOrderByNumberAsync(string orderNumber)
	{
		return await _context.Orders
			.Include(o => o.Items)
			.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
	}
}