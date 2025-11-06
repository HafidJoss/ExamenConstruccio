using AutoMapper;
using SalesSuite.Domain.Entities;
using SalesSuite.Web.DTOs;

namespace SalesSuite.Web.Mappings;

/// <summary>
/// Perfil de AutoMapper para configurar los mapeos entre entidades y DTOs
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapeos para Tema
        CreateMap<Tema, TemaDto>()
            .ForMember(dest => dest.CategoriaNombre, 
                opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nombre : string.Empty))
            .ForMember(dest => dest.UsuarioNombre, 
                opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.NombreUsuario : string.Empty))
            .ForMember(dest => dest.NumeroRespuestas, 
                opt => opt.MapFrom(src => src.Mensajes != null ? src.Mensajes.Count : 0));

        CreateMap<TemaCreateDto, Tema>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => GenerarSlug(src.Titulo)))
            .ForMember(dest => dest.NumeroVistas, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.Cerrado, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.FechaUltimaActividad, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Categoria, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.Mensajes, opt => opt.Ignore());

        CreateMap<TemaEditDto, Tema>()
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => GenerarSlug(src.Titulo)))
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.NumeroVistas, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioId, opt => opt.Ignore())
            .ForMember(dest => dest.FechaUltimaActividad, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Categoria, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.Mensajes, opt => opt.Ignore());

        CreateMap<Tema, TemaEditDto>();

        // Mapeos para Categoria
        CreateMap<Categoria, CategoriaDto>()
            .ForMember(dest => dest.NumeroTemas, 
                opt => opt.MapFrom(src => src.Temas != null ? src.Temas.Count : 0));

        // Mapeos para Usuario
        CreateMap<Usuario, UsuarioDto>();
    }

    /// <summary>
    /// Genera un slug amigable para URLs a partir de un título
    /// </summary>
    /// <param name="titulo">Título del tema</param>
    /// <returns>Slug generado</returns>
    private static string GenerarSlug(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            return string.Empty;

        // Convertir a minúsculas
        string slug = titulo.ToLowerInvariant();

        // Reemplazar caracteres especiales
        slug = slug.Replace("á", "a").Replace("é", "e").Replace("í", "i")
                   .Replace("ó", "o").Replace("ú", "u").Replace("ñ", "n");

        // Remover caracteres no alfanuméricos excepto espacios y guiones
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");

        // Reemplazar espacios múltiples por uno solo
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", " ").Trim();

        // Reemplazar espacios por guiones
        slug = slug.Replace(" ", "-");

        // Remover guiones múltiples
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-");

        // Limitar longitud
        if (slug.Length > 100)
            slug = slug.Substring(0, 100).TrimEnd('-');

        return slug;
    }
}
