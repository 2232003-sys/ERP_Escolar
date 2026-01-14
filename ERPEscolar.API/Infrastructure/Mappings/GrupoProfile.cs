using AutoMapper;
using ERPEscolar.API.Models;
using ERPEscolar.API.DTOs.ControlEscolar;

namespace ERPEscolar.API.Infrastructure.Mappings;

/// <summary>
/// AutoMapper Profile para mapeos entre Grupo y sus DTOs.
/// Configura las transformaciones automáticas de datos.
/// </summary>
public class GrupoProfile : Profile
{
    public GrupoProfile()
    {
        // CreateGrupoDto -> Grupo (creación)
        CreateMap<CreateGrupoDto, Grupo>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(_ => true))  // Default: activo
            .ForMember(dest => dest.CicloEscolar, opt => opt.Ignore())  // No mapear relaciones
            .ForMember(dest => dest.School, opt => opt.Ignore())
            .ForMember(dest => dest.DocenteTutor, opt => opt.Ignore())
            .ForMember(dest => dest.Inscripciones, opt => opt.Ignore())
            .ForMember(dest => dest.GrupoMaterias, opt => opt.Ignore());

        // Grupo -> GrupoDto (lectura básica)
        CreateMap<Grupo, GrupoDto>();

        // UpdateGrupoDto -> Grupo (actualización parcial)
        CreateMap<UpdateGrupoDto, Grupo>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SchoolId, opt => opt.Ignore())
            .ForMember(dest => dest.CicloEscolarId, opt => opt.Ignore())
            .ForMember(dest => dest.Activo, opt => opt.Ignore())  // No cambiar estado aquí
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.CicloEscolar, opt => opt.Ignore())
            .ForMember(dest => dest.School, opt => opt.Ignore())
            .ForMember(dest => dest.DocenteTutor, opt => opt.Ignore())
            .ForMember(dest => dest.Inscripciones, opt => opt.Ignore())
            .ForMember(dest => dest.GrupoMaterias, opt => opt.Ignore());

        // Grupo -> GrupoFullDataDto (lectura con datos relacionados)
        CreateMap<Grupo, GrupoFullDataDto>()
            .ForMember(dest => dest.CicloNombre, opt => opt.MapFrom(src => src.CicloEscolar.Nombre))
            .ForMember(dest => dest.DocenteTutorNombre, opt => opt.MapFrom(src => 
                src.DocenteTutor != null ? $"{src.DocenteTutor.Nombre} {src.DocenteTutor.Apellido}" : null))
            .ForMember(dest => dest.AlumnosInscritos, opt => opt.MapFrom(src => 
                src.Inscripciones.Count(i => i.Activo)));
    }
}
