using Domain.Entities;

namespace Domain.Interfaces;

public interface ICartRepository : IRepository<Cart>
{
	Task<Cart?> GetCartByUserIdAsync(Guid userId);
	Task<Cart?> GetCartWithItemsAsync(Guid cartId);
	Task ClearCartAsync(Guid cartId);
}