using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
	private readonly ApplicationDbContext _context;

	public UserRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<User?> GetByIdAsync(Guid id)
	{
		return await _context.Users.FindAsync(id);
	}

	public async Task<IEnumerable<User>> GetAllAsync()
	{
		return await _context.Users.ToListAsync();
	}

	public async Task<User> AddAsync(User entity)
	{
		await _context.Users.AddAsync(entity);
		return entity;
	}

	public Task UpdateAsync(User entity)
	{
		_context.Users.Update(entity);
		return Task.CompletedTask;
	}

	public Task DeleteAsync(User entity)
	{
		_context.Users.Remove(entity);
		return Task.CompletedTask;
	}

	public async Task<bool> SaveChangesAsync()
	{
		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<User?> GetByEmailAsync(string email)
	{
		return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
	}

	public async Task<bool> ExistsAsync(string email)
	{
		return await _context.Users.AnyAsync(u => u.Email == email);
	}
}