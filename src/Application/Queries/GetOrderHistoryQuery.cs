using Domain.Interfaces;
using Application.DTOs;

namespace Application.Queries;

public class GetOrderHistoryQuery
{
	public Guid UserId { get; set; }
}

public class GetOrderHistoryQueryHandler
{
	private readonly IOrderRepository _orderRepository;

	public GetOrderHistoryQueryHandler(IOrderRepository orderRepository)
	{
		_orderRepository = orderRepository;
	}

	public async Task<IEnumerable<OrderDto>> HandleAsync(GetOrderHistoryQuery query)
	{
		var orders = await _orderRepository.GetOrdersByUserIdAsync(query.UserId);

		return orders.Select(o => new OrderDto(
			o.Id,
			o.OrderNumber,
			o.TotalAmount,
			o.Status.ToString(),
			o.CreatedAt,
			o.Items.Select(i => new OrderItemDto(
				i.ProductId,
				i.ProductName,
				i.Quantity,
				i.UnitPrice,
				i.Subtotal
			)).ToList()
		));
	}
}