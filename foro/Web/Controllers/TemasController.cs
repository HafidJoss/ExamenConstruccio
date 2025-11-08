using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using X.PagedList;
using X.PagedList.Extensions;
using foro.Domain.Entities;
using foro.Domain.Interfaces;
using foro.Web.DTOs;
using foro.Application.Interfaces;
using foro.Application.UseCases.Temas;
using System.Security.Claims;

namespace foro.Web.Controllers;

/// <summary>
/// Controlador para gestionar las operaciones principales del foro de temas
/// </summary>
[Authorize] // ? REQUERIR AUTENTICACIÓN PARA TODO EL CONTROLADOR
public class TemasController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<TemasController> _logger;
    private readonly ICrearTemaUseCase _crearTemaUseCase;
    private readonly UserManager<ApplicationUser> _userManager;

    public TemasController(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
    ILogger<TemasController> logger,
        ICrearTemaUseCase crearTemaUseCase,
        UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
     _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _crearTemaUseCase = crearTemaUseCase ?? throw new ArgumentNullException(nameof(crearTemaUseCase));
     _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    // GET: Temas
    /// <summary>
    /// Muestra el listado de temas con búsqueda y paginación
    /// </summary>
    [AllowAnonymous] // ? PERMITIR ACCESO PÚBLICO A LISTADO
    public async Task<IActionResult> Index(string searchString, int? categoriaId, int? page)
    {
 try
        {
       var pageNumber = page ?? 1;
        var pageSize = 10;

    // Obtener todos los temas con sus relaciones
   var temas = await _unitOfWork.Temas.GetAllAsync();
        
        // Cargar las relaciones manualmente
        var temasConRelaciones = new List<Tema>();
  foreach (var tema in temas)
 {
          var categoria = await _unitOfWork.Categorias.GetByIdAsync(tema.CategoriaId);
          var usuario = await _unitOfWork.Usuarios.GetByIdAsync(tema.UsuarioId);
     var mensajes = await _unitOfWork.Mensajes.FindAsync(m => m.TemaId == tema.Id);
        
     tema.Categoria = categoria!;
       tema.Usuario = usuario!;
          tema.Mensajes = mensajes.ToList();
                
      temasConRelaciones.Add(tema);
    }

            // Filtrar por búsqueda
if (!string.IsNullOrWhiteSpace(searchString))
  {
    temasConRelaciones = temasConRelaciones
     .Where(t => t.Titulo.Contains(searchString, StringComparison.OrdinalIgnoreCase))
        .ToList();
          }

            // Filtrar por categoría
   if (categoriaId.HasValue && categoriaId.Value > 0)
          {
    temasConRelaciones = temasConRelaciones
      .Where(t => t.CategoriaId == categoriaId.Value)
   .ToList();
     }

            // Ordenar por fijados primero, luego por fecha de última actividad
            var temasOrdenados = temasConRelaciones
          .OrderByDescending(t => t.Fijado)
       .ThenByDescending(t => t.FechaUltimaActividad ?? t.FechaCreacion)
       .ToList();

            // Mapear a DTOs
            var temasDto = _mapper.Map<List<TemaListDto>>(temasOrdenados);

  // Aplicar paginación
         var temasPaginados = temasDto.ToPagedList(pageNumber, pageSize);

    // Cargar categorías para el filtro
            var categorias = await _unitOfWork.Categorias.FindAsync(c => c.Activa);
            ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre", categoriaId);
            ViewBag.SearchString = searchString;
            ViewBag.CategoriaId = categoriaId;

      return View(temasPaginados);
        }
      catch (Exception ex)
        {
  _logger.LogError(ex, "Error al cargar el listado de temas");
            TempData["Error"] = "Error al cargar los temas. Por favor, intente nuevamente.";
            return View(new List<TemaListDto>().ToPagedList(1, 10));
  }
    }

    // GET: Temas/Details/5
    /// <summary>
    /// Muestra los detalles de un tema
    /// </summary>
    [AllowAnonymous] // ? PERMITIR VER DETALLES SIN AUTENTICACIÓN
  public async Task<IActionResult> Details(int id)
    {
 try
        {
          var tema = await _unitOfWork.Temas.GetByIdAsync(id);
     if (tema == null)
   {
       TempData["Error"] = "Tema no encontrado.";
    return RedirectToAction(nameof(Index));
            }

            // Cargar relaciones
    tema.Categoria = await _unitOfWork.Categorias.GetByIdAsync(tema.CategoriaId);
tema.Usuario = await _unitOfWork.Usuarios.GetByIdAsync(tema.UsuarioId);
    var mensajes = await _unitOfWork.Mensajes.FindAsync(m => m.TemaId == tema.Id);
 tema.Mensajes = mensajes.ToList();

            // Incrementar vistas
      tema.Vistas++;
 _unitOfWork.Temas.Update(tema);
          await _unitOfWork.CommitAsync();

    var temaDto = _mapper.Map<TemaDeleteDto>(tema);
         return View(temaDto);
    }
        catch (Exception ex)
        {
   _logger.LogError(ex, "Error al cargar los detalles del tema {TemaId}", id);
 TempData["Error"] = "Error al cargar el tema.";
            return RedirectToAction(nameof(Index));
      }
 }

    // GET: Temas/Create
    /// <summary>
  /// Muestra el formulario para crear un nuevo tema
 /// </summary>
    public async Task<IActionResult> Create()
    {
        try
        {
        await CargarCategoriasAsync();
        return View();
        }
        catch (Exception ex)
    {
 _logger.LogError(ex, "Error al cargar el formulario de creación");
    TempData["Error"] = "Error al cargar el formulario.";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Temas/Create
    /// <summary>
    /// Procesa la creación de un nuevo tema con su mensaje inicial usando transacciones
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TemaCreateDto temaDto)
    {
     if (!ModelState.IsValid)
        {
         await CargarCategoriasAsync(temaDto.CategoriaId);
  return View(temaDto);
        }

        try
        {
            // TODO: Obtener el usuario actual de la sesión/autenticación
       // Por ahora, usar un usuario de ejemplo o desde HttpContext
        var usuarioId = ObtenerUsuarioActualId(); // Método helper que simula obtener usuario autenticado

      // Crear request para el caso de uso
      var request = new CrearTemaRequest
    {
      Titulo = temaDto.Titulo,
          Contenido = temaDto.Contenido,
    CategoriaId = temaDto.CategoriaId,
       UsuarioId = usuarioId
         };

   // Ejecutar caso de uso
var response = await _crearTemaUseCase.ExecuteAsync(request);

        if (!response.Exito)
      {
    // Agregar errores al ModelState
       foreach (var error in response.Errores)
     {
 ModelState.AddModelError("", error);
            }
     await CargarCategoriasAsync(temaDto.CategoriaId);
         return View(temaDto);
         }

      _logger.LogInformation("Tema {TemaId} creado exitosamente por usuario {UsuarioId}", response.TemaId, usuarioId);
            TempData["Success"] = response.Mensaje;
            return RedirectToAction(nameof(Details), new { id = response.TemaId });
        }
        catch (Exception ex)
      {
            _logger.LogError(ex, "Error al crear el tema");
     ModelState.AddModelError("", "Error inesperado al crear el tema. Por favor, intente nuevamente.");
         await CargarCategoriasAsync(temaDto.CategoriaId);
            return View(temaDto);
      }
    }

    // GET: Temas/Edit/5
    /// <summary>
    /// Muestra el formulario para editar un tema
    /// </summary>
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
       var tema = await _unitOfWork.Temas.GetByIdAsync(id);
            if (tema == null)
  {
     TempData["Error"] = "Tema no encontrado.";
           return RedirectToAction(nameof(Index));
  }

       var temaDto = _mapper.Map<TemaEditDto>(tema);
    await CargarCategoriasAsync(temaDto.CategoriaId);
   return View(temaDto);
}
        catch (Exception ex)
        {
  _logger.LogError(ex, "Error al cargar el formulario de edición del tema {TemaId}", id);
      TempData["Error"] = "Error al cargar el tema.";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Temas/Edit/5
    /// <summary>
    /// Procesa la edición de un tema
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TemaEditDto temaDto)
    {
        if (id != temaDto.Id)
     {
     return BadRequest();
    }

        if (!ModelState.IsValid)
        {
await CargarCategoriasAsync(temaDto.CategoriaId);
            return View(temaDto);
        }

        try
        {
            var temaExistente = await _unitOfWork.Temas.GetByIdAsync(id);
          if (temaExistente == null)
     {
                TempData["Error"] = "Tema no encontrado.";
    return RedirectToAction(nameof(Index));
       }

            // Validar que la categoría existe y está activa
            var categoria = await _unitOfWork.Categorias.GetByIdAsync(temaDto.CategoriaId);
       if (categoria == null || !categoria.Activa)
        {
             ModelState.AddModelError("CategoriaId", "La categoría seleccionada no es válida.");
 await CargarCategoriasAsync(temaDto.CategoriaId);
         return View(temaDto);
         }

            // Actualizar solo los campos permitidos
          temaExistente.Titulo = temaDto.Titulo;
     temaExistente.CategoriaId = temaDto.CategoriaId;

            _unitOfWork.Temas.Update(temaExistente);
            await _unitOfWork.CommitAsync();

    _logger.LogInformation("Tema actualizado exitosamente: {TemaId}", id);
     TempData["Success"] = "Tema actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
  _logger.LogError(ex, "Error al actualizar el tema {TemaId}", id);
            ModelState.AddModelError("", "Error al actualizar el tema. Por favor, intente nuevamente.");
         await CargarCategoriasAsync(temaDto.CategoriaId);
            return View(temaDto);
}
    }

    // GET: Temas/Delete/5
    /// <summary>
    /// Muestra la confirmación para eliminar un tema
    /// </summary>
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
        var tema = await _unitOfWork.Temas.GetByIdAsync(id);
      if (tema == null)
            {
     TempData["Error"] = "Tema no encontrado.";
                return RedirectToAction(nameof(Index));
            }

   // Cargar relaciones
 tema.Categoria = await _unitOfWork.Categorias.GetByIdAsync(tema.CategoriaId);
        tema.Usuario = await _unitOfWork.Usuarios.GetByIdAsync(tema.UsuarioId);
 var mensajes = await _unitOfWork.Mensajes.FindAsync(m => m.TemaId == tema.Id);
   tema.Mensajes = mensajes.ToList();

            var temaDto = _mapper.Map<TemaDeleteDto>(tema);
        return View(temaDto);
        }
  catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la confirmación de eliminación del tema {TemaId}", id);
    TempData["Error"] = "Error al cargar el tema.";
       return RedirectToAction(nameof(Index));
        }
    }

    // POST: Temas/Delete/5
    /// <summary>
    /// Procesa la eliminación de un tema
  /// </summary>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
   {
          var tema = await _unitOfWork.Temas.GetByIdAsync(id);
     if (tema == null)
            {
     TempData["Error"] = "Tema no encontrado.";
          return RedirectToAction(nameof(Index));
        }

      _unitOfWork.Temas.Delete(tema);
            await _unitOfWork.CommitAsync();

        _logger.LogInformation("Tema eliminado exitosamente: {TemaId}", id);
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

  #region Métodos Auxiliares

    /// <summary>
    /// Carga las categorías activas en el ViewBag
    /// </summary>
    private async Task CargarCategoriasAsync(int? categoriaSeleccionada = null)
    {
     var categorias = await _unitOfWork.Categorias.FindAsync(c => c.Activa);
     var categoriasDto = _mapper.Map<List<CategoriaDto>>(categorias);
 ViewBag.Categorias = new SelectList(categoriasDto, "Id", "Nombre", categoriaSeleccionada);
    }

    /// <summary>
    /// Obtiene el ID del usuario actualmente autenticado desde Identity
    /// </summary>
 private int ObtenerUsuarioActualId()
    {
        // Obtener el ID del usuario autenticado desde los claims
 var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    
  if (string.IsNullOrEmpty(userId))
   {
     _logger.LogWarning("No se pudo obtener el ID del usuario autenticado");
      return 1; // Fallback al usuario admin (solo para desarrollo)
        }

        // En este caso, el ID del usuario de Identity es un string (GUID)
// Necesitamos mapear a la entidad Usuario antigua o usar ApplicationUser
        // Por ahora, retornamos un ID fijo para compatibilidad
     // TODO: Migrar completamente a ApplicationUser en el dominio
     
        return 1; // Temporal hasta migrar completamente
    }

    /// <summary>
    /// Obtiene el usuario actual autenticado de Identity
    /// </summary>
    private async Task<ApplicationUser?> ObtenerUsuarioActualAsync()
    {
        return await _userManager.GetUserAsync(User);
    }

    #endregion
}
