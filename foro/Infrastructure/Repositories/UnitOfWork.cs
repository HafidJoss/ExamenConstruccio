using Microsoft.EntityFrameworkCore.Storage;
using foro.Domain.Entities;
using foro.Domain.Interfaces;
using foro.Infrastructure.Data;

namespace foro.Infrastructure.Repositories;

/// <summary>
/// Implementación del patrón Unit of Work para coordinar operaciones de múltiples repositorios
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ForumDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed = false;

    // Repositorios lazy-loaded
 private IGenericRepository<Usuario>? _usuarios;
    private IGenericRepository<Categoria>? _categorias;
  private IGenericRepository<Tema>? _temas;
    private IGenericRepository<Mensaje>? _mensajes;

    public UnitOfWork(ForumDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Repositorio para la entidad Usuario
    /// </summary>
    public IGenericRepository<Usuario> Usuarios
    {
    get
     {
            _usuarios ??= new GenericRepository<Usuario>(_context);
      return _usuarios;
        }
    }

    /// <summary>
    /// Repositorio para la entidad Categoria
    /// </summary>
    public IGenericRepository<Categoria> Categorias
 {
     get
        {
      _categorias ??= new GenericRepository<Categoria>(_context);
    return _categorias;
        }
 }

    /// <summary>
    /// Repositorio para la entidad Tema
  /// </summary>
    public IGenericRepository<Tema> Temas
    {
 get
        {
            _temas ??= new GenericRepository<Tema>(_context);
            return _temas;
        }
    }

    /// <summary>
    /// Repositorio para la entidad Mensaje
 /// </summary>
    public IGenericRepository<Mensaje> Mensajes
    {
    get
        {
    _mensajes ??= new GenericRepository<Mensaje>(_context);
            return _mensajes;
        }
    }

 /// <summary>
    /// Guarda todos los cambios realizados en el contexto de forma asíncrona
    /// </summary>
    public async Task<int> CommitAsync()
    {
    try
        {
      return await _context.SaveChangesAsync();
        }
  catch (Exception ex)
    {
  // Log the exception (puedes integrar un logger aquí)
            throw new Exception("Error al guardar los cambios en la base de datos", ex);
        }
    }

  /// <summary>
    /// Inicia una transacción de base de datos
    /// </summary>
    public async Task BeginTransactionAsync()
    {
 if (_transaction != null)
        {
  throw new InvalidOperationException("Ya existe una transacción activa");
 }

        _transaction = await _context.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// Confirma la transacción actual
    /// </summary>
    public async Task CommitTransactionAsync()
    {
        if (_transaction == null)
        {
         throw new InvalidOperationException("No hay una transacción activa para confirmar");
        }

        try
  {
            await _context.SaveChangesAsync();
await _transaction.CommitAsync();
  }
        catch
      {
            await RollbackTransactionAsync();
       throw;
        }
        finally
        {
         if (_transaction != null)
            {
  await _transaction.DisposeAsync();
    _transaction = null;
   }
        }
    }

    /// <summary>
    /// Revierte la transacción actual
    /// </summary>
    public async Task RollbackTransactionAsync()
    {
        if (_transaction == null)
        {
 throw new InvalidOperationException("No hay una transacción activa para revertir");
        }

        try
        {
      await _transaction.RollbackAsync();
      }
        finally
 {
     if (_transaction != null)
            {
        await _transaction.DisposeAsync();
            _transaction = null;
  }
        }
    }

    /// <summary>
    /// Libera los recursos utilizados por el Unit of Work
    /// </summary>
  protected virtual void Dispose(bool disposing)
  {
 if (!_disposed)
   {
            if (disposing)
     {
        _transaction?.Dispose();
   _context.Dispose();
   }
 _disposed = true;
        }
    }

    /// <summary>
    /// Libera los recursos utilizados por el Unit of Work
  /// </summary>
 public void Dispose()
    {
     Dispose(true);
        GC.SuppressFinalize(this);
    }
}
