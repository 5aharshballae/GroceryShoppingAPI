using Domain.Entities;

namespace Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
	Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);
	Task<Order?> GetOrderWithItemsAsync(Guid orderId);
	Task<Order?> GetOrderByNumberAsync(string orderNumber);
}