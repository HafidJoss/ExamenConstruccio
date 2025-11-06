using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SalesSuite.Domain.Interfaces;
using SalesSuite.Web.DTOs;

namespace SalesSuite.Web.ViewComponents;

/// <summary>
/// ViewComponent para mostrar los temas más recientes del foro
/// </summary>
public class TemasRecientesViewComponent : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<TemasRecientesViewComponent> _logger;

    public TemasRecientesViewComponent(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<TemasRecientesViewComponent> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Invoca el ViewComponent para obtener los temas recientes
    /// </summary>
    /// <param name="cantidad">Número de temas a mostrar (por defecto 5)</param>
    public async Task<IViewComponentResult> InvokeAsync(int cantidad = 5)
    {
        try
        {
            var temas = await _unitOfWork.Temas.GetAsync(
                filter: null,
                orderBy: q => q.OrderByDescending(t => t.FechaCreacion),
                includeProperties: "Categoria,Usuario"
            );

            var temasRecientes = temas.Take(cantidad);
            var temasDto = _mapper.Map<IEnumerable<TemaDto>>(temasRecientes);

            return View(temasDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener temas recientes");
            return View(Enumerable.Empty<TemaDto>());
        }
    }
}
