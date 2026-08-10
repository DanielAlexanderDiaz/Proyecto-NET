using System;

namespace ApiEcommerce.Models.Dtos;

public class UsuarioDTO
{
    public string Id {get; set; } = string.Empty;
    public string? Nombre {get; set; }
    public string? NombreUsuario {get; set; }
    public string? Password {get; set; }
    public string? Role {get; set; }
}
