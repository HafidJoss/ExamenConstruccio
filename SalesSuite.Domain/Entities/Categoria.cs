using System.ComponentModel.DataAnnotations;

namespace SalesSuite.Domain.Entities;

/// <summary>
/// Entidad que representa una categoría del foro
/// </summary>
public class Categoria
{
    /// <summary>
    /// Identificador único de la categoría
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre de la categoría
    /// </summary>
    [Required(ErrorMessage = "El nombre de la categoría es obligatorio")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Descripción de la categoría
    /// </summary>
    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Slug para URL amigable
    /// </summary>
    [Required]
    [StringLength(100, ErrorMessage = "El slug no puede exceder 100 caracteres")]
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Icono o emoji representativo de la categoría
    /// </summary>
    [StringLength(50, ErrorMessage = "El icono no puede exceder 50 caracteres")]
    public string? Icono { get; set; }

    /// <summary>
    /// Orden de visualización de la categoría
    /// </summary>
    [Required]
    public int Orden { get; set; } = 0;

    /// <summary>
    /// Indica si la categoría está activa
    /// </summary>
    public bool Activa { get; set; } = true;

    /// <summary>
    /// Fecha de creación de la categoría
    /// </summary>
    [Required]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Propiedades de navegación
    /// <summary>
    /// Colección de temas que pertenecen a esta categoría
    /// </summary>
    public virtual ICollection<Tema> Temas { get; set; } = new List<Tema>();
}
