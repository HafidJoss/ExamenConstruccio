using System.ComponentModel.DataAnnotations;

namespace SalesSuite.Web.DTOs;

/// <summary>
/// DTO para mostrar información de una categoría
/// </summary>
public class CategoriaDto
{
    public int Id { get; set; }

    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;

    [Display(Name = "Slug")]
    public string Slug { get; set; } = string.Empty;

    [Display(Name = "Icono")]
    public string? Icono { get; set; }

    [Display(Name = "Orden")]
    public int Orden { get; set; }

    [Display(Name = "Activa")]
    public bool Activa { get; set; }

    [Display(Name = "Número de Temas")]
    public int NumeroTemas { get; set; }
}
