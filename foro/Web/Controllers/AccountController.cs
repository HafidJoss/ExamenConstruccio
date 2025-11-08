using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using foro.Domain.Entities;
using foro.Web.ViewModels;

namespace foro.Web.Controllers;

/// <summary>
/// Controlador para gestionar autenticación y registro de usuarios
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

    #region Login

    /// <summary>
    /// GET: /Account/Login
    /// Muestra el formulario de inicio de sesión
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        var model = new LoginViewModel { ReturnUrl = returnUrl };
        return View(model);
    }

  /// <summary>
    /// POST: /Account/Login
    /// Procesa el inicio de sesión
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
  {
        ViewData["ReturnUrl"] = returnUrl;
        model.ReturnUrl = returnUrl; // Asegurar que el modelo tenga el returnUrl

        if (!ModelState.IsValid)
   {
   return View(model);
        }

    try
  {
// Buscar usuario por email
    var user = await _userManager.FindByEmailAsync(model.Email);
     
            if (user == null)
            {
             ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
      return View(model);
            }

       // Verificar si el usuario está activo
 if (!user.Activo)
            {
     ModelState.AddModelError(string.Empty, "Tu cuenta ha sido desactivada. Contacta al administrador.");
     return View(model);
            }

            // Intentar iniciar sesión
            var result = await _signInManager.PasswordSignInAsync(
 user.UserName!,
       model.Password,
      model.RememberMe,
      lockoutOnFailure: true
            );

            if (result.Succeeded)
      {
        _logger.LogInformation("Usuario {Email} inició sesión exitosamente.", model.Email);
          
            // Redirigir a la URL de retorno o al inicio
if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
          return Redirect(returnUrl);
 }
     
        return RedirectToAction("Index", "Temas");
   }

       if (result.IsLockedOut)
          {
 _logger.LogWarning("Usuario {Email} está bloqueado.", model.Email);
       ModelState.AddModelError(string.Empty, "Tu cuenta ha sido bloqueada temporalmente por múltiples intentos fallidos.");
     return View(model);
            }

       if (result.RequiresTwoFactor)
            {
     return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
            }

            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
     return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el inicio de sesión");
  ModelState.AddModelError(string.Empty, "Error al procesar tu solicitud. Intenta nuevamente.");
            return View(model);
        }
    }

    #endregion

    #region Register

    /// <summary>
    /// GET: /Account/Register
  /// Muestra el formulario de registro
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        var model = new RegisterViewModel { ReturnUrl = returnUrl };
        return View(model);
    }

    /// <summary>
 /// POST: /Account/Register
    /// Procesa el registro de un nuevo usuario
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
      model.ReturnUrl = returnUrl; // Asegurar que el modelo tenga el returnUrl

        if (!ModelState.IsValid)
        {
       return View(model);
 }

  try
   {
       // Crear nuevo usuario
  var user = new ApplicationUser
   {
                UserName = model.Email,
         Email = model.Email,
   NombreCompleto = model.NombreCompleto,
       Biografia = model.Biografia,
      FechaRegistro = DateTime.UtcNow,
  Activo = true,
    EmailConfirmed = true
  };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
     {
     _logger.LogInformation("Nuevo usuario registrado: {Email}", user.Email);

     // Asignar rol "Usuario" por defecto
     if (!await _userManager.IsInRoleAsync(user, "Usuario"))
     {
         await _userManager.AddToRoleAsync(user, "Usuario");
                }

 // Iniciar sesión automáticamente
        await _signInManager.SignInAsync(user, isPersistent: false);

   TempData["Success"] = "¡Bienvenido al foro! Tu cuenta ha sido creada exitosamente.";

        // Redirigir a la URL de retorno o al inicio
              if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
       {
    return Redirect(returnUrl);
                }

          return RedirectToAction("Index", "Temas");
       }

            // Si hay errores, agregarlos al ModelState
 foreach (var error in result.Errors)
{
        ModelState.AddModelError(string.Empty, error.Description);
         }

            return View(model);
        }
        catch (Exception ex)
    {
  _logger.LogError(ex, "Error durante el registro de usuario");
    ModelState.AddModelError(string.Empty, "Error al crear tu cuenta. Intenta nuevamente.");
            return View(model);
    }
    }

  #endregion

 #region Logout

    /// <summary>
    /// POST: /Account/Logout
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
   
            _logger.LogInformation("Usuario {Email} cerró sesión.", userEmail);
            
          TempData["Info"] = "Has cerrado sesión exitosamente.";
            return RedirectToAction("Index", "Home");
    }
 catch (Exception ex)
{
            _logger.LogError(ex, "Error durante el cierre de sesión");
            return RedirectToAction("Index", "Home");
        }
    }

    #endregion

    #region Helpers

    /// <summary>
    /// GET: /Account/AccessDenied
    /// Página mostrada cuando el usuario no tiene permisos
    /// </summary>
    [HttpGet]
    public IActionResult AccessDenied()
    {
        ViewData["Message"] = "No tienes permisos para acceder a esta sección.";
        return View();
  }

    /// <summary>
 /// Placeholder para autenticación de dos factores (2FA)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult LoginWith2fa(string returnUrl, bool rememberMe)
    {
        // TODO: Implementar 2FA en el futuro
        return RedirectToAction(nameof(Login));
    }

    #endregion
}
