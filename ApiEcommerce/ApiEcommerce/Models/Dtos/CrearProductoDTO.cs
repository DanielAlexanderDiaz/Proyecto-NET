using System;

namespace ApiEcommerce.Models.Dtos;

public class CrearProductoDTO
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string? ImgUrl { get; set; }
    public IFormFile? Image { get; set; }
    public string SKU { get; set; } = string.Empty;
    public int Stock { get; set; }
    public DateTime? FechaActualizacion { get; set; } = null;
    public int CategoriaId { get; set; }

}
