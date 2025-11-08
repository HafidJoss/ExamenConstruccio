using System.ComponentModel.DataAnnotations;

namespace foro.Web.DTOs;

/// <summary>
/// DTO para mostrar detalles de un tema antes de eliminarlo
/// </summary>
public class TemaDeleteDto
{
    public int Id { get; set; }

    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Display(Name = "Contenido")]
    public string Contenido { get; set; } = string.Empty;

    [Display(Name = "Categoría")]
    public string CategoriaNombre { get; set; } = string.Empty;

    [Display(Name = "Autor")]
    public string UsuarioNombre { get; set; } = string.Empty;

    [Display(Name = "Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

 [Display(Name = "Número de Mensajes")]
    public int NumeroMensajes { get; set; }
}
