using foro.Domain.Entities;
using foro.Domain.Interfaces;

namespace foro.Application.Services;

/// <summary>
/// Servicio de aplicación para gestionar usuarios del foro
/// </summary>
public class UsuarioService
{
    private readonly IUnitOfWork _unitOfWork;

    public UsuarioService(IUnitOfWork unitOfWork)
    {
     _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Obtiene todos los usuarios activos
    /// </summary>
    public async Task<IEnumerable<Usuario>> ObtenerUsuariosActivosAsync()
    {
        return await _unitOfWork.Usuarios.FindAsync(u => u.Activo);
    }

    /// <summary>
    /// Obtiene un usuario por su ID
    /// </summary>
    public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int id)
 {
        return await _unitOfWork.Usuarios.GetByIdAsync(id);
    }

    /// <summary>
    /// Verifica si un email ya está registrado
    /// </summary>
    public async Task<bool> EmailExisteAsync(string email)
    {
        return await _unitOfWork.Usuarios.AnyAsync(u => u.Email == email);
    }

    /// <summary>
    /// Crea un nuevo usuario
    /// </summary>
    public async Task<Usuario> CrearUsuarioAsync(Usuario usuario)
    {
      // Validar que el email no exista
        if (await EmailExisteAsync(usuario.Email))
        {
         throw new InvalidOperationException($"El email {usuario.Email} ya está registrado");
        }

  // Agregar el usuario
    await _unitOfWork.Usuarios.AddAsync(usuario);
        
        // Guardar cambios
        await _unitOfWork.CommitAsync();

        return usuario;
    }

    /// <summary>
    /// Actualiza un usuario existente
    /// </summary>
    public async Task ActualizarUsuarioAsync(Usuario usuario)
    {
        var usuarioExistente = await _unitOfWork.Usuarios.GetByIdAsync(usuario.Id);
        if (usuarioExistente == null)
        {
        throw new InvalidOperationException($"Usuario con ID {usuario.Id} no encontrado");
        }

        // Verificar si el email cambió y si ya existe
        if (usuarioExistente.Email != usuario.Email)
 {
 if (await _unitOfWork.Usuarios.AnyAsync(u => u.Email == usuario.Email && u.Id != usuario.Id))
            {
  throw new InvalidOperationException($"El email {usuario.Email} ya está registrado");
            }
        }

        _unitOfWork.Usuarios.Update(usuario);
        await _unitOfWork.CommitAsync();
    }

    /// <summary>
    /// Desactiva un usuario (eliminación lógica)
    /// </summary>
    public async Task DesactivarUsuarioAsync(int id)
    {
        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
        if (usuario == null)
 {
    throw new InvalidOperationException($"Usuario con ID {id} no encontrado");
        }

        usuario.Activo = false;
        _unitOfWork.Usuarios.Update(usuario);
        await _unitOfWork.CommitAsync();
    }

    /// <summary>
    /// Elimina un usuario de forma permanente
    /// </summary>
    public async Task EliminarUsuarioAsync(int id)
    {
        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
        if (usuario == null)
{
     throw new InvalidOperationException($"Usuario con ID {id} no encontrado");
        }

      _unitOfWork.Usuarios.Delete(usuario);
        await _unitOfWork.CommitAsync();
    }

    /// <summary>
 /// Obtiene el número total de usuarios activos
    /// </summary>
    public async Task<int> ContarUsuariosActivosAsync()
    {
      return await _unitOfWork.Usuarios.CountAsync(u => u.Activo);
    }
}
