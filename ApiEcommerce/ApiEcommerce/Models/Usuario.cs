using System;
using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models;

public class Usuario
{
    [Key]
    public int Id {get; set; }
    public string? Nombre {get; set; }
    public string NombreUsuario {get; set; } = string.Empty;
    public string? Password {get; set; }
    public string? Role {get; set; }
}
