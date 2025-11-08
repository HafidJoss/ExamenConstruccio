using System.ComponentModel.DataAnnotations;

namespace foro.Web.DTOs;

/// <summary>
/// DTO para crear un nuevo tema
/// </summary>
public class TemaCreateDto
{
    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(250, MinimumLength = 5, ErrorMessage = "El título debe tener entre 5 y 250 caracteres")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El contenido es obligatorio")]
    [StringLength(5000, MinimumLength = 10, ErrorMessage = "El contenido debe tener entre 10 y 5000 caracteres")]
    [Display(Name = "Contenido")]
    [DataType(DataType.MultilineText)]
    public string Contenido { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar una categoría")]
    [Display(Name = "Categoría")]
    public int CategoriaId { get; set; }

    [Required]
    public int UsuarioId { get; set; }
}
