using AutoMapper;
using ERPEscolar.API.DTOs.ControlEscolar;
using ERPEscolar.API.Models;

namespace ERPEscolar.API.Infrastructure.Mappings;

/// <summary>
/// Perfil de mapeos para Inscripción.
/// </summary>
public class InscripcionProfile : Profile
{
    public InscripcionProfile()
    {
        // CreateInscripcionDto → Inscripcion
        CreateMap<CreateInscripcionDto, Inscripcion>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.Alumno, opt => opt.Ignore())
            .ForMember(dest => dest.Grupo, opt => opt.Ignore())
            .ForMember(dest => dest.CicloEscolar, opt => opt.Ignore())
            .ForMember(dest => dest.Asistencias, opt => opt.Ignore())
            .ForMember(dest => dest.Calificaciones, opt => opt.Ignore())
            .ForMember(dest => dest.FechaInscripcion, 
                opt => opt.MapFrom(src => src.FechaInscripcion ?? DateTime.UtcNow));

        // Inscripcion → InscripcionDto
        CreateMap<Inscripcion, InscripcionDto>();

        // UpdateInscripcionDto → Inscripcion
        CreateMap<UpdateInscripcionDto, Inscripcion>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AlumnoId, opt => opt.Ignore())
            .ForMember(dest => dest.CicloEscolarId, opt => opt.Ignore())
            .ForMember(dest => dest.Activo, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.Alumno, opt => opt.Ignore())
            .ForMember(dest => dest.Grupo, opt => opt.Ignore())
            .ForMember(dest => dest.CicloEscolar, opt => opt.Ignore())
            .ForMember(dest => dest.Asistencias, opt => opt.Ignore())
            .ForMember(dest => dest.Calificaciones, opt => opt.Ignore());

        // Inscripcion → InscripcionFullDataDto
        CreateMap<Inscripcion, InscripcionFullDataDto>()
            .ForMember(dest => dest.AlumnoNombre, 
                opt => opt.MapFrom(src => $"{src.Alumno.Nombre} {src.Alumno.Apellido}"))
            .ForMember(dest => dest.AlumnoMatricula, 
                opt => opt.MapFrom(src => src.Alumno.Matricula))
            .ForMember(dest => dest.GrupoNombre, 
                opt => opt.MapFrom(src => src.Grupo.Nombre))
            .ForMember(dest => dest.GrupoGrado, 
                opt => opt.MapFrom(src => src.Grupo.Grado))
            .ForMember(dest => dest.CicloNombre, 
                opt => opt.MapFrom(src => src.CicloEscolar.Nombre))
            .ForMember(dest => dest.TotalMaterias, 
                opt => opt.MapFrom(src => src.Grupo.GrupoMaterias.Count(gm => gm.Activo)))
            .ForMember(dest => dest.TotalAsistencias, 
                opt => opt.MapFrom(src => src.Asistencias.Count))
            .ForMember(dest => dest.TotalCalificaciones, 
                opt => opt.MapFrom(src => src.Calificaciones.Count));
    }
}
