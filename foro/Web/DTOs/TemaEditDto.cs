using System.ComponentModel.DataAnnotations;

namespace foro.Web.DTOs;

/// <summary>
/// DTO para editar un tema existente
/// </summary>
public class TemaEditDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(250, MinimumLength = 5, ErrorMessage = "El título debe tener entre 5 y 250 caracteres")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar una categoría")]
    [Display(Name = "Categoría")]
    public int CategoriaId { get; set; }

    public int UsuarioId { get; set; }

    public DateTime FechaCreacion { get; set; }
}
