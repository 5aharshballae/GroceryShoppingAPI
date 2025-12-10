using Domain.Entities;
using Domain.Interfaces;

namespace Application.Commands;

public class AddToCartCommand
{
	public Guid UserId { get; set; }
	public Guid ProductId { get; set; }
	public int Quantity { get; set; }
}

public class AddToCartCommandHandler
{
	private readonly ICartRepository _cartRepository;
	private readonly IProductRepository _productRepository;

	public AddToCartCommandHandler(ICartRepository cartRepository, IProductRepository productRepository)
	{
		_cartRepository = cartRepository;
		_productRepository = productRepository;
	}

	public async Task<Cart> HandleAsync(AddToCartCommand command)
	{
		var cart = await _cartRepository.GetCartByUserIdAsync(command.UserId);
		if (cart == null)
		{
			throw new InvalidOperationException("Cart not found");
		}

		cart = await _cartRepository.GetCartWithItemsAsync(cart.Id);

		var product = await _productRepository.GetByIdAsync(command.ProductId);
		if (product == null)
		{
			throw new InvalidOperationException("Product not found");
		}

		if (product.StockQuantity < command.Quantity)
		{
			throw new InvalidOperationException("Insufficient stock");
		}

		var existingItem = cart!.Items.FirstOrDefault(i => i.ProductId == command.ProductId);

		if (existingItem != null)
		{
			existingItem.Quantity += command.Quantity;
		}
		else
		{
			cart.Items.Add(new CartItem
			{
				Id = Guid.NewGuid(),
				CartId = cart.Id,
				ProductId = command.ProductId,
				Quantity = command.Quantity,
				PriceAtAdd = product.Price
			});
		}

		cart.UpdatedAt = DateTime.UtcNow;
		await _cartRepository.UpdateAsync(cart);
		await _cartRepository.SaveChangesAsync();

		return cart;
	}
}