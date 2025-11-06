using System.ComponentModel.DataAnnotations;

namespace SalesSuite.Web.DTOs;

/// <summary>
/// DTO para mostrar información de un usuario
/// </summary>
public class UsuarioDto
{
    public int Id { get; set; }

    [Display(Name = "Nombre de Usuario")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Nombre Completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Display(Name = "Avatar")]
    public string? AvatarUrl { get; set; }

    [Display(Name = "Rol")]
    public string Rol { get; set; } = string.Empty;

    [Display(Name = "Fecha de Registro")]
    public DateTime FechaRegistro { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; }
}
