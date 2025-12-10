using Domain.Interfaces;
using Application.DTOs;

namespace Application.Queries;

public class GetStoresQuery { }

public class GetStoresQueryHandler
{
	private readonly IStoreRepository _storeRepository;

	public GetStoresQueryHandler(IStoreRepository storeRepository)
	{
		_storeRepository = storeRepository;
	}

	public async Task<IEnumerable<StoreDto>> HandleAsync()
	{
		var stores = await _storeRepository.GetActiveStoresAsync();
		return stores.Select(s => new StoreDto(s.Id, s.Name, s.Description, s.Address, s.IsActive));
	}
}