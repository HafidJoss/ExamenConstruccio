using System.ComponentModel.DataAnnotations;

namespace foro.Web.DTOs;

/// <summary>
/// DTO para mostrar información de un tema en el listado
/// </summary>
public class TemaListDto
{
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Contenido { get; set; } = string.Empty;

    public string CategoriaNombre { get; set; } = string.Empty;

    public int CategoriaId { get; set; }

    public string UsuarioNombre { get; set; } = string.Empty;

    public int UsuarioId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaUltimaActividad { get; set; }

    public int NumeroMensajes { get; set; }

    public int Vistas { get; set; }

    public bool Cerrado { get; set; }

    public bool Fijado { get; set; }
}
