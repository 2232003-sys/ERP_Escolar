using AutoMapper;
using ERPEscolar.API.DTOs.ControlEscolar;
using ERPEscolar.API.Models;

namespace ERPEscolar.API.Infrastructure.Mappings;

/// <summary>
/// Perfil de AutoMapper para Calificaciones
/// </summary>
public class CalificacionProfile : Profile
{
    public CalificacionProfile()
    {
        // Calificacion -> CalificacionDto
        CreateMap<Calificacion, CalificacionDto>()
            .ForMember(dest => dest.AlumnoNombre,
                opt => opt.MapFrom(src => $"{src.Alumno.Nombre} {src.Alumno.Apellido}"))
            .ForMember(dest => dest.MateriaNombre,
                opt => opt.MapFrom(src => src.GrupoMateria.Materia.Nombre))
            .ForMember(dest => dest.PeriodoNombre,
                opt => opt.MapFrom(src => src.PeriodoCalificacion.Nombre))
            .ForMember(dest => dest.DocenteNombre,
                opt => opt.MapFrom(src => src.DocenteQueQualifica != null
                    ? $"{src.DocenteQueQualifica.Nombre} {src.DocenteQueQualifica.Apellido}"
                    : null));

        // CreateCalificacionDto -> Calificacion
        CreateMap<CreateCalificacionDto, Calificacion>()
            .ForMember(dest => dest.Aprobado,
                opt => opt.MapFrom(src => src.Calificacion1 >= 6.0m));

        // UpdateCalificacionDto -> Calificacion
        CreateMap<UpdateCalificacionDto, Calificacion>()
            .ForMember(dest => dest.Aprobado,
                opt => opt.MapFrom(src => src.Calificacion1 >= 6.0m));
    }
}