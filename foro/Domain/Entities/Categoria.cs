using System.ComponentModel.DataAnnotations;

namespace foro.Domain.Entities;

/// <summary>
/// Agrupa los temas del foro por categoría
/// </summary>
public class Categoria
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la categoría es obligatorio")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Descripcion { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public bool Activa { get; set; } = true;

    public int Orden { get; set; } = 0;

    // Propiedades de navegación
    public virtual ICollection<Tema> Temas { get; set; } = new List<Tema>();
}
