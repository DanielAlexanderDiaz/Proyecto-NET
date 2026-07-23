using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiEcommerce.Models;

public class Producto
{
    [Key]
    public int ProductoId { get; set; }
    [Required]
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    [Range(0, double.MaxValue)]
    public decimal Precio { get; set; }
    public string ImgUrl { get; set; } = string.Empty;
    [Required]
    public string SKU { get; set; } = string.Empty;
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public DateTime? FechaActualizacion { get; set; } = null;
    public int CategoriaId { get; set; }
    [ForeignKey("CategoriaId")]
    public required Categoria Categoria { get; set; } = null!;
}
