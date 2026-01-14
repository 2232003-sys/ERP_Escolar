using AutoMapper;
using ERPEscolar.API.Models;
using ERPEscolar.API.DTOs.ControlEscolar;

namespace ERPEscolar.API.Infrastructure.Mappings;

/// <summary>
/// AutoMapper Profile para mapeos entre Alumno y sus DTOs.
/// Configura las transformaciones automáticas de datos.
/// </summary>
public class AlumnoProfile : Profile
{
    public AlumnoProfile()
    {
        // CreateAlumnoDto -> Alumno (creación)
        CreateMap<CreateAlumnoDto, Alumno>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Matricula, opt => opt.Ignore())  // Se genera en servicio
            .ForMember(dest => dest.FechaInscripcion, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore())  // Null en creación
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(_ => true))  // Default: activo
            .ForMember(dest => dest.Tutores, opt => opt.Ignore())  // No mapear relaciones en creación
            .ForMember(dest => dest.Inscripciones, opt => opt.Ignore())
            .ForMember(dest => dest.Asistencias, opt => opt.Ignore())
            .ForMember(dest => dest.Calificaciones, opt => opt.Ignore())
            .ForMember(dest => dest.Cargos, opt => opt.Ignore())
            .ForMember(dest => dest.Pagos, opt => opt.Ignore())
            .ForMember(dest => dest.School, opt => opt.Ignore());

        // Alumno -> AlumnoDto (lectura básica)
        CreateMap<Alumno, AlumnoDto>()
            .ForMember(dest => dest.FechaInscripcion, opt => opt.MapFrom(src => src.FechaInscripcion));

        // UpdateAlumnoDto -> Alumno (actualización parcial)
        // Mapea solo los campos permitidos, ignora todo lo demás
        CreateMap<UpdateAlumnoDto, Alumno>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SchoolId, opt => opt.Ignore())
            .ForMember(dest => dest.CURP, opt => opt.Ignore())  // CURP no se puede cambiar
            .ForMember(dest => dest.Matricula, opt => opt.Ignore())  // Matrícula no se puede cambiar
            .ForMember(dest => dest.FechaNacimiento, opt => opt.Ignore())  // FechaNacimiento no se cambia después
            .ForMember(dest => dest.Sexo, opt => opt.Ignore())  // Sexo no se cambia después
            .ForMember(dest => dest.Activo, opt => opt.Ignore())  // Activo no se cambia aquí (usar DELETE/RESTORE)
            .ForMember(dest => dest.FechaInscripcion, opt => opt.Ignore())  // Fecha inscripción no cambia
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())  // Fecha creación no cambia
            .ForMember(dest => dest.FechaActualizacion, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Tutores, opt => opt.Ignore())  // No actualizar tutores aquí
            .ForMember(dest => dest.Inscripciones, opt => opt.Ignore())
            .ForMember(dest => dest.Asistencias, opt => opt.Ignore())
            .ForMember(dest => dest.Calificaciones, opt => opt.Ignore())
            .ForMember(dest => dest.Cargos, opt => opt.Ignore())
            .ForMember(dest => dest.Pagos, opt => opt.Ignore())
            .ForMember(dest => dest.School, opt => opt.Ignore());

        // Alumno -> AlumnoFullDataDto (lectura completa con relaciones)
        CreateMap<Alumno, AlumnoFullDataDto>()
            .IncludeBase<Alumno, AlumnoDto>()  // Heredar mapeos base
            .ForMember(dest => dest.TutoresNombres, opt => opt.MapFrom(src => 
                src.Tutores.Select(t => $"{t.Nombre} {t.Apellido}").ToList()))
            .ForMember(dest => dest.Inscripciones, opt => opt.MapFrom(src => 
                src.Inscripciones.Select(i => new GrupoInscripcionDto
                {
                    GrupoId = i.GrupoId,
                    GrupoNombre = i.Grupo!.Nombre,
                    CicloEscolarId = i.Grupo!.CicloEscolarId,
                    CicloNombre = i.Grupo!.CicloEscolar!.Nombre,
                    Activo = i.Activo
                }).ToList()));
    }
}
