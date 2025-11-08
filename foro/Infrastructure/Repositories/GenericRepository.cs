using Microsoft.EntityFrameworkCore;
using foro.Domain.Interfaces;
using foro.Infrastructure.Data;
using System.Linq.Expressions;

namespace foro.Infrastructure.Repositories;

/// <summary>
/// Implementación genérica del patrón Repository usando Entity Framework Core
/// </summary>
/// <typeparam name="T">Tipo de entidad</typeparam>
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ForumDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(ForumDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
     _dbSet = _context.Set<T>();
  }

    /// <summary>
    /// Obtiene todas las entidades de forma asíncrona
    /// </summary>
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
      return await _dbSet.ToListAsync();
    }

    /// <summary>
    /// Obtiene una entidad por su ID de forma asíncrona
    /// </summary>
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
 }

    /// <summary>
    /// Obtiene entidades que cumplan un criterio específico
    /// </summary>
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    /// <summary>
    /// Agrega una nueva entidad de forma asíncrona
    /// </summary>
    public virtual async Task AddAsync(T entity)
    {
        if (entity == null)
   throw new ArgumentNullException(nameof(entity));

        await _dbSet.AddAsync(entity);
    }

    /// <summary>
    /// Actualiza una entidad existente
    /// </summary>
    public virtual void Update(T entity)
    {
      if (entity == null)
   throw new ArgumentNullException(nameof(entity));

  _dbSet.Update(entity);
}

    /// <summary>
    /// Elimina una entidad
    /// </summary>
    public virtual void Delete(T entity)
    {
        if (entity == null)
 throw new ArgumentNullException(nameof(entity));

        _dbSet.Remove(entity);
    }

    /// <summary>
    /// Verifica si existe alguna entidad que cumpla el criterio
    /// </summary>
    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    /// <summary>
    /// Cuenta el número de entidades que cumplen el criterio
    /// </summary>
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
  if (predicate == null)
return await _dbSet.CountAsync();

        return await _dbSet.CountAsync(predicate);
    }
}
