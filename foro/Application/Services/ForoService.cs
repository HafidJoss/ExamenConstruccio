using foro.Domain.Entities;
using foro.Domain.Interfaces;

namespace foro.Application.Services;

/// <summary>
/// Servicio de aplicación para gestionar temas y mensajes del foro
/// </summary>
public class ForoService
{
    private readonly IUnitOfWork _unitOfWork;

    public ForoService(IUnitOfWork unitOfWork)
    {
 _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Crea un nuevo tema con un mensaje inicial usando transacción
    /// </summary>
    public async Task<Tema> CrearTemaConMensajeInicialAsync(int usuarioId, int categoriaId, string titulo, string contenido)
    {
        // Validar que el usuario existe
        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(usuarioId);
        if (usuario == null || !usuario.Activo)
        {
          throw new InvalidOperationException("Usuario no encontrado o inactivo");
   }

      // Validar que la categoría existe
    var categoria = await _unitOfWork.Categorias.GetByIdAsync(categoriaId);
   if (categoria == null || !categoria.Activa)
    {
   throw new InvalidOperationException("Categoría no encontrada o inactiva");
   }

     Tema? nuevoTema = null;

        try
        {
   // Iniciar transacción
            await _unitOfWork.BeginTransactionAsync();

            // Crear el tema
  nuevoTema = new Tema
   {
     Titulo = titulo,
    Contenido = contenido,
                UsuarioId = usuarioId,
                CategoriaId = categoriaId,
           FechaCreacion = DateTime.UtcNow,
          FechaUltimaActividad = DateTime.UtcNow
            };

            await _unitOfWork.Temas.AddAsync(nuevoTema);
   await _unitOfWork.CommitAsync(); // Guardar para obtener el ID del tema

            // Crear el mensaje inicial
         var mensajeInicial = new Mensaje
            {
   Contenido = contenido,
    TemaId = nuevoTema.Id,
     UsuarioId = usuarioId,
    FechaCreacion = DateTime.UtcNow
            };

            await _unitOfWork.Mensajes.AddAsync(mensajeInicial);
 await _unitOfWork.CommitAsync();

            // Confirmar transacción
      await _unitOfWork.CommitTransactionAsync();

 return nuevoTema;
        }
        catch (Exception)
        {
     // Revertir transacción en caso de error
     await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    /// <summary>
    /// Agrega un mensaje a un tema existente
    /// </summary>
    public async Task<Mensaje> AgregarMensajeAsync(int temaId, int usuarioId, string contenido)
    {
        // Validar que el tema existe y no está cerrado
        var tema = await _unitOfWork.Temas.GetByIdAsync(temaId);
        if (tema == null)
      {
    throw new InvalidOperationException("Tema no encontrado");
        }

        if (tema.Cerrado)
   {
            throw new InvalidOperationException("El tema está cerrado y no acepta nuevos mensajes");
        }

        // Validar que el usuario existe
        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(usuarioId);
  if (usuario == null || !usuario.Activo)
        {
      throw new InvalidOperationException("Usuario no encontrado o inactivo");
        }

        // Crear el mensaje
        var nuevoMensaje = new Mensaje
        {
            Contenido = contenido,
     TemaId = temaId,
   UsuarioId = usuarioId,
         FechaCreacion = DateTime.UtcNow
        };

    await _unitOfWork.Mensajes.AddAsync(nuevoMensaje);

        // Actualizar la fecha de última actividad del tema
        tema.FechaUltimaActividad = DateTime.UtcNow;
        _unitOfWork.Temas.Update(tema);

        // Guardar todos los cambios
        await _unitOfWork.CommitAsync();

        return nuevoMensaje;
    }

    /// <summary>
    /// Obtiene todos los temas de una categoría
    /// </summary>
    public async Task<IEnumerable<Tema>> ObtenerTemasPorCategoriaAsync(int categoriaId)
    {
     return await _unitOfWork.Temas.FindAsync(t => t.CategoriaId == categoriaId);
    }

    /// <summary>
    /// Obtiene todos los mensajes de un tema
    /// </summary>
    public async Task<IEnumerable<Mensaje>> ObtenerMensajesPorTemaAsync(int temaId)
    {
        return await _unitOfWork.Mensajes.FindAsync(m => m.TemaId == temaId);
    }

    /// <summary>
    /// Incrementa el contador de vistas de un tema
    /// </summary>
    public async Task IncrementarVistasAsync(int temaId)
    {
  var tema = await _unitOfWork.Temas.GetByIdAsync(temaId);
        if (tema != null)
   {
tema.Vistas++;
            _unitOfWork.Temas.Update(tema);
            await _unitOfWork.CommitAsync();
        }
    }

    /// <summary>
    /// Cierra un tema (solo el autor o un administrador pueden hacerlo)
 /// </summary>
    public async Task CerrarTemaAsync(int temaId, int usuarioId)
    {
        var tema = await _unitOfWork.Temas.GetByIdAsync(temaId);
        if (tema == null)
        {
   throw new InvalidOperationException("Tema no encontrado");
        }

        // Verificar que el usuario es el autor
        if (tema.UsuarioId != usuarioId)
  {
    throw new UnauthorizedAccessException("Solo el autor puede cerrar el tema");
        }

        tema.Cerrado = true;
        _unitOfWork.Temas.Update(tema);
    await _unitOfWork.CommitAsync();
    }

    /// <summary>
    /// Edita un mensaje existente
    /// </summary>
    public async Task EditarMensajeAsync(int mensajeId, int usuarioId, string nuevoContenido)
    {
        var mensaje = await _unitOfWork.Mensajes.GetByIdAsync(mensajeId);
   if (mensaje == null)
        {
            throw new InvalidOperationException("Mensaje no encontrado");
     }

        // Verificar que el usuario es el autor
   if (mensaje.UsuarioId != usuarioId)
        {
        throw new UnauthorizedAccessException("Solo el autor puede editar el mensaje");
        }

        mensaje.Contenido = nuevoContenido;
        mensaje.FechaEdicion = DateTime.UtcNow;
        mensaje.Editado = true;

        _unitOfWork.Mensajes.Update(mensaje);
  await _unitOfWork.CommitAsync();
    }

    /// <summary>
    /// Obtiene estadísticas de una categoría
    /// </summary>
    public async Task<(int TotalTemas, int TotalMensajes)> ObtenerEstadisticasCategoriaAsync(int categoriaId)
    {
        var totalTemas = await _unitOfWork.Temas.CountAsync(t => t.CategoriaId == categoriaId);
        
  // Obtener todos los IDs de temas de la categoría
        var temas = await _unitOfWork.Temas.FindAsync(t => t.CategoriaId == categoriaId);
        var temaIds = temas.Select(t => t.Id).ToList();
        
        var totalMensajes = 0;
        foreach (var temaId in temaIds)
        {
     totalMensajes += await _unitOfWork.Mensajes.CountAsync(m => m.TemaId == temaId);
        }

        return (totalTemas, totalMensajes);
    }
}
