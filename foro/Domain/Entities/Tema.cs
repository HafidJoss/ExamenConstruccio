using System.ComponentModel.DataAnnotations;

namespace foro.Domain.Entities;

/// <summary>
/// Representa un hilo de conversación creado por un usuario en una categoría
/// </summary>
public class Tema
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(250, MinimumLength = 5, ErrorMessage = "El título debe tener entre 5 y 250 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El contenido es obligatorio")]
    [StringLength(5000, MinimumLength = 10, ErrorMessage = "El contenido debe tener entre 10 y 5000 caracteres")]
    public string Contenido { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaUltimaActividad { get; set; }

    public bool Cerrado { get; set; } = false;

    public bool Fijado { get; set; } = false;

    public int Vistas { get; set; } = 0;

    // Claves foráneas
    [Required]
    public int UsuarioId { get; set; }

    [Required]
    public int CategoriaId { get; set; }

    // Propiedades de navegación
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual Categoria Categoria { get; set; } = null!;
  public virtual ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
}
