using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesSuite.Domain.Interfaces;
using SalesSuite.Infrastructure.Data;
using SalesSuite.Infrastructure.Repositories;
using SalesSuite.Infrastructure.Seeding;

namespace SalesSuite.Infrastructure;

/// <summary>
/// Clase de extensión para configurar la inyección de dependencias de la capa de infraestructura
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Agrega los servicios de infraestructura al contenedor de inyección de dependencias
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <param name="configuration">Configuración de la aplicación</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configurar el contexto de base de datos
        services.AddDbContext<ForumDbContext>(options =>
        {
            // Obtener la cadena de conexión desde appsettings.json
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                // Configuraciones adicionales de SQL Server
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                
                sqlOptions.CommandTimeout(60);
            });

            // Habilitar logging sensible de datos solo en desarrollo
            #if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            #endif
        });

        // Registrar el patrón Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Registrar repositorios genéricos (opcional, si se necesitan directamente)
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // Registrar el DataSeeder
        services.AddScoped<DataSeeder>();

        return services;
    }

    /// <summary>
    /// Agrega los servicios de infraestructura con opciones personalizadas
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <param name="connectionString">Cadena de conexión a la base de datos</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        // Configurar el contexto de base de datos
        services.AddDbContext<ForumDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                
                sqlOptions.CommandTimeout(60);
            });

            #if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            #endif
        });

        // Registrar el patrón Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Registrar repositorios genéricos
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        return services;
    }
}
