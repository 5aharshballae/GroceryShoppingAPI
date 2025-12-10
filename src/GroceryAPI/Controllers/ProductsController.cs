using Application.DTOs;
using Application.Queries;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
	private readonly GetProductsQueryHandler _getProductsHandler;
	private readonly IProductRepository _productRepository;

	public ProductsController(
		GetProductsQueryHandler getProductsHandler,
		IProductRepository productRepository)
	{
		_getProductsHandler = getProductsHandler;
		_productRepository = productRepository;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(
		[FromQuery] Guid? categoryId,
		[FromQuery] Guid? storeId)
	{
		var query = new GetProductsQuery
		{
			CategoryId = categoryId,
			StoreId = storeId
		};

		var products = await _getProductsHandler.HandleAsync(query);
		return Ok(products);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
	{
		var product = await _productRepository.GetProductWithCategoryAsync(id);

		if (product == null)
		{
			return NotFound(new { message = "Product not found" });
		}

		return Ok(new ProductDto(
			product.Id,
			product.Name,
			product.Description,
			product.Price,
			product.StockQuantity,
			product.ImageUrl,
			product.CategoryId,
			product.Category.Name,
			product.IsAvailable
		));
	}
}