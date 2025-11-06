using System.ComponentModel.DataAnnotations;

namespace SalesSuite.Domain.Entities;

/// <summary>
/// Entidad que representa un tema o hilo de conversación en el foro
/// </summary>
public class Tema
{
    /// <summary>
    /// Identificador único del tema
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Título del tema
    /// </summary>
    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "El título debe tener entre 5 y 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Contenido inicial del tema
    /// </summary>
    [Required(ErrorMessage = "El contenido es obligatorio")]
    [StringLength(5000, MinimumLength = 10, ErrorMessage = "El contenido debe tener entre 10 y 5000 caracteres")]
    public string Contenido { get; set; } = string.Empty;

    /// <summary>
    /// Slug para URL amigable
    /// </summary>
    [Required]
    [StringLength(250, ErrorMessage = "El slug no puede exceder 250 caracteres")]
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Número de vistas del tema
    /// </summary>
    [Required]
    public int NumeroVistas { get; set; } = 0;

    /// <summary>
    /// Indica si el tema está cerrado (no permite más respuestas)
    /// </summary>
    public bool Cerrado { get; set; } = false;

    /// <summary>
    /// Indica si el tema está fijado (aparece primero)
    /// </summary>
    public bool Fijado { get; set; } = false;

    /// <summary>
    /// Fecha de creación del tema
    /// </summary>
    [Required]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de la última actividad en el tema
    /// </summary>
    public DateTime? FechaUltimaActividad { get; set; }

    // Claves foráneas
    /// <summary>
    /// Identificador de la categoría a la que pertenece el tema
    /// </summary>
    [Required]
    public int CategoriaId { get; set; }

    /// <summary>
    /// Identificador del usuario que creó el tema
    /// </summary>
    [Required]
    public int UsuarioId { get; set; }

    // Propiedades de navegación
    /// <summary>
    /// Categoría a la que pertenece el tema
    /// </summary>
    public virtual Categoria Categoria { get; set; } = null!;

    /// <summary>
    /// Usuario que creó el tema
    /// </summary>
    public virtual Usuario Usuario { get; set; } = null!;

    /// <summary>
    /// Colección de mensajes o respuestas del tema
    /// </summary>
    public virtual ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
}
