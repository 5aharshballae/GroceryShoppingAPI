using Application.DTOs;
using Application.Queries;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StoresController : ControllerBase
{
	private readonly GetStoresQueryHandler _getStoresHandler;
	private readonly IStoreRepository _storeRepository;

	public StoresController(
		GetStoresQueryHandler getStoresHandler,
		IStoreRepository storeRepository)
	{
		_getStoresHandler = getStoresHandler;
		_storeRepository = storeRepository;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<StoreDto>>> GetStores()
	{
		var stores = await _getStoresHandler.HandleAsync();
		return Ok(stores);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<StoreDto>> GetStore(Guid id)
	{
		var store = await _storeRepository.GetByIdAsync(id);

		if (store == null)
		{
			return NotFound(new { message = "Store not found" });
		}

		return Ok(new StoreDto(store.Id, store.Name, store.Description, store.Address, store.IsActive));
	}

	[HttpGet("{id}/categories")]
	public async Task<ActionResult<IEnumerable<CategoryDto>>> GetStoreCategories(Guid id)
	{
		var store = await _storeRepository.GetStoreWithCategoriesAsync(id);

		if (store == null)
		{
			return NotFound(new { message = "Store not found" });
		}

		var categories = store.Categories.Select(c => new CategoryDto(
			c.Id,
			c.Name,
			c.Description,
			c.StoreId
		));

		return Ok(categories);
	}
}