using Domain.Entities;
using Domain.Interfaces;
using Application.Interfaces;

namespace Application.Commands;

public class CreateOrderCommand
{
	public Guid UserId { get; set; }
	public string CardNumber { get; set; } = string.Empty;
	public string CardHolderName { get; set; } = string.Empty;
	public string ExpiryMonth { get; set; } = string.Empty;
	public string ExpiryYear { get; set; } = string.Empty;
	public string Cvv { get; set; } = string.Empty;
}

public class CreateOrderCommandHandler
{
	private readonly IOrderRepository _orderRepository;
	private readonly ICartRepository _cartRepository;
	private readonly IProductRepository _productRepository;
	private readonly IPaymentService _paymentService;
	private readonly IEmailService _emailService;
	private readonly IUserRepository _userRepository;

	public CreateOrderCommandHandler(
		IOrderRepository orderRepository,
		ICartRepository cartRepository,
		IProductRepository productRepository,
		IPaymentService paymentService,
		IEmailService emailService,
		IUserRepository userRepository)
	{
		_orderRepository = orderRepository;
		_cartRepository = cartRepository;
		_productRepository = productRepository;
		_paymentService = paymentService;
		_emailService = emailService;
		_userRepository = userRepository;
	}

	public async Task<Order> HandleAsync(CreateOrderCommand command)
	{
		var cart = await _cartRepository.GetCartByUserIdAsync(command.UserId);
		if (cart == null)
		{
			throw new InvalidOperationException("Cart not found");
		}

		cart = await _cartRepository.GetCartWithItemsAsync(cart.Id);

		if (!cart!.Items.Any())
		{
			throw new InvalidOperationException("Cart is empty");
		}

		decimal totalAmount = 0;
		foreach (var item in cart.Items)
		{
			var product = await _productRepository.GetByIdAsync(item.ProductId);
			if (product == null || product.StockQuantity < item.Quantity)
			{
				throw new InvalidOperationException($"Insufficient stock for product {item.ProductId}");
			}
			totalAmount += product.Price * item.Quantity;
		}

		var paymentResult = await _paymentService.ProcessPaymentAsync(
			command.CardNumber,
			command.CardHolderName,
			command.ExpiryMonth,
			command.ExpiryYear,
			command.Cvv,
			totalAmount
		);

		if (!paymentResult.Success)
		{
			throw new InvalidOperationException($"Payment failed: {paymentResult.Message}");
		}

		var order = new Order
		{
			Id = Guid.NewGuid(),
			OrderNumber = GenerateOrderNumber(),
			UserId = command.UserId,
			TotalAmount = totalAmount,
			Status = OrderStatus.Completed,
			PaymentIntentId = paymentResult.TransactionId,
			CreatedAt = DateTime.UtcNow
		};

		foreach (var item in cart.Items)
		{
			var product = await _productRepository.GetByIdAsync(item.ProductId);

			order.Items.Add(new OrderItem
			{
				Id = Guid.NewGuid(),
				OrderId = order.Id,
				ProductId = item.ProductId,
				ProductName = product!.Name,
				Quantity = item.Quantity,
				UnitPrice = product.Price,
				Subtotal = product.Price * item.Quantity
			});

			await _productRepository.UpdateStockAsync(item.ProductId, product.StockQuantity - item.Quantity);
		}

		await _orderRepository.AddAsync(order);
		await _cartRepository.ClearCartAsync(cart.Id);
		await _orderRepository.SaveChangesAsync();

		var user = await _userRepository.GetByIdAsync(command.UserId);
		await _emailService.SendOrderConfirmationAsync(user!.Email, order.OrderNumber, totalAmount);

		return order;
	}

	private string GenerateOrderNumber()
	{
		return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
	}
}