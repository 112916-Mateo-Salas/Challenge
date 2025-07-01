using InventoryService.Dtos;
using InventoryService.Models;
using InventoryService.Repositories.Interfaces;
using InventoryService.Response;
using InventoryService.Services.Interfaces;

namespace InventoryService.Services.Impl
{
    public class ProductServiceImpl : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductServiceImpl(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<bool> deleteProduct(Guid id)
        {
            try
            {
                Producto producto = await _productRepository.getById(id);
                if (producto != null)
                {
                    await _productRepository.DeleteProduct(producto);
                    return true;
                }
                return false;
            } catch (Exception ex)
            {
                throw new Exception($"Error al eliminar producto con ID {id} desde productService", ex);
            }
            
        }

        public async Task<List<ProductoDto>> getAllProducts()
        {
            try
            {
                List<ProductoDto> productoDtos = new List<ProductoDto>();
                IEnumerable<Producto> productos = await _productRepository.getAll();
                if (productos != null)
                {
                    
                    foreach (Producto producto in productos)
                    {
                        ProductoDto productoDto = new ProductoDto
                        {
                            Id = producto.Id,
                            Nombre = producto.Nombre,
                            Descripcion = producto.Descripcion,
                            Precio = producto.Precio,
                            Stock = producto.Stock,
                            Categoria = producto.Categoria,
                        };
                        productoDtos.Add(productoDto);
                    }                    
                }
                return productoDtos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener todos los productos desde productSerivce", ex);
            }
        }

        public async Task<ProductoDto> getProductById(Guid id)
        {
            try
            {
                Producto producto = await _productRepository.getById(id);
                if (producto != null)
                {
                    ProductoDto productoDto = new ProductoDto
                    {
                        Id = producto.Id,
                        Nombre = producto.Nombre,
                        Descripcion = producto.Descripcion,
                        Precio = producto.Precio,
                        Stock = producto.Stock,
                        Categoria = producto.Categoria,
                    };
                    return productoDto;
                }
                return null;
            } catch (Exception ex)
            {
                throw new Exception($"Error al buscar producto con ID {id} desde productService.", ex);
            }
        }

        public async Task<ProductoDto> postProductNew(NewProductDto productDto)
        {
            try
            {
                Producto producto = new Producto
                {
                    Id = Guid.NewGuid(),
                    Nombre= productDto.Nombre,
                    Descripcion = productDto.Descripcion,
                    Precio = productDto.Precio,
                    Stock = productDto.Stock,
                    Categoria = productDto.Categoria
                };
                await _productRepository.AddProduct(producto);

                ProductoDto productoDto = new ProductoDto
                {
                    Id = producto.Id,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    Precio = producto.Precio,
                    Stock = producto.Stock,
                    Categoria = producto.Categoria,
                };
                return productoDto ;


            } catch (Exception ex)
            {
                throw new Exception("Error al crear producto desde productService.", ex);
            }
        }

        public async Task<bool> updateProduct(Guid id,ProductoDto productDto)
        {
            try
            {
                Producto productoExist = await _productRepository.getById(id);
                if (productoExist != null)
                {
                    productoExist.Nombre = productDto.Nombre;
                    productoExist.Descripcion = productDto.Descripcion;
                    productoExist.Stock = productDto.Stock;
                    productoExist.Precio = productDto.Precio;
                    productoExist.Categoria = productDto.Categoria;
                    await _productRepository.UpdateProduct(productoExist);
                    
                    return true;
                }
                return false;
            } catch (Exception ex)
            {
                throw new Exception($"Error al actualizar producto con ID {productDto.Id} desde productService.", ex);
            }
        }
    }
}
