using System;

namespace ApiEcommerce.Models.Dtos;

public class ProductoDTO
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string ImgUrl { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int Stock { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public DateTime? FechaActualizacion { get; set; } = null;
    public int CategoriaId { get; set; }

}
