using Microsoft.AspNetCore.Mvc;
using SalesSuite.Domain.Interfaces;

namespace SalesSuite.Web.ViewComponents;

/// <summary>
/// Modelo para las estadísticas del foro
/// </summary>
public class EstadisticasForoViewModel
{
    public int TotalTemas { get; set; }
    public int TotalMensajes { get; set; }
    public int TotalUsuarios { get; set; }
    public int TotalCategorias { get; set; }
    public int TemasHoy { get; set; }
    public int MensajesHoy { get; set; }
}

/// <summary>
/// ViewComponent para mostrar estadísticas del foro
/// </summary>
public class EstadisticasForoViewComponent : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EstadisticasForoViewComponent> _logger;

    public EstadisticasForoViewComponent(
        IUnitOfWork unitOfWork,
        ILogger<EstadisticasForoViewComponent> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Invoca el ViewComponent para obtener estadísticas
    /// </summary>
    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var hoy = DateTime.UtcNow.Date;

            var temas = await _unitOfWork.Temas.GetAsync();
            var mensajes = await _unitOfWork.Mensajes.GetAsync();
            var usuarios = await _unitOfWork.Usuarios.GetAsync();
            var categorias = await _unitOfWork.Categorias.GetAsync();

            var estadisticas = new EstadisticasForoViewModel
            {
                TotalTemas = temas.Count(),
                TotalMensajes = mensajes.Count(),
                TotalUsuarios = usuarios.Count(),
                TotalCategorias = categorias.Count(c => c.Activa),
                TemasHoy = temas.Count(t => t.FechaCreacion.Date == hoy),
                MensajesHoy = mensajes.Count(m => m.FechaCreacion.Date == hoy)
            };

            return View(estadisticas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas del foro");
            return View(new EstadisticasForoViewModel());
        }
    }
}
