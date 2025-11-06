using System.ComponentModel.DataAnnotations;

namespace SalesSuite.Web.DTOs;

/// <summary>
/// DTO para mostrar información de un tema
/// </summary>
public class TemaDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "El título debe tener entre 5 y 200 caracteres")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El contenido es obligatorio")]
    [StringLength(5000, MinimumLength = 10, ErrorMessage = "El contenido debe tener entre 10 y 5000 caracteres")]
    [Display(Name = "Contenido")]
    [DataType(DataType.MultilineText)]
    public string Contenido { get; set; } = string.Empty;

    [Display(Name = "Slug")]
    public string Slug { get; set; } = string.Empty;

    [Display(Name = "Número de Vistas")]
    public int NumeroVistas { get; set; }

    [Display(Name = "Cerrado")]
    public bool Cerrado { get; set; }

    [Display(Name = "Fijado")]
    public bool Fijado { get; set; }

    [Display(Name = "Fecha de Creación")]
    [DataType(DataType.DateTime)]
    public DateTime FechaCreacion { get; set; }

    [Display(Name = "Última Actividad")]
    [DataType(DataType.DateTime)]
    public DateTime? FechaUltimaActividad { get; set; }

    [Required(ErrorMessage = "Debe seleccionar una categoría")]
    [Display(Name = "Categoría")]
    public int CategoriaId { get; set; }

    [Display(Name = "Categoría")]
    public string? CategoriaNombre { get; set; }

    [Display(Name = "Autor")]
    public int UsuarioId { get; set; }

    [Display(Name = "Autor")]
    public string? UsuarioNombre { get; set; }

    [Display(Name = "Número de Respuestas")]
    public int NumeroRespuestas { get; set; }
}

/// <summary>
/// DTO para crear un nuevo tema con su primer mensaje
/// </summary>
public class CrearTemaDto
{
    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "El título debe tener entre 5 y 200 caracteres")]
    [Display(Name = "Título del Tema")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El contenido del mensaje es obligatorio")]
    [StringLength(5000, MinimumLength = 10, ErrorMessage = "El contenido debe tener entre 10 y 5000 caracteres")]
    [Display(Name = "Contenido del Primer Mensaje")]
    [DataType(DataType.MultilineText)]
    public string ContenidoMensaje { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar una categoría")]
    [Display(Name = "Categoría")]
    public int CategoriaId { get; set; }

    [Display(Name = "Fijar este tema")]
    public bool Fijado { get; set; }
}

/// <summary>
/// DTO para crear un nuevo tema (versión simple sin mensaje)
/// </summary>
public class TemaCreateDto
{
    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "El título debe tener entre 5 y 200 caracteres")]
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

    [Display(Name = "Fijado")]
    public bool Fijado { get; set; }
}

/// <summary>
/// DTO para editar un tema existente
/// </summary>
public class TemaEditDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "El título debe tener entre 5 y 200 caracteres")]
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

    [Display(Name = "Cerrado")]
    public bool Cerrado { get; set; }

    [Display(Name = "Fijado")]
    public bool Fijado { get; set; }
}
