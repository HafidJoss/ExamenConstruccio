namespace SalesSuite.Application.UseCases.Temas;

/// <summary>
/// Comando para crear un nuevo tema con su primer mensaje
/// </summary>
public class CrearTemaCommand
{
    /// <summary>
    /// Título del tema
    /// </summary>
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Contenido del primer mensaje
    /// </summary>
    public string ContenidoMensaje { get; set; } = string.Empty;

    /// <summary>
    /// ID de la categoría
    /// </summary>
    public int CategoriaId { get; set; }

    /// <summary>
    /// ID del usuario que crea el tema
    /// </summary>
    public int UsuarioId { get; set; }

    /// <summary>
    /// Indica si el tema debe estar fijado
    /// </summary>
    public bool Fijado { get; set; }
}
