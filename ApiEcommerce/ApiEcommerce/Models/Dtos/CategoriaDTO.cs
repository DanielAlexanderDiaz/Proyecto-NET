using System;

namespace ApiEcommerce.Models.Dtos;

public class CategoriaDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateTime Creacion { get; set; }
}
