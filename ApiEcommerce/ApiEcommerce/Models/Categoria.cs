using System.ComponentModel.DataAnnotations;

public class Categoria
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Nombre { get; set; } = string.Empty;
    [Required]
    public DateTime Creacion { get; set; }
}