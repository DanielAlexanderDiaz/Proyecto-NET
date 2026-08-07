using System;

namespace ApiEcommerce.Models.Dtos;

public class UsuarioLoginResponseDTO
{
    public UsuarioDataDTO? Usuario { get; set; }
    public string? Token { get; set; }
    public string? Mensaje { get; set; }
}
