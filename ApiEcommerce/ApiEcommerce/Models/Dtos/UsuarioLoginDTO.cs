using System;
using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models.Dtos;

public class UsuarioLoginDTO
{
  [Required(ErrorMessage = "El campo name es requerido")]
  public string? Name { get; set; }
  [Required(ErrorMessage = "El campo password es requerido")]
  public string? Password { get; set; }
}
