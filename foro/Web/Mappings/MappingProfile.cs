using AutoMapper;
using foro.Domain.Entities;
using foro.Web.DTOs;

namespace foro.Web.Mappings;

/// <summary>
/// Perfil de AutoMapper para configurar los mapeos entre entidades y DTOs
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapeos para Tema
        CreateMap<Tema, TemaListDto>()
     .ForMember(dest => dest.CategoriaNombre, opt => opt.MapFrom(src => src.Categoria.Nombre))
            .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => src.Usuario.Nombre))
       .ForMember(dest => dest.NumeroMensajes, opt => opt.MapFrom(src => src.Mensajes.Count))
       .ForMember(dest => dest.FechaUltimaActividad, opt => opt.MapFrom(src => src.FechaUltimaActividad));

    CreateMap<Tema, TemaEditDto>();

        CreateMap<TemaEditDto, Tema>()
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
    .ForMember(dest => dest.Categoria, opt => opt.Ignore())
    .ForMember(dest => dest.Mensajes, opt => opt.Ignore())
     .ForMember(dest => dest.Contenido, opt => opt.Ignore())
            .ForMember(dest => dest.FechaUltimaActividad, opt => opt.Ignore())
   .ForMember(dest => dest.Cerrado, opt => opt.Ignore())
        .ForMember(dest => dest.Fijado, opt => opt.Ignore())
   .ForMember(dest => dest.Vistas, opt => opt.Ignore());

     CreateMap<TemaCreateDto, Tema>()
   .ForMember(dest => dest.Id, opt => opt.Ignore())
       .ForMember(dest => dest.Usuario, opt => opt.Ignore())
     .ForMember(dest => dest.Categoria, opt => opt.Ignore())
         .ForMember(dest => dest.Mensajes, opt => opt.Ignore())
  .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.UtcNow))
  .ForMember(dest => dest.FechaUltimaActividad, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Cerrado, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.Fijado, opt => opt.MapFrom(src => false))
    .ForMember(dest => dest.Vistas, opt => opt.MapFrom(src => 0));

        CreateMap<Tema, TemaDeleteDto>()
            .ForMember(dest => dest.CategoriaNombre, opt => opt.MapFrom(src => src.Categoria.Nombre))
     .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => src.Usuario.Nombre))
     .ForMember(dest => dest.NumeroMensajes, opt => opt.MapFrom(src => src.Mensajes.Count));

        // Mapeos para Categoria
        CreateMap<Categoria, CategoriaDto>();
    }
}
