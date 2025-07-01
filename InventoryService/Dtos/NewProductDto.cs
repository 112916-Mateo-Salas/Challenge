using System.ComponentModel.DataAnnotations;

namespace InventoryService.Dtos
{
    public class NewProductDto
    {
        [Required(ErrorMessage ="El Nombre es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La Descripción es requerido")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El Precio es requerido")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El Stock es requerido")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "La Categoría es requerida")]
        public string Categoria { get; set; }
    }
}
