using Microsoft.AspNetCore.Mvc;
using foro.Domain.Interfaces;
using foro.Domain.Entities;

namespace foro.Web.ViewComponents;

/// <summary>
/// ViewComponent para mostrar temas recientes
/// </summary>
public class RecentTopicsViewComponent : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;

    public RecentTopicsViewComponent(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<IViewComponentResult> InvokeAsync(int count = 5)
    {
        try
        {
     // Obtener temas más recientes
         var temas = await _unitOfWork.Temas.GetAllAsync();
       
       var temasRecientes = temas
     .OrderByDescending(t => t.FechaCreacion)
     .Take(count)
      .ToList();

// Cargar relaciones necesarias
       foreach (var tema in temasRecientes)
    {
      tema.Categoria = await _unitOfWork.Categorias.GetByIdAsync(tema.CategoriaId);
    tema.Usuario = await _unitOfWork.Usuarios.GetByIdAsync(tema.UsuarioId);
                
    // Contar mensajes
    var mensajes = await _unitOfWork.Mensajes.FindAsync(m => m.TemaId == tema.Id);
      tema.Mensajes = mensajes.ToList();
      }

            return View(temasRecientes);
        }
        catch
    {
       return View(new List<Tema>());
      }
    }
}
