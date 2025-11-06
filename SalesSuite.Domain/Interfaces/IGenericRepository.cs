namespace SalesSuite.Domain.Interfaces;

/// <summary>
/// Interfaz genérica para operaciones de repositorio
/// </summary>
/// <typeparam name="T">Tipo de entidad</typeparam>
public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Obtiene todas las entidades de forma asíncrona
    /// </summary>
    /// <returns>Colección de entidades</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Obtiene una entidad por su identificador de forma asíncrona
    /// </summary>
    /// <param name="id">Identificador de la entidad</param>
    /// <returns>Entidad encontrada o null</returns>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Agrega una nueva entidad de forma asíncrona
    /// </summary>
    /// <param name="entity">Entidad a agregar</param>
    /// <returns>Tarea asíncrona</returns>
    Task AddAsync(T entity);

    /// <summary>
    /// Actualiza una entidad existente
    /// </summary>
    /// <param name="entity">Entidad a actualizar</param>
    void Update(T entity);

    /// <summary>
    /// Elimina una entidad
    /// </summary>
    /// <param name="entity">Entidad a eliminar</param>
    void Delete(T entity);

    /// <summary>
    /// Obtiene entidades con filtro, ordenamiento y paginación
    /// </summary>
    /// <param name="filter">Expresión de filtro</param>
    /// <param name="orderBy">Función de ordenamiento</param>
    /// <param name="includeProperties">Propiedades de navegación a incluir</param>
    /// <returns>Colección de entidades filtradas</returns>
    Task<IEnumerable<T>> GetAsync(
        System.Linq.Expressions.Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeProperties = "");

    /// <summary>
    /// Obtiene una entidad con filtro
    /// </summary>
    /// <param name="filter">Expresión de filtro</param>
    /// <param name="includeProperties">Propiedades de navegación a incluir</param>
    /// <returns>Primera entidad que cumple el filtro o null</returns>
    Task<T?> GetFirstOrDefaultAsync(
        System.Linq.Expressions.Expression<Func<T, bool>>? filter = null,
        string includeProperties = "");

    /// <summary>
    /// Cuenta el número de entidades que cumplen un filtro
    /// </summary>
    /// <param name="filter">Expresión de filtro</param>
    /// <returns>Número de entidades</returns>
    Task<int> CountAsync(System.Linq.Expressions.Expression<Func<T, bool>>? filter = null);

    /// <summary>
    /// Verifica si existe alguna entidad que cumpla el filtro
    /// </summary>
    /// <param name="filter">Expresión de filtro</param>
    /// <returns>True si existe, false en caso contrario</returns>
    Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<T, bool>> filter);
}
