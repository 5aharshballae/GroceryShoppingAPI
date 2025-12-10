using Domain.Entities;
using Domain.Interfaces;
using Application.DTOs;

namespace Application.Queries;

public class GetProductsQuery
{
	public Guid? CategoryId { get; set; }
	public Guid? StoreId { get; set; }
}

public class GetProductsQueryHandler
{
	private readonly IProductRepository _productRepository;

	public GetProductsQueryHandler(IProductRepository productRepository)
	{
		_productRepository = productRepository;
	}

	public async Task<IEnumerable<ProductDto>> HandleAsync(GetProductsQuery query)
	{
		IEnumerable<Product> products;

		if (query.CategoryId.HasValue)
		{
			products = await _productRepository.GetProductsByCategoryAsync(query.CategoryId.Value);
		}
		else if (query.StoreId.HasValue)
		{
			products = await _productRepository.GetProductsByStoreAsync(query.StoreId.Value);
		}
		else
		{
			products = await _productRepository.GetAllAsync();
		}

		return products.Select(p => new ProductDto(
			p.Id,
			p.Name,
			p.Description,
			p.Price,
			p.StockQuantity,
			p.ImageUrl,
			p.CategoryId,
			p.Category?.Name ?? string.Empty,
			p.IsAvailable
		));
	}
}