using System;

namespace ApiEcommerce.Models.Dtos;

public class UsuarioRegisterDTO
{
    public string? Id {get; set; }
    public string? Nombre {get; set; }
    public required string NombreUsuario {get; set; }
    public required string Password {get; set; }
    public string? Role {get; set; }
}
