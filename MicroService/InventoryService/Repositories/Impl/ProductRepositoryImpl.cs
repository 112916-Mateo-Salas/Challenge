using InventoryService.Context;
using InventoryService.Models;
using InventoryService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Repositories.Impl
{
    public class ProductRepositoryImpl : IProductRepository
    {
        private readonly InventoryContext _context;

        public ProductRepositoryImpl(InventoryContext inventoryContext)
        {
            _context = inventoryContext;
        }
        public async Task AddProduct(Producto product)
        {
            try
            {
                _context.Productos.Add(product);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                throw new Exception("Error al agregar el producto a la base de datos.", ex);
            }

            
        }

        public Task DeleteProduct(Producto producto)
        {
            try
            {
                _context.Productos.Remove(producto);
                _context.SaveChangesAsync();
                return Task.CompletedTask;
            } catch (Exception ex)
            {
                throw new Exception($"Error al eliminar el producto con ID {producto.Id}.", ex);
            }
        }

        public async Task<IEnumerable<Producto>> getAll()
        {
            try
            {
               return await _context.Productos.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de productos desde la base de datos.", ex);
            }
        }

        public async Task<Producto> getById(Guid id)
        {
            try
            {
                var producto = await _context.Productos.Where(p => p.Id.Equals(id)).FirstOrDefaultAsync();
                return producto;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar el producto con ID {id}.", ex);
            }
        }

        public async Task UpdateProduct(Producto product)
        {
            try
            {
                _context.Productos.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar el producto con ID {product.Id}.", ex);
            }
        }
    }
}
