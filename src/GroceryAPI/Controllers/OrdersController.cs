using System.Security.Claims;
using Application.Commands;
using Application.DTOs;
using Application.Queries;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
	private readonly CreateOrderCommandHandler _createOrderHandler;
	private readonly GetOrderHistoryQueryHandler _getOrderHistoryHandler;
	private readonly IOrderRepository _orderRepository;

	public OrdersController(
		CreateOrderCommandHandler createOrderHandler,
		GetOrderHistoryQueryHandler getOrderHistoryHandler,
		IOrderRepository orderRepository)
	{
		_createOrderHandler = createOrderHandler;
		_getOrderHistoryHandler = getOrderHistoryHandler;
		_orderRepository = orderRepository;
	}

	[HttpPost("checkout")]
	public async Task<ActionResult<CheckoutResponse>> Checkout([FromBody] CheckoutRequest request)
	{
		try
		{
			var userId = GetCurrentUserId();
			var command = new CreateOrderCommand
			{
				UserId = userId,
				CardNumber = request.CardNumber,
				CardHolderName = request.CardHolderName,
				ExpiryMonth = request.ExpiryMonth,
				ExpiryYear = request.ExpiryYear,
				Cvv = request.Cvv
			};

			var order = await _createOrderHandler.HandleAsync(command);

			return Ok(new CheckoutResponse(
				order.Id,
				order.OrderNumber,
				order.TotalAmount,
				order.Status.ToString(),
				"Order placed successfully"
			));
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrderHistory()
	{
		var userId = GetCurrentUserId();
		var query = new GetOrderHistoryQuery { UserId = userId };
		var orders = await _getOrderHistoryHandler.HandleAsync(query);
		return Ok(orders);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
	{
		var userId = GetCurrentUserId();
		var order = await _orderRepository.GetOrderWithItemsAsync(id);

		if (order == null)
		{
			return NotFound(new { message = "Order not found" });
		}

		if (order.UserId != userId)
		{
			return Forbid();
		}

		var orderDto = new OrderDto(
			order.Id,
			order.OrderNumber,
			order.TotalAmount,
			order.Status.ToString(),
			order.CreatedAt,
			order.Items.Select(i => new OrderItemDto(
				i.ProductId,
				i.ProductName,
				i.Quantity,
				i.UnitPrice,
				i.Subtotal
			)).ToList()
		);

		return Ok(orderDto);
	}

	private Guid GetCurrentUserId()
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		return Guid.Parse(userIdClaim!);
	}
}