using System.ComponentModel.DataAnnotations;

namespace foro.Web.ViewModels;

/// <summary>
/// ViewModel para el formulario de registro de usuarios
/// </summary>
public class RegisterViewModel
{
    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    [Display(Name = "Nombre Completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido")]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Contraseña")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La biografía no puede exceder 500 caracteres")]
    [Display(Name = "Biografía (Opcional)")]
    public string? Biografia { get; set; }

    [Display(Name = "Acepto los términos y condiciones")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "Debes aceptar los términos y condiciones")]
    public bool AceptaTerminos { get; set; }

    public string? ReturnUrl { get; set; }
}
