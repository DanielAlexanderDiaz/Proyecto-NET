using System;
using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models.Dtos;

public class CrearCategoriaDTO
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
    [MinLength(3, ErrorMessage = "El nombre debe tener al menos 3 caracteres")]
    public string Nombre { get; set; } = string.Empty;
}
