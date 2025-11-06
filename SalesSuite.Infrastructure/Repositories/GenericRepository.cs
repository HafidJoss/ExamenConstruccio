using Microsoft.EntityFrameworkCore;
using SalesSuite.Domain.Interfaces;
using SalesSuite.Infrastructure.Data;
using System.Linq.Expressions;

namespace SalesSuite.Infrastructure.Repositories;

/// <summary>
/// Implementación genérica del repositorio usando Entity Framework Core
/// </summary>
/// <typeparam name="T">Tipo de entidad</typeparam>
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ForumDbContext _context;
    private readonly DbSet<T> _dbSet;

    /// <summary>
    /// Constructor del repositorio genérico
    /// </summary>
    /// <param name="context">Contexto de base de datos</param>
    public GenericRepository(ForumDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<T>();
    }

    /// <summary>
    /// Obtiene todas las entidades de forma asíncrona
    /// </summary>
    /// <returns>Colección de entidades</returns>
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <summary>
    /// Obtiene una entidad por su identificador de forma asíncrona
    /// </summary>
    /// <param name="id">Identificador de la entidad</param>
    /// <returns>Entidad encontrada o null</returns>
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <summary>
    /// Agrega una nueva entidad de forma asíncrona
    /// </summary>
    /// <param name="entity">Entidad a agregar</param>
    /// <returns>Tarea asíncrona</returns>
    public virtual async Task AddAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await _dbSet.AddAsync(entity);
    }

    /// <summary>
    /// Actualiza una entidad existente
    /// </summary>
    /// <param name="entity">Entidad a actualizar</param>
    public virtual void Update(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    /// <summary>
    /// Elimina una entidad
    /// </summary>
    /// <param name="entity">Entidad a eliminar</param>
    public virtual void Delete(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }

        _dbSet.Remove(entity);
    }

    /// <summary>
    /// Obtiene entidades con filtro, ordenamiento y propiedades de navegación
    /// </summary>
    /// <param name="filter">Expresión de filtro</param>
    /// <param name="orderBy">Función de ordenamiento</param>
    /// <param name="includeProperties">Propiedades de navegación a incluir (separadas por coma)</param>
    /// <returns>Colección de entidades filtradas</returns>
    public virtual async Task<IEnumerable<T>> GetAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeProperties = "")
    {
        IQueryable<T> query = _dbSet;

        // Aplicar filtro
        if (filter != null)
        {
            query = query.Where(filter);
        }

        // Incluir propiedades de navegación
        foreach (var includeProperty in includeProperties.Split(
            new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty.Trim());
        }

        // Aplicar ordenamiento
        if (orderBy != null)
        {
            return await orderBy(query).ToListAsync();
        }

        return await query.ToListAsync();
    }

    /// <summary>
    /// Obtiene la primera entidad que cumple el filtro o null
    /// </summary>
    /// <param name="filter">Expresión de filtro</param>
    /// <param name="includeProperties">Propiedades de navegación a incluir (separadas por coma)</param>
    /// <returns>Primera entidad que cumple el filtro o null</returns>
    public virtual async Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>>? filter = null,
        string includeProperties = "")
    {
        IQueryable<T> query = _dbSet;

        // Incluir propiedades de navegación
        foreach (var includeProperty in includeProperties.Split(
            new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty.Trim());
        }

        // Aplicar filtro
        if (filter != null)
        {
            return await query.FirstOrDefaultAsync(filter);
        }

        return await query.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Cuenta el número de entidades que cumplen un filtro
    /// </summary>
    /// <param name="filter">Expresión de filtro</param>
    /// <returns>Número de entidades</returns>
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = _dbSet;

        if (filter != null)
        {
            return await query.CountAsync(filter);
        }

        return await query.CountAsync();
    }

    /// <summary>
    /// Verifica si existe alguna entidad que cumpla el filtro
    /// </summary>
    /// <param name="filter">Expresión de filtro</param>
    /// <returns>True si existe, false en caso contrario</returns>
    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter)
    {
        if (filter == null)
            throw new ArgumentNullException(nameof(filter));

        return await _dbSet.AnyAsync(filter);
    }
}
