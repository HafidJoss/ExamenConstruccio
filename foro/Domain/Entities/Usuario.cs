using System.ComponentModel.DataAnnotations;

namespace foro.Domain.Entities;

/// <summary>
/// Representa un participante del foro
/// </summary>
public class Usuario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [StringLength(200, ErrorMessage = "El email no puede exceder 200 caracteres")]
    public string Email { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La biografía no puede exceder 500 caracteres")]
    public string? Biografia { get; set; }

 public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public bool Activo { get; set; } = true;

    // Propiedades de navegación
    public virtual ICollection<Tema> Temas { get; set; } = new List<Tema>();
  public virtual ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
}
