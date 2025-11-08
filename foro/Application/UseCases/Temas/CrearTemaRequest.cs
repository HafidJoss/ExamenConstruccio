using foro.Domain.Entities;

namespace foro.Application.UseCases.Temas;

/// <summary>
/// Request para crear un tema con su mensaje inicial
/// </summary>
public class CrearTemaRequest
{
    public string Titulo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public int UsuarioId { get; set; }
}

/// <summary>
/// Response del caso de uso
/// </summary>
public class CrearTemaResponse
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public int TemaId { get; set; }
    public List<string> Errores { get; set; } = new();
}
