using Microsoft.AspNetCore.Identity;

namespace foro.Domain.Entities;

/// <summary>
/// Usuario de la aplicación que extiende IdentityUser para integración con ASP.NET Core Identity
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Nombre completo del usuario
    /// </summary>
    public string NombreCompleto { get; set; } = string.Empty;

    /// <summary>
 /// Biografía o descripción del usuario
    /// </summary>
    public string? Biografia { get; set; }

    /// <summary>
    /// Fecha de registro en el foro
    /// </summary>
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indica si el usuario está activo
    /// </summary>
    public bool Activo { get; set; } = true;

    /// <summary>
    /// URL de la foto de perfil
    /// </summary>
    public string? FotoPerfil { get; set; }

 // Navegación
    /// <summary>
    /// Temas creados por este usuario
    /// </summary>
  public virtual ICollection<Tema> Temas { get; set; } = new List<Tema>();

    /// <summary>
    /// Mensajes publicados por este usuario
    /// </summary>
    public virtual ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
}
