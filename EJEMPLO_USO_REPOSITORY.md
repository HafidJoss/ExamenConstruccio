# Guía de Uso del Patrón Repository y Unit of Work

## Configuración en Program.cs (ASP.NET Core 8)

```csharp
using SalesSuite.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios de infraestructura
builder.Services.AddInfrastructure(builder.Configuration);

// O usando cadena de conexión directa:
// builder.Services.AddInfrastructure("Server=.;Database=ForumDB;Trusted_Connection=True;");

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configurar el pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

## Configuración en appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ForumDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

## Ejemplo de Uso en un Controlador

```csharp
using Microsoft.AspNetCore.Mvc;
using SalesSuite.Domain.Entities;
using SalesSuite.Domain.Interfaces;

namespace SalesSuite.Web.Controllers;

public class UsuariosController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(IUnitOfWork unitOfWork, ILogger<UsuariosController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // GET: Usuarios
    public async Task<IActionResult> Index()
    {
        try
        {
            var usuarios = await _unitOfWork.Usuarios.GetAllAsync();
            return View(usuarios);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la lista de usuarios");
            return View("Error");
        }
    }

    // GET: Usuarios/Details/5
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var usuario = await _unitOfWork.Usuarios.GetFirstOrDefaultAsync(
                filter: u => u.Id == id,
                includeProperties: "TemasCreados,MensajesEscritos"
            );

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el usuario {Id}", id);
            return View("Error");
        }
    }

    // GET: Usuarios/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Usuarios/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Usuario usuario)
    {
        if (!ModelState.IsValid)
        {
            return View(usuario);
        }

        try
        {
            // Verificar si el email ya existe
            var existeEmail = await _unitOfWork.Usuarios.ExistsAsync(u => u.Email == usuario.Email);
            if (existeEmail)
            {
                ModelState.AddModelError("Email", "El email ya está registrado");
                return View(usuario);
            }

            await _unitOfWork.Usuarios.AddAsync(usuario);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Usuario {NombreUsuario} creado exitosamente", usuario.NombreUsuario);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el usuario");
            ModelState.AddModelError("", "Error al guardar el usuario");
            return View(usuario);
        }
    }

    // GET: Usuarios/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el usuario {Id}", id);
            return View("Error");
        }
    }

    // POST: Usuarios/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Usuario usuario)
    {
        if (id != usuario.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(usuario);
        }

        try
        {
            _unitOfWork.Usuarios.Update(usuario);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Usuario {Id} actualizado exitosamente", id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el usuario {Id}", id);
            ModelState.AddModelError("", "Error al actualizar el usuario");
            return View(usuario);
        }
    }

    // POST: Usuarios/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _unitOfWork.Usuarios.Delete(usuario);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Usuario {Id} eliminado exitosamente", id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el usuario {Id}", id);
            TempData["Error"] = "Error al eliminar el usuario";
            return RedirectToAction(nameof(Index));
        }
    }
}
```

## Ejemplo con Transacciones

```csharp
public class TemasController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TemasController> _logger;

    public TemasController(IUnitOfWork unitOfWork, ILogger<TemasController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateWithFirstMessage(Tema tema, string mensajeContenido)
    {
        if (!ModelState.IsValid)
        {
            return View(tema);
        }

        try
        {
            // Iniciar transacción
            await _unitOfWork.BeginTransactionAsync();

            // Crear el tema
            await _unitOfWork.Temas.AddAsync(tema);
            await _unitOfWork.CommitAsync();

            // Crear el primer mensaje del tema
            var primerMensaje = new Mensaje
            {
                TemaId = tema.Id,
                UsuarioId = tema.UsuarioId,
                Contenido = mensajeContenido,
                FechaCreacion = DateTime.UtcNow
            };

            await _unitOfWork.Mensajes.AddAsync(primerMensaje);
            await _unitOfWork.CommitAsync();

            // Confirmar transacción
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Tema {TemaId} creado con su primer mensaje", tema.Id);
            return RedirectToAction("Details", new { id = tema.Id });
        }
        catch (Exception ex)
        {
            // Revertir transacción en caso de error
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error al crear el tema con mensaje");
            ModelState.AddModelError("", "Error al crear el tema");
            return View(tema);
        }
    }
}
```

## Ejemplo de Consultas Avanzadas

```csharp
public class ForoService
{
    private readonly IUnitOfWork _unitOfWork;

    public ForoService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // Obtener temas activos de una categoría con paginación
    public async Task<IEnumerable<Tema>> GetTemasActivosPorCategoria(int categoriaId, int pagina, int tamanoPagina)
    {
        return await _unitOfWork.Temas.GetAsync(
            filter: t => t.CategoriaId == categoriaId && !t.Cerrado,
            orderBy: q => q.OrderByDescending(t => t.Fijado)
                          .ThenByDescending(t => t.FechaUltimaActividad),
            includeProperties: "Usuario,Categoria"
        );
    }

    // Obtener mensajes de un tema con información del usuario
    public async Task<IEnumerable<Mensaje>> GetMensajesPorTema(int temaId)
    {
        return await _unitOfWork.Mensajes.GetAsync(
            filter: m => m.TemaId == temaId && !m.Oculto,
            orderBy: q => q.OrderBy(m => m.FechaCreacion),
            includeProperties: "Usuario,MensajePadre"
        );
    }

    // Contar temas por usuario
    public async Task<int> ContarTemasPorUsuario(int usuarioId)
    {
        return await _unitOfWork.Temas.CountAsync(t => t.UsuarioId == usuarioId);
    }

    // Verificar si un usuario puede editar un mensaje
    public async Task<bool> PuedeEditarMensaje(int mensajeId, int usuarioId)
    {
        var mensaje = await _unitOfWork.Mensajes.GetFirstOrDefaultAsync(
            filter: m => m.Id == mensajeId && m.UsuarioId == usuarioId
        );

        return mensaje != null;
    }

    // Obtener categorías con conteo de temas
    public async Task<IEnumerable<Categoria>> GetCategoriasConConteo()
    {
        var categorias = await _unitOfWork.Categorias.GetAsync(
            filter: c => c.Activa,
            orderBy: q => q.OrderBy(c => c.Orden),
            includeProperties: "Temas"
        );

        return categorias;
    }
}
```

## Comandos de Migración

```bash
# Agregar una nueva migración
dotnet ef migrations add InitialCreate --project SalesSuite.Infrastructure --startup-project SalesSuite.Web

# Actualizar la base de datos
dotnet ef database update --project SalesSuite.Infrastructure --startup-project SalesSuite.Web

# Eliminar la última migración
dotnet ef migrations remove --project SalesSuite.Infrastructure --startup-project SalesSuite.Web

# Ver el script SQL de la migración
dotnet ef migrations script --project SalesSuite.Infrastructure --startup-project SalesSuite.Web
```

## Buenas Prácticas

1. **Siempre usar async/await** para operaciones de base de datos
2. **Manejar excepciones** apropiadamente en los controladores
3. **Usar transacciones** cuando se realizan múltiples operaciones relacionadas
4. **Validar datos** antes de guardar en la base de datos
5. **Usar logging** para rastrear operaciones y errores
6. **Incluir propiedades de navegación** solo cuando sean necesarias
7. **Implementar paginación** para consultas que retornan muchos registros
8. **Usar filtros** para optimizar las consultas
9. **Liberar recursos** correctamente (el UnitOfWork implementa IDisposable)
10. **Separar la lógica de negocio** en servicios dedicados
