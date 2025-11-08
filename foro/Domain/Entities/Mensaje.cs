using System.ComponentModel.DataAnnotations;

namespace foro.Domain.Entities;

/// <summary>
/// Representa una respuesta o comentario dentro de un tema
/// </summary>
public class Mensaje
{
  public int Id { get; set; }

    [Required(ErrorMessage = "El contenido del mensaje es obligatorio")]
    [StringLength(5000, MinimumLength = 1, ErrorMessage = "El contenido debe tener entre 1 y 5000 caracteres")]
    public string Contenido { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaEdicion { get; set; }

    public bool Editado { get; set; } = false;

    // Claves foráneas
    [Required]
    public int TemaId { get; set; }

    [Required]
 public int UsuarioId { get; set; }

    // Propiedades de navegación
    public virtual Tema Tema { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;
}
