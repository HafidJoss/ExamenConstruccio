using Microsoft.AspNetCore.Mvc;
using foro.Domain.Interfaces;

namespace foro.Web.ViewComponents;

/// <summary>
/// ViewComponent para mostrar estadísticas del foro
/// </summary>
public class ForumStatsViewComponent : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;

    public ForumStatsViewComponent(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
      try
   {
       var temas = await _unitOfWork.Temas.GetAllAsync();
        var mensajes = await _unitOfWork.Mensajes.GetAllAsync();
      var usuarios = await _unitOfWork.Usuarios.GetAllAsync();
    var categorias = await _unitOfWork.Categorias.GetAllAsync();

       var stats = new ForumStatsViewModel
            {
 TotalTemas = temas.Count(),
      TotalMensajes = mensajes.Count(),
TotalUsuarios = usuarios.Count(),
TotalCategorias = categorias.Count(c => c.Activa),
       TemasHoy = temas.Count(t => t.FechaCreacion.Date == DateTime.UtcNow.Date),
    MensajesHoy = mensajes.Count(m => m.FechaCreacion.Date == DateTime.UtcNow.Date)
   };

      return View(stats);
      }
        catch
   {
  return View(new ForumStatsViewModel());
  }
    }
}

public class ForumStatsViewModel
{
    public int TotalTemas { get; set; }
    public int TotalMensajes { get; set; }
    public int TotalUsuarios { get; set; }
    public int TotalCategorias { get; set; }
    public int TemasHoy { get; set; }
    public int MensajesHoy { get; set; }
}
