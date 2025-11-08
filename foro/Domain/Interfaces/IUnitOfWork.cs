using foro.Domain.Entities;

namespace foro.Domain.Interfaces;

/// <summary>
/// Interfaz para el patrón Unit of Work que coordina las operaciones de múltiples repositorios
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Repositorio para la entidad Usuario
/// </summary>
    IGenericRepository<Usuario> Usuarios { get; }

    /// <summary>
    /// Repositorio para la entidad Categoria
    /// </summary>
    IGenericRepository<Categoria> Categorias { get; }

    /// <summary>
    /// Repositorio para la entidad Tema
    /// </summary>
    IGenericRepository<Tema> Temas { get; }

    /// <summary>
    /// Repositorio para la entidad Mensaje
    /// </summary>
    IGenericRepository<Mensaje> Mensajes { get; }

    /// <summary>
    /// Guarda todos los cambios realizados en el contexto de forma asíncrona
 /// </summary>
    /// <returns>Número de entidades afectadas</returns>
    Task<int> CommitAsync();

    /// <summary>
    /// Inicia una transacción de base de datos
    /// </summary>
    Task BeginTransactionAsync();

  /// <summary>
    /// Confirma la transacción actual
    /// </summary>
    Task CommitTransactionAsync();

    /// <summary>
  /// Revierte la transacción actual
    /// </summary>
    Task RollbackTransactionAsync();
}
