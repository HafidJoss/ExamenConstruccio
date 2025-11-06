using System.ComponentModel.DataAnnotations;

namespace SalesSuite.Domain.Entities;

/// <summary>
/// Entidad que representa un mensaje o respuesta dentro de un tema del foro
/// </summary>
public class Mensaje
{
    /// <summary>
    /// Identificador único del mensaje
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Contenido del mensaje
    /// </summary>
    [Required(ErrorMessage = "El contenido del mensaje es obligatorio")]
    [StringLength(5000, MinimumLength = 1, ErrorMessage = "El contenido debe tener entre 1 y 5000 caracteres")]
    public string Contenido { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de creación del mensaje
    /// </summary>
    [Required]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de la última edición del mensaje
    /// </summary>
    public DateTime? FechaEdicion { get; set; }

    /// <summary>
    /// Indica si el mensaje ha sido editado
    /// </summary>
    public bool Editado { get; set; } = false;

    /// <summary>
    /// Número de "me gusta" o votos positivos
    /// </summary>
    [Required]
    public int NumeroMeGusta { get; set; } = 0;

    /// <summary>
    /// Indica si el mensaje está oculto o eliminado
    /// </summary>
    public bool Oculto { get; set; } = false;

    /// <summary>
    /// Razón por la que el mensaje fue ocultado
    /// </summary>
    [StringLength(500, ErrorMessage = "La razón no puede exceder 500 caracteres")]
    public string? RazonOculto { get; set; }

    // Claves foráneas
    /// <summary>
    /// Identificador del tema al que pertenece el mensaje
    /// </summary>
    [Required]
    public int TemaId { get; set; }

    /// <summary>
    /// Identificador del usuario que escribió el mensaje
    /// </summary>
    [Required]
    public int UsuarioId { get; set; }

    /// <summary>
    /// Identificador del mensaje al que responde (si es una respuesta)
    /// </summary>
    public int? MensajePadreId { get; set; }

    // Propiedades de navegación
    /// <summary>
    /// Tema al que pertenece el mensaje
    /// </summary>
    public virtual Tema Tema { get; set; } = null!;

    /// <summary>
    /// Usuario que escribió el mensaje
    /// </summary>
    public virtual Usuario Usuario { get; set; } = null!;

    /// <summary>
    /// Mensaje padre al que responde (si aplica)
    /// </summary>
    public virtual Mensaje? MensajePadre { get; set; }

    /// <summary>
    /// Colección de respuestas a este mensaje
    /// </summary>
    public virtual ICollection<Mensaje> Respuestas { get; set; } = new List<Mensaje>();
}
