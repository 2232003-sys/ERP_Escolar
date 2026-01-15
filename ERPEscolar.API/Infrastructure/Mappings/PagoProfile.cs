using AutoMapper;
using ERPEscolar.API.DTOs.Finanzas;
using ERPEscolar.API.Models;

namespace ERPEscolar.API.Infrastructure.Mappings;

public class PagoProfile : Profile
{
    public PagoProfile()
    {
        CreateMap<Pago, PagoDto>()
            .ForMember(dest => dest.ColegiaturaId, opt => opt.MapFrom(src => src.CargoId))
            .ForMember(dest => dest.ColegiaturaFolio, opt => opt.MapFrom(src => src.Cargo != null ? src.Cargo.Folio : null))
            .ForMember(dest => dest.AlumnoNombre, opt => opt.MapFrom(src => $"{src.Alumno.Nombre} {src.Alumno.Apellido}"));

        CreateMap<CreatePagoDto, Pago>()
            .ForMember(dest => dest.CargoId, opt => opt.MapFrom(src => src.ColegiaturaId))
            .ForMember(dest => dest.FechaPago, opt => opt.MapFrom(src => src.FechaPago ?? DateTime.UtcNow))
            .ForMember(dest => dest.Folio, opt => opt.Ignore()); // Will be set in service

        CreateMap<UpdatePagoDto, Pago>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<PagoTransferenciaDto, Pago>()
            .ForMember(dest => dest.CargoId, opt => opt.MapFrom(src => src.ColegiaturaId))
            .ForMember(dest => dest.Metodo, opt => opt.MapFrom(src => "Transferencia"))
            .ForMember(dest => dest.FechaPago, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Folio, opt => opt.Ignore()); // Will be set in service
    }
}