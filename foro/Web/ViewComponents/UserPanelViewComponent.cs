using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using foro.Domain.Entities;

namespace foro.Web.ViewComponents;

/// <summary>
/// ViewComponent para mostrar panel del usuario autenticado
/// </summary>
public class UserPanelViewComponent : ViewComponent
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserPanelViewComponent(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (User?.Identity?.IsAuthenticated == true)
        {
     var user = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);
          return View("Default", user);
        }

    return View("Guest");
    }
}
