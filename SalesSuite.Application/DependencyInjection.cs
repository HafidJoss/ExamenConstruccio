using Microsoft.Extensions.DependencyInjection;
using SalesSuite.Application.UseCases.Temas;

namespace SalesSuite.Application;

/// <summary>
/// Clase de extensión para configurar la inyección de dependencias de la capa de aplicación
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Agrega los servicios de aplicación al contenedor de inyección de dependencias
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registrar casos de uso / handlers
        services.AddScoped<ICrearTemaHandler, CrearTemaHandler>();

        return services;
    }
}
