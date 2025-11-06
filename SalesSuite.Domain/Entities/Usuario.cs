using System.ComponentModel.DataAnnotations;

namespace SalesSuite.Domain.Entities;

/// <summary>
/// Entidad que representa un usuario participante del foro
/// </summary>
public class Usuario
{
    /// <summary>
    /// Identificador único del usuario
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre de usuario único
    /// </summary>
    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres")]
    public string NombreUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico del usuario
    /// </summary>
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [StringLength(150, ErrorMessage = "El email no puede exceder 150 caracteres")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nombre completo del usuario
    /// </summary>
    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre completo no puede exceder 100 caracteres")]
    public string NombreCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña hasheada del usuario
    /// </summary>
    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(255, ErrorMessage = "La contraseña no puede exceder 255 caracteres")]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Biografía o descripción del usuario
    /// </summary>
    [StringLength(500, ErrorMessage = "La biografía no puede exceder 500 caracteres")]
    public string? Biografia { get; set; }

    /// <summary>
    /// URL del avatar del usuario
    /// </summary>
    [StringLength(500, ErrorMessage = "La URL del avatar no puede exceder 500 caracteres")]
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Fecha de registro del usuario
    /// </summary>
    [Required]
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha del último acceso del usuario
    /// </summary>
    public DateTime? UltimoAcceso { get; set; }

    /// <summary>
    /// Indica si el usuario está activo
    /// </summary>
    public bool Activo { get; set; } = true;

    /// <summary>
    /// Rol del usuario (Administrador, Moderador, Usuario)
    /// </summary>
    [Required]
    [StringLength(50, ErrorMessage = "El rol no puede exceder 50 caracteres")]
    public string Rol { get; set; } = "Usuario";

    // Propiedades de navegación
    /// <summary>
    /// Colección de temas creados por el usuario
    /// </summary>
    public virtual ICollection<Tema> TemasCreados { get; set; } = new List<Tema>();

    /// <summary>
    /// Colección de mensajes escritos por el usuario
    /// </summary>
    public virtual ICollection<Mensaje> MensajesEscritos { get; set; } = new List<Mensaje>();
}
