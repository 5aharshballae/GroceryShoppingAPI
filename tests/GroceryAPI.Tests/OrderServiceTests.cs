using Application.Commands;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace GroceryAPI.Tests;

public class OrderServiceTests
{
	private readonly Mock<IOrderRepository> _mockOrderRepository;
	private readonly Mock<ICartRepository> _mockCartRepository;
	private readonly Mock<IProductRepository> _mockProductRepository;
	private readonly Mock<IPaymentService> _mockPaymentService;
	private readonly Mock<IEmailService> _mockEmailService;
	private readonly Mock<IUserRepository> _mockUserRepository;
	private readonly CreateOrderCommandHandler _handler;

	public OrderServiceTests()
	{
		_mockOrderRepository = new Mock<IOrderRepository>();
		_mockCartRepository = new Mock<ICartRepository>();
		_mockProductRepository = new Mock<IProductRepository>();
		_mockPaymentService = new Mock<IPaymentService>();
		_mockEmailService = new Mock<IEmailService>();
		_mockUserRepository = new Mock<IUserRepository>();

		_handler = new CreateOrderCommandHandler(
			_mockOrderRepository.Object,
			_mockCartRepository.Object,
			_mockProductRepository.Object,
			_mockPaymentService.Object,
			_mockEmailService.Object,
			_mockUserRepository.Object
		);
	}

	[Fact]
	public async Task CreateOrder_WithEmptyCart_ThrowsException()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var cart = new Cart
		{
			Id = Guid.NewGuid(),
			UserId = userId,
			Items = new List<CartItem>()
		};

		_mockCartRepository
			.Setup(r => r.GetCartByUserIdAsync(userId))
			.ReturnsAsync(cart);

		_mockCartRepository
			.Setup(r => r.GetCartWithItemsAsync(cart.Id))
			.ReturnsAsync(cart);

		var command = new CreateOrderCommand
		{
			UserId = userId,
			CardNumber = "4111111111111111",
			CardHolderName = "John Doe",
			ExpiryMonth = "12",
			ExpiryYear = "2025",
			Cvv = "123"
		};

		// Act & Assert
		await Assert.ThrowsAsync<InvalidOperationException>(
			() => _handler.HandleAsync(command)
		);
	}

	[Fact]
	public async Task CreateOrder_WithValidCart_CreatesOrder()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var productId = Guid.NewGuid();

		var product = new Product
		{
			Id = productId,
			Name = "Apple",
			Price = 2.99m,
			StockQuantity = 100
		};

		var cart = new Cart
		{
			Id = Guid.NewGuid(),
			UserId = userId,
			Items = new List<CartItem>
			{
				new CartItem
				{
					Id = Guid.NewGuid(),
					ProductId = productId,
					Quantity = 5,
					PriceAtAdd = 2.99m
				}
			}
		};

		var user = new User
		{
			Id = userId,
			Email = "test@example.com",
			FirstName = "John",
			LastName = "Doe"
		};

		_mockCartRepository
			.Setup(r => r.GetCartByUserIdAsync(userId))
			.ReturnsAsync(cart);

		_mockCartRepository
			.Setup(r => r.GetCartWithItemsAsync(cart.Id))
			.ReturnsAsync(cart);

		_mockProductRepository
			.Setup(r => r.GetByIdAsync(productId))
			.ReturnsAsync(product);

		_mockPaymentService
			.Setup(s => s.ProcessPaymentAsync(
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<decimal>()))
			.ReturnsAsync(new PaymentResult
			{
				Success = true,
				TransactionId = "TXN-123456",
				Message = "Success"
			});

		_mockUserRepository
			.Setup(r => r.GetByIdAsync(userId))
			.ReturnsAsync(user);

		_mockOrderRepository
			.Setup(r => r.AddAsync(It.IsAny<Order>()))
			.ReturnsAsync((Order o) => o);

		_mockOrderRepository
			.Setup(r => r.SaveChangesAsync())
			.ReturnsAsync(true);

		var command = new CreateOrderCommand
		{
			UserId = userId,
			CardNumber = "4111111111111111",
			CardHolderName = "John Doe",
			ExpiryMonth = "12",
			ExpiryYear = "2025",
			Cvv = "123"
		};

		// Act
		var order = await _handler.HandleAsync(command);

		// Assert
		Assert.NotNull(order);
		Assert.Equal(userId, order.UserId);
		Assert.Equal(OrderStatus.Completed, order.Status);
		Assert.NotEmpty(order.OrderNumber);

		_mockEmailService.Verify(
			s => s.SendOrderConfirmationAsync(
				user.Email,
				It.IsAny<string>(),
				It.IsAny<decimal>()),
			Times.Once);
	}
}