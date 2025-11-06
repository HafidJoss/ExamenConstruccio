using Microsoft.AspNetCore.Identity;

namespace SalesSuite.Domain.Entities;

/// <summary>
/// Usuario de la aplicación extendiendo IdentityUser para ASP.NET Core Identity
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
    /// URL del avatar del usuario
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Fecha de registro en el foro
    /// </summary>
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha del último acceso
    /// </summary>
    public DateTime? UltimoAcceso { get; set; }

    /// <summary>
    /// Indica si el usuario está activo
    /// </summary>
    public bool Activo { get; set; } = true;

    // Navegación: Temas creados por el usuario
    public virtual ICollection<Tema> Temas { get; set; } = new List<Tema>();

    // Navegación: Mensajes creados por el usuario
    public virtual ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
}
