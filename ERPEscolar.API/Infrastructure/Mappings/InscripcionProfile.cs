
using AutoMapper;
using ERPEscolar.API.Models;
using ERPEscolar.DTOs.ControlEscolar;

namespace ERPEscolar.API.Infrastructure.Mappings
{
    public class InscripcionProfile : Profile
    {
        public InscripcionProfile()
        {
            CreateMap<CreateInscripcionDto, Inscripcion>();

            CreateMap<Inscripcion, InscripcionDto>()
                .ForMember(dest => dest.AlumnoNombre, opt => opt.MapFrom(src => $"{src.Alumno.Nombres} {src.Alumno.ApellidoPaterno} {src.Alumno.ApellidoMaterno}"))
                .ForMember(dest => dest.GrupoNombre, opt => opt.MapFrom(src => src.Grupo.Nombre));
        }
    }
}
