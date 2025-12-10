using Domain.Entities;

namespace Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
	Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId);
	Task<IEnumerable<Product>> GetProductsByStoreAsync(Guid storeId);
	Task<bool> UpdateStockAsync(Guid productId, int quantity);
	Task<Product?> GetProductWithCategoryAsync(Guid productId);
}