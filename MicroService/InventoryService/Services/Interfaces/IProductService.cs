using InventoryService.Dtos;
using InventoryService.Response;

namespace InventoryService.Services.Interfaces
{
    public interface IProductService
    {

        Task<List<ProductoDto>> getAllProducts();

        Task<ProductoDto> getProductById(Guid id);

        Task<ProductoDto> postProductNew(NewProductDto productDto);

        Task<bool> updateProduct(Guid id,ProductoDto productDto);

        Task<bool> deleteProduct(Guid id);

    }
}
