namespace SalesSuite.Application.UseCases.Temas;

/// <summary>
/// Resultado de la operación de crear tema
/// </summary>
public class CrearTemaResult
{
    /// <summary>
    /// Indica si la operación fue exitosa
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// ID del tema creado
    /// </summary>
    public int TemaId { get; set; }

    /// <summary>
    /// ID del primer mensaje creado
    /// </summary>
    public int MensajeId { get; set; }

    /// <summary>
    /// Mensaje de error si la operación falló
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Lista de errores de validación
    /// </summary>
    public List<string> ValidationErrors { get; set; } = new();

    /// <summary>
    /// Crea un resultado exitoso
    /// </summary>
    public static CrearTemaResult CreateSuccess(int temaId, int mensajeId)
    {
        return new CrearTemaResult
        {
            Success = true,
            TemaId = temaId,
            MensajeId = mensajeId
        };
    }

    /// <summary>
    /// Crea un resultado de error
    /// </summary>
    public static CrearTemaResult CreateError(string errorMessage)
    {
        return new CrearTemaResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }

    /// <summary>
    /// Crea un resultado con errores de validación
    /// </summary>
    public static CrearTemaResult CreateValidationError(List<string> errors)
    {
        return new CrearTemaResult
        {
            Success = false,
            ErrorMessage = "Errores de validación",
            ValidationErrors = errors
        };
    }
}
