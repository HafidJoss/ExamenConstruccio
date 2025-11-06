using Microsoft.EntityFrameworkCore.Storage;
using SalesSuite.Domain.Entities;
using SalesSuite.Domain.Interfaces;
using SalesSuite.Infrastructure.Data;

namespace SalesSuite.Infrastructure.Repositories;

/// <summary>
/// Implementación del patrón Unit of Work
/// Coordina el trabajo de múltiples repositorios y gestiona las transacciones
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

    /// <summary>
    /// Constructor del Unit of Work
    /// </summary>
    /// <param name="context">Contexto de base de datos</param>
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
    /// Guarda todos los cambios pendientes en la base de datos de forma asíncrona
    /// </summary>
    /// <returns>Número de registros afectados</returns>
    public async Task<int> CommitAsync()
    {
        try
        {
            return await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log del error (aquí podrías integrar un logger)
            throw new Exception("Error al guardar los cambios en la base de datos", ex);
        }
    }

    /// <summary>
    /// Inicia una transacción de base de datos
    /// </summary>
    /// <returns>Tarea asíncrona</returns>
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
    /// <returns>Tarea asíncrona</returns>
    public async Task CommitTransactionAsync()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No hay ninguna transacción activa para confirmar");
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
    /// <returns>Tarea asíncrona</returns>
    public async Task RollbackTransactionAsync()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No hay ninguna transacción activa para revertir");
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
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Libera los recursos utilizados por el Unit of Work
    /// </summary>
    /// <param name="disposing">Indica si se están liberando recursos administrados</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Liberar la transacción si existe
                _transaction?.Dispose();
                
                // Liberar el contexto
                _context?.Dispose();
            }

            _disposed = true;
        }
    }
}
