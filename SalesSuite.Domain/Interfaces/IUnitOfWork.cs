using SalesSuite.Domain.Entities;

namespace SalesSuite.Domain.Interfaces;

/// <summary>
/// Interfaz para el patrón Unit of Work
/// Coordina el trabajo de múltiples repositorios y gestiona las transacciones
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
    /// Guarda todos los cambios pendientes en la base de datos de forma asíncrona
    /// </summary>
    /// <returns>Número de registros afectados</returns>
    Task<int> CommitAsync();

    /// <summary>
    /// Inicia una transacción de base de datos
    /// </summary>
    /// <returns>Tarea asíncrona</returns>
    Task BeginTransactionAsync();

    /// <summary>
    /// Confirma la transacción actual
    /// </summary>
    /// <returns>Tarea asíncrona</returns>
    Task CommitTransactionAsync();

    /// <summary>
    /// Revierte la transacción actual
    /// </summary>
    /// <returns>Tarea asíncrona</returns>
    Task RollbackTransactionAsync();
}
