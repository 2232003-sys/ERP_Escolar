using AutoMapper;
using ERPEscolar.API.DTOs.Finanzas;
using ERPEscolar.API.Models;

namespace ERPEscolar.API.Infrastructure.Mappings;

public class BecaProfile : Profile
{
    public BecaProfile()
    {
        CreateMap<Beca, BecaDto>()
            .ForMember(dest => dest.AlumnoNombre, opt => opt.MapFrom(src => $"{src.Alumno.Nombre} {src.Alumno.Apellido}"));

        CreateMap<CreateBecaDto, Beca>();

        CreateMap<UpdateBecaDto, Beca>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}