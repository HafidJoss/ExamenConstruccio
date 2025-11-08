using System.Linq.Expressions;

namespace foro.Domain.Interfaces;

/// <summary>
/// Interfaz genérica para operaciones CRUD asíncronas
/// </summary>
/// <typeparam name="T">Tipo de entidad</typeparam>
public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Obtiene todas las entidades de forma asíncrona
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Obtiene una entidad por su ID de forma asíncrona
    /// </summary>
 Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Obtiene entidades que cumplan un criterio específico
    /// </summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Agrega una nueva entidad de forma asíncrona
    /// </summary>
    Task AddAsync(T entity);

    /// <summary>
    /// Actualiza una entidad existente
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Elimina una entidad
    /// </summary>
    void Delete(T entity);

    /// <summary>
    /// Verifica si existe alguna entidad que cumpla el criterio
  /// </summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

 /// <summary>
    /// Cuenta el número de entidades que cumplen el criterio
    /// </summary>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}
