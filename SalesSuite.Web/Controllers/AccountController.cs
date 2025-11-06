using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SalesSuite.Domain.Entities;
using SalesSuite.Web.ViewModels;

namespace SalesSuite.Web.Controllers;

/// <summary>
/// Controlador para gestionar la autenticación y registro de usuarios
/// </summary>
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Muestra el formulario de inicio de sesión
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    /// <summary>
    /// Procesa el inicio de sesión
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // Intentar iniciar sesión
            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("Usuario {Email} inició sesión exitosamente", model.Email);
                
                // Actualizar último acceso
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    user.UltimoAcceso = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);
                }

                // Redireccionar
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("Cuenta bloqueada: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Cuenta bloqueada. Intente más tarde.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al iniciar sesión: {Email}", model.Email);
            ModelState.AddModelError(string.Empty, "Error al iniciar sesión. Intente nuevamente.");
            return View(model);
        }
    }

    /// <summary>
    /// Muestra el formulario de registro
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    /// <summary>
    /// Procesa el registro de un nuevo usuario
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // Crear el usuario
            var user = new ApplicationUser
            {
                UserName = model.Email, // Usar email como username
                Email = model.Email,
                NombreCompleto = model.NombreCompleto,
                Biografia = model.Biografia,
                FechaRegistro = DateTime.UtcNow,
                Activo = true,
                EmailConfirmed = true // Auto-confirmar para desarrollo
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("Usuario {Email} registrado exitosamente", user.Email);

                // Asignar rol "Usuario" por defecto
                await _userManager.AddToRoleAsync(user, "Usuario");

                // Iniciar sesión automáticamente
                await _signInManager.SignInAsync(user, isPersistent: false);

                TempData["Success"] = "Registro exitoso. ¡Bienvenido al foro!";
                return RedirectToAction("Index", "Home");
            }

            // Agregar errores al ModelState
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar usuario: {Email}", model.Email);
            ModelState.AddModelError(string.Empty, "Error al registrar usuario. Intente nuevamente.");
            return View(model);
        }
    }

    /// <summary>
    /// Cierra la sesión del usuario
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var userEmail = User.Identity?.Name;
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Usuario {Email} cerró sesión", userEmail);
            
            TempData["Success"] = "Sesión cerrada exitosamente.";
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cerrar sesión");
            return RedirectToAction("Index", "Home");
        }
    }

    /// <summary>
    /// Página de acceso denegado
    /// </summary>
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
