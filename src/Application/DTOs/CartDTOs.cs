namespace Application.DTOs;

public record CartDto(
	Guid Id,
	Guid UserId,
	List<CartItemDto> Items,
	decimal TotalAmount
);

public record CartItemDto(
	Guid Id,
	Guid ProductId,
	string ProductName,
	int Quantity,
	decimal UnitPrice,
	decimal Subtotal
);

public record AddToCartRequest(
	Guid ProductId,
	int Quantity
);

public record UpdateCartItemRequest(
	int Quantity
);