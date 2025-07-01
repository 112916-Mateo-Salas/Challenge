using InventoryService.Models;

namespace InventoryService.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Producto>> getAll();

        Task<Producto> getById(Guid id);

        Task AddProduct(Producto product);

        Task UpdateProduct(Producto product);

        Task DeleteProduct(Producto product);
    }
}
