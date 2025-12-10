namespace Application.DTOs;

public record OrderDto(
	Guid Id,
	string OrderNumber,
	decimal TotalAmount,
	string Status,
	DateTime CreatedAt,
	List<OrderItemDto> Items
);

public record OrderItemDto(
	Guid ProductId,
	string ProductName,
	int Quantity,
	decimal UnitPrice,
	decimal Subtotal
);

public record CheckoutRequest(
	string CardNumber,
	string CardHolderName,
	string ExpiryMonth,
	string ExpiryYear,
	string Cvv
);

public record CheckoutResponse(
	Guid OrderId,
	string OrderNumber,
	decimal TotalAmount,
	string Status,
	string Message
);
