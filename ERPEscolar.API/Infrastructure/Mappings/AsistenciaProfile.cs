using AutoMapper;
using ERPEscolar.API.Models;
using ERPEscolar.API.DTOs.ControlEscolar;

namespace ERPEscolar.API.Infrastructure.Mappings;

/// <summary>
/// Perfil de AutoMapper para entidad Asistencia
/// </summary>
public class AsistenciaProfile : Profile
{
    public AsistenciaProfile()
    {
        // CreateAsistenciaDto -> Asistencia
        CreateMap<CreateAsistenciaDto, Asistencia>()
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(_ => true));

        // Asistencia -> AsistenciaDto
        CreateMap<Asistencia, AsistenciaDto>();

        // UpdateAsistenciaDto -> Asistencia (solo actualiza campos específicos)
        CreateMap<UpdateAsistenciaDto, Asistencia>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.InscripcionId, opt => opt.Ignore())
            .ForMember(dest => dest.GrupoMateriaId, opt => opt.Ignore())
            .ForMember(dest => dest.Fecha, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.Activo, opt => opt.Ignore());

        // Asistencia -> AsistenciaFullDataDto (con relaciones)
        CreateMap<Asistencia, AsistenciaFullDataDto>()
            .ForMember(dest => dest.AlumnoNombre, opt => opt.MapFrom(src =>
                $"{src.Inscripcion.Alumno.Apellido}, {src.Inscripcion.Alumno.Nombre}"))
            .ForMember(dest => dest.MateriaNombre, opt => opt.MapFrom(src =>
                src.GrupoMateria.Materia.Nombre));
    }
}
