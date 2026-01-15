
using AutoMapper;
using ERPEscolar.API.Models;
using ERPEscolar.DTOs.ControlEscolar;

namespace ERPEscolar.API.Infrastructure.Mappings
{
    public class GrupoProfile : Profile
    {
        public GrupoProfile()
        {
            // Mapeo para la creación
            CreateMap<CreateGrupoDto, Grupo>();

            // Mapeo para la actualización
            CreateMap<UpdateGrupoDto, Grupo>();

            // Mapeo de la entidad al DTO de respuesta
            CreateMap<Grupo, GrupoDto>()
                .ForMember(dest => dest.CicloEscolarNombre, opt => opt.MapFrom(src => src.CicloEscolar.Nombre));
        }
    }
}
