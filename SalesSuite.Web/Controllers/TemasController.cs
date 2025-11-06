using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesSuite.Application.UseCases.Temas;
using SalesSuite.Domain.Entities;
using SalesSuite.Domain.Interfaces;
using SalesSuite.Web.DTOs;
using X.PagedList;

namespace SalesSuite.Web.Controllers;

/// <summary>
/// Controlador para gestionar las operaciones de temas del foro
/// </summary>
[Authorize]
public class TemasController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<TemasController> _logger;
    private readonly ICrearTemaHandler _crearTemaHandler;

    public TemasController(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<TemasController> logger,
        ICrearTemaHandler crearTemaHandler)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _crearTemaHandler = crearTemaHandler ?? throw new ArgumentNullException(nameof(crearTemaHandler));
    }

    /// <summary>
    /// Muestra el listado de temas con búsqueda y paginación
    /// </summary>
    /// <param name="searchString">Término de búsqueda</param>
    /// <param name="categoriaId">ID de categoría para filtrar</param>
    /// <param name="page">Número de página</param>
    /// <returns>Vista con el listado de temas</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index(string? searchString, int? categoriaId, int? page)
    {
        try
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            // Obtener todos los temas con sus relaciones
            var temas = await _unitOfWork.Temas.GetAsync(
                filter: null,
                orderBy: q => q.OrderByDescending(t => t.Fijado)
                              .ThenByDescending(t => t.FechaUltimaActividad),
                includeProperties: "Categoria,Usuario,Mensajes"
            );

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                temas = temas.Where(t => 
                    t.Titulo.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    t.Contenido.Contains(searchString, StringComparison.OrdinalIgnoreCase));
                ViewBag.SearchString = searchString;
            }

            if (categoriaId.HasValue && categoriaId.Value > 0)
            {
                temas = temas.Where(t => t.CategoriaId == categoriaId.Value);
                ViewBag.CategoriaId = categoriaId.Value;
            }

            // Mapear a DTOs
            var temasDto = _mapper.Map<IEnumerable<TemaDto>>(temas);

            // Aplicar paginación
            var temasPaginados = temasDto.ToPagedList(pageNumber, pageSize);

            // Obtener categorías para el filtro
            var categorias = await _unitOfWork.Categorias.GetAsync(
                filter: c => c.Activa,
                orderBy: q => q.OrderBy(c => c.Orden)
            );
            ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre", categoriaId);

            return View(temasPaginados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el listado de temas");
            TempData["Error"] = "Error al cargar los temas. Por favor, intente nuevamente.";
            return View(new List<TemaDto>().ToPagedList(1, 10));
        }
    }

    /// <summary>
    /// Muestra los detalles de un tema específico
    /// </summary>
    /// <param name="id">ID del tema</param>
    /// <returns>Vista con los detalles del tema</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var tema = await _unitOfWork.Temas.GetFirstOrDefaultAsync(
                filter: t => t.Id == id,
                includeProperties: "Categoria,Usuario,Mensajes"
            );

            if (tema == null)
            {
                _logger.LogWarning("Tema con ID {TemaId} no encontrado", id);
                TempData["Error"] = "El tema solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }

            // Incrementar número de vistas
            tema.NumeroVistas++;
            _unitOfWork.Temas.Update(tema);
            await _unitOfWork.CommitAsync();

            var temaDto = _mapper.Map<TemaDto>(tema);
            return View(temaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tema {TemaId}", id);
            TempData["Error"] = "Error al cargar el tema. Por favor, intente nuevamente.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Muestra el formulario para crear un nuevo tema con su primer mensaje
    /// </summary>
    /// <returns>Vista con el formulario de creación</returns>
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        try
        {
            await CargarCategoriasViewBag();
            return View(new CrearTemaDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar el formulario de creación");
            TempData["Error"] = "Error al cargar el formulario. Por favor, intente nuevamente.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Procesa la creación de un nuevo tema con su primer mensaje usando transacción
    /// </summary>
    /// <param name="crearTemaDto">DTO con los datos del tema y mensaje inicial</param>
    /// <returns>Redirección al tema creado o vista con errores</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CrearTemaDto crearTemaDto)
    {
        if (!ModelState.IsValid)
        {
            await CargarCategoriasViewBag(crearTemaDto.CategoriaId);
            return View(crearTemaDto);
        }

        try
        {
            // TODO: Obtener el ID del usuario autenticado desde el sistema de autenticación
            // Por ahora, usar un usuario de ejemplo (ID = 1)
            int usuarioId = 1;

            // Crear el comando para el caso de uso
            var command = new CrearTemaCommand
            {
                Titulo = crearTemaDto.Titulo,
                ContenidoMensaje = crearTemaDto.ContenidoMensaje,
                CategoriaId = crearTemaDto.CategoriaId,
                UsuarioId = usuarioId,
                Fijado = crearTemaDto.Fijado
            };

            // Ejecutar el caso de uso
            var result = await _crearTemaHandler.HandleAsync(command);

            if (!result.Success)
            {
                // Agregar errores de validación al ModelState
                if (result.ValidationErrors.Any())
                {
                    foreach (var error in result.ValidationErrors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
                else if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                }

                await CargarCategoriasViewBag(crearTemaDto.CategoriaId);
                return View(crearTemaDto);
            }

            _logger.LogInformation(
                "Tema creado exitosamente: TemaId={TemaId}, MensajeId={MensajeId}, Usuario={UsuarioId}",
                result.TemaId, result.MensajeId, usuarioId);

            TempData["Success"] = "Tema y primer mensaje creados exitosamente.";
            return RedirectToAction(nameof(Details), new { id = result.TemaId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al crear el tema");
            ModelState.AddModelError(string.Empty, "Error inesperado al crear el tema. Por favor, intente nuevamente.");
            await CargarCategoriasViewBag(crearTemaDto.CategoriaId);
            return View(crearTemaDto);
        }
    }

    /// <summary>
    /// Muestra el formulario para editar un tema existente
    /// </summary>
    /// <param name="id">ID del tema a editar</param>
    /// <returns>Vista con el formulario de edición</returns>
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var tema = await _unitOfWork.Temas.GetByIdAsync(id);
            if (tema == null)
            {
                _logger.LogWarning("Tema con ID {TemaId} no encontrado para editar", id);
                TempData["Error"] = "El tema solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }

            var temaEditDto = _mapper.Map<TemaEditDto>(tema);
            await CargarCategoriasViewBag(tema.CategoriaId);
            return View(temaEditDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar el tema {TemaId} para editar", id);
            TempData["Error"] = "Error al cargar el tema. Por favor, intente nuevamente.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Procesa la edición de un tema existente
    /// </summary>
    /// <param name="id">ID del tema</param>
    /// <param name="temaEditDto">DTO con los datos actualizados</param>
    /// <returns>Redirección al índice o vista con errores</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TemaEditDto temaEditDto)
    {
        if (id != temaEditDto.Id)
        {
            _logger.LogWarning("ID de tema no coincide: URL={UrlId}, DTO={DtoId}", id, temaEditDto.Id);
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await CargarCategoriasViewBag(temaEditDto.CategoriaId);
            return View(temaEditDto);
        }

        try
        {
            // Verificar que el tema existe
            var temaExistente = await _unitOfWork.Temas.GetByIdAsync(id);
            if (temaExistente == null)
            {
                _logger.LogWarning("Tema con ID {TemaId} no encontrado para actualizar", id);
                TempData["Error"] = "El tema solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }

            // Verificar que la categoría existe
            var categoriaExiste = await _unitOfWork.Categorias.ExistsAsync(c => c.Id == temaEditDto.CategoriaId);
            if (!categoriaExiste)
            {
                ModelState.AddModelError("CategoriaId", "La categoría seleccionada no existe");
                await CargarCategoriasViewBag(temaEditDto.CategoriaId);
                return View(temaEditDto);
            }

            // Mapear cambios del DTO a la entidad existente
            _mapper.Map(temaEditDto, temaExistente);

            // Actualizar el tema
            _unitOfWork.Temas.Update(temaExistente);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Tema {TemaId} actualizado exitosamente", id);
            TempData["Success"] = "Tema actualizado exitosamente.";
            return RedirectToAction(nameof(Details), new { id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tema {TemaId}", id);
            ModelState.AddModelError("", "Error al actualizar el tema. Por favor, intente nuevamente.");
            await CargarCategoriasViewBag(temaEditDto.CategoriaId);
            return View(temaEditDto);
        }
    }

    /// <summary>
    /// Muestra la confirmación para eliminar un tema
    /// </summary>
    /// <param name="id">ID del tema a eliminar</param>
    /// <returns>Vista de confirmación</returns>
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var tema = await _unitOfWork.Temas.GetFirstOrDefaultAsync(
                filter: t => t.Id == id,
                includeProperties: "Categoria,Usuario"
            );

            if (tema == null)
            {
                _logger.LogWarning("Tema con ID {TemaId} no encontrado para eliminar", id);
                TempData["Error"] = "El tema solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }

            var temaDto = _mapper.Map<TemaDto>(tema);
            return View(temaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar el tema {TemaId} para eliminar", id);
            TempData["Error"] = "Error al cargar el tema. Por favor, intente nuevamente.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Procesa la eliminación de un tema
    /// </summary>
    /// <param name="id">ID del tema a eliminar</param>
    /// <returns>Redirección al índice</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var tema = await _unitOfWork.Temas.GetByIdAsync(id);
            if (tema == null)
            {
                _logger.LogWarning("Tema con ID {TemaId} no encontrado para eliminar", id);
                TempData["Error"] = "El tema solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.Temas.Delete(tema);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Tema {TemaId} eliminado exitosamente", id);
            TempData["Success"] = "Tema eliminado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tema {TemaId}", id);
            TempData["Error"] = "Error al eliminar el tema. Por favor, intente nuevamente.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Carga las categorías en el ViewBag para los dropdowns
    /// </summary>
    /// <param name="categoriaSeleccionada">ID de la categoría seleccionada</param>
    private async Task CargarCategoriasViewBag(int? categoriaSeleccionada = null)
    {
        var categorias = await _unitOfWork.Categorias.GetAsync(
            filter: c => c.Activa,
            orderBy: q => q.OrderBy(c => c.Orden)
        );

        ViewBag.CategoriaId = new SelectList(categorias, "Id", "Nombre", categoriaSeleccionada);
    }
}
