using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SalesSuite.Domain.Entities;

namespace SalesSuite.Web.ViewComponents;

/// <summary>
/// ViewComponent para mostrar el panel del usuario autenticado
/// </summary>
public class UsuarioPanelViewComponent : ViewComponent
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UsuarioPanelViewComponent> _logger;

    public UsuarioPanelViewComponent(
        UserManager<ApplicationUser> userManager,
        ILogger<UsuarioPanelViewComponent> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Invoca el ViewComponent para obtener información del usuario
    /// </summary>
    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return View("Anonymous");
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return View("Anonymous");
            }

            return View(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener información del usuario");
            return View("Anonymous");
        }
    }
}
