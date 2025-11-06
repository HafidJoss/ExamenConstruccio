namespace SalesSuite.Application.UseCases.Temas;

/// <summary>
/// Interfaz para el handler que crea un tema con su primer mensaje
/// </summary>
public interface ICrearTemaHandler
{
    /// <summary>
    /// Ejecuta el caso de uso de crear tema con mensaje inicial
    /// </summary>
    /// <param name="command">Comando con los datos del tema y mensaje</param>
    /// <returns>Resultado de la operación</returns>
    Task<CrearTemaResult> HandleAsync(CrearTemaCommand command);
}
