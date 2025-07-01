using InventoryService.Dtos;
using InventoryService.Response;
using InventoryService.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace InventoryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            return Ok(await _productService.getAllProducts());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var producto = await _productService.getProductById(id);
            return producto is null ? NotFound($"Producto con ID {id} no encontrado") : Ok(producto);
        }

        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] NewProductDto newProductDto)
        {
            return Ok(await _productService.postProductNew(newProductDto));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody]  ProductoDto productoDto)
        {
            if (!id.Equals(productoDto.Id))
            {
                return BadRequest($"No coincide Id de URl con el producto a actualizar.");
            }
            var updateProduct = await _productService.updateProduct(id, productoDto);
            return updateProduct ? NoContent() : NotFound($"Producto con ID {id} no encontrado");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var deletedProduct = await _productService.deleteProduct(id);
            return deletedProduct ? NoContent() : NotFound($"Producto con ID {id} no encontrado");
        }
    }
}
