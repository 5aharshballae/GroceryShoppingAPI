using Domain.Entities;

namespace Domain.Interfaces;

public interface IStoreRepository : IRepository<Store>
{
	Task<IEnumerable<Store>> GetActiveStoresAsync();
	Task<Store?> GetStoreWithCategoriesAsync(Guid storeId);
}