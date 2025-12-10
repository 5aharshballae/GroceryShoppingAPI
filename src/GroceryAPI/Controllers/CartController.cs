using System.Security.Claims;
using Application.Commands;
using Application.DTOs;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
	private readonly AddToCartCommandHandler _addToCartHandler;
	private readonly ICartRepository _cartRepository;

	public CartController(
		AddToCartCommandHandler addToCartHandler,
		ICartRepository cartRepository)
	{
		_addToCartHandler = addToCartHandler;
		_cartRepository = cartRepository;
	}

	[HttpGet]
	public async Task<ActionResult<CartDto>> GetCart()
	{
		var userId = GetCurrentUserId();
		var cart = await _cartRepository.GetCartByUserIdAsync(userId);

		if (cart == null)
		{
			return NotFound(new { message = "Cart not found" });
		}

		cart = await _cartRepository.GetCartWithItemsAsync(cart.Id);

		var cartDto = new CartDto(
			cart!.Id,
			cart.UserId,
			cart.Items.Select(i => new CartItemDto(
				i.Id,
				i.ProductId,
				i.Product.Name,
				i.Quantity,
				i.Product.Price,
				i.Product.Price * i.Quantity
			)).ToList(),
			cart.Items.Sum(i => i.Product.Price * i.Quantity)
		);

		return Ok(cartDto);
	}

	[HttpPost("items")]
	public async Task<ActionResult<CartDto>> AddToCart([FromBody] AddToCartRequest request)
	{
		try
		{
			var userId = GetCurrentUserId();
			var command = new AddToCartCommand
			{
				UserId = userId,
				ProductId = request.ProductId,
				Quantity = request.Quantity
			};

			await _addToCartHandler.HandleAsync(command);

			return await GetCart();
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	[HttpPut("items/{itemId}")]
	public async Task<ActionResult<CartDto>> UpdateCartItem(Guid itemId, [FromBody] UpdateCartItemRequest request)
	{
		var userId = GetCurrentUserId();
		var cart = await _cartRepository.GetCartByUserIdAsync(userId);

		if (cart == null)
		{
			return NotFound(new { message = "Cart not found" });
		}

		cart = await _cartRepository.GetCartWithItemsAsync(cart.Id);
		var item = cart!.Items.FirstOrDefault(i => i.Id == itemId);

		if (item == null)
		{
			return NotFound(new { message = "Cart item not found" });
		}

		if (request.Quantity <= 0)
		{
			cart.Items.Remove(item);
		}
		else
		{
			item.Quantity = request.Quantity;
		}

		cart.UpdatedAt = DateTime.UtcNow;
		await _cartRepository.UpdateAsync(cart);
		await _cartRepository.SaveChangesAsync();

		return await GetCart();
	}

	[HttpDelete("items/{itemId}")]
	public async Task<ActionResult> RemoveCartItem(Guid itemId)
	{
		var userId = GetCurrentUserId();
		var cart = await _cartRepository.GetCartByUserIdAsync(userId);

		if (cart == null)
		{
			return NotFound(new { message = "Cart not found" });
		}

		cart = await _cartRepository.GetCartWithItemsAsync(cart.Id);
		var item = cart!.Items.FirstOrDefault(i => i.Id == itemId);

		if (item == null)
		{
			return NotFound(new { message = "Cart item not found" });
		}

		cart.Items.Remove(item);
		cart.UpdatedAt = DateTime.UtcNow;
		await _cartRepository.UpdateAsync(cart);
		await _cartRepository.SaveChangesAsync();

		return NoContent();
	}

	[HttpDelete]
	public async Task<ActionResult> ClearCart()
	{
		var userId = GetCurrentUserId();
		var cart = await _cartRepository.GetCartByUserIdAsync(userId);

		if (cart == null)
		{
			return NotFound(new { message = "Cart not found" });
		}

		await _cartRepository.ClearCartAsync(cart.Id);
		return NoContent();
	}

	private Guid GetCurrentUserId()
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		return Guid.Parse(userIdClaim!);
	}
}