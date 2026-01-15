using AutoMapper;
using ERPEscolar.API.Models;
using ERPEscolar.API.DTOs.Finanzas;

namespace ERPEscolar.API.Infrastructure.Mappings;

/// <summary>
/// AutoMapper Profile para mapeos entre Cargo y sus DTOs.
/// Configura las transformaciones automáticas de datos.
/// </summary>
public class CargoProfile : Profile
{
    public CargoProfile()
    {
        // CreateCargoDto -> Cargo (creación)
        CreateMap<CreateCargoDto, Cargo>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Folio, opt => opt.Ignore())  // Se genera en servicio
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src =>
                src.Monto - src.Descuento + src.Recargo))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src =>
                (src.Monto - src.Descuento + src.Recargo) * (1 + src.IVA)))
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(_ => "Pendiente"))
            .ForMember(dest => dest.MontoRecibido, opt => opt.MapFrom(_ => 0m))
            .ForMember(dest => dest.FechaEmision, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.FechaPago, opt => opt.Ignore())  // Null en creación
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(_ => true))  // Default: activo
            .ForMember(dest => dest.Pagos, opt => opt.Ignore())  // No mapear relaciones en creación
            .ForMember(dest => dest.Alumno, opt => opt.Ignore())
            .ForMember(dest => dest.ConceptoCobro, opt => opt.Ignore())
            .ForMember(dest => dest.CicloEscolar, opt => opt.Ignore());

        // Cargo -> CargoDto (lectura básica)
        CreateMap<Cargo, CargoDto>()
            .ForMember(dest => dest.FechaEmision, opt => opt.MapFrom(src => src.FechaEmision))
            .ForMember(dest => dest.FechaVencimiento, opt => opt.MapFrom(src => src.FechaVencimiento))
            .ForMember(dest => dest.FechaPago, opt => opt.MapFrom(src => src.FechaPago));

        // Cargo -> CargoFullDataDto (lectura con relaciones)
        CreateMap<Cargo, CargoFullDataDto>()
            .ForMember(dest => dest.Alumno, opt => opt.MapFrom(src => src.Alumno))
            .ForMember(dest => dest.ConceptoCobro, opt => opt.MapFrom(src => src.ConceptoCobro))
            .ForMember(dest => dest.CicloEscolar, opt => opt.MapFrom(src => src.CicloEscolar))
            .ForMember(dest => dest.Pagos, opt => opt.MapFrom(src => src.Pagos.Where(p => p.Activo)))
            .ForMember(dest => dest.FechaEmision, opt => opt.MapFrom(src => src.FechaEmision))
            .ForMember(dest => dest.FechaVencimiento, opt => opt.MapFrom(src => src.FechaVencimiento))
            .ForMember(dest => dest.FechaPago, opt => opt.MapFrom(src => src.FechaPago));

        // UpdateCargoDto -> Cargo (actualización parcial)
        CreateMap<UpdateCargoDto, Cargo>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AlumnoId, opt => opt.Ignore())
            .ForMember(dest => dest.ConceptoCobroId, opt => opt.Ignore())
            .ForMember(dest => dest.CicloEscolarId, opt => opt.Ignore())
            .ForMember(dest => dest.Folio, opt => opt.Ignore())
            .ForMember(dest => dest.Mes, opt => opt.Ignore())
            .ForMember(dest => dest.Monto, opt => opt.Ignore())
            .ForMember(dest => dest.Descuento, opt => opt.Ignore())
            .ForMember(dest => dest.Recargo, opt => opt.Ignore())
            .ForMember(dest => dest.Subtotal, opt => opt.Ignore())
            .ForMember(dest => dest.IVA, opt => opt.Ignore())
            .ForMember(dest => dest.Total, opt => opt.Ignore())
            .ForMember(dest => dest.FechaEmision, opt => opt.Ignore())
            .ForMember(dest => dest.FechaVencimiento, opt => opt.Ignore())
            .ForMember(dest => dest.Activo, opt => opt.Ignore())
            .ForMember(dest => dest.Pagos, opt => opt.Ignore())
            .ForMember(dest => dest.Alumno, opt => opt.Ignore())
            .ForMember(dest => dest.ConceptoCobro, opt => opt.Ignore())
            .ForMember(dest => dest.CicloEscolar, opt => opt.Ignore())
            .ForMember(dest => dest.FechaPago, opt => opt.MapFrom(src =>
                src.Estado == "Pagado" && !src.FechaPago.HasValue ? DateTime.UtcNow : src.FechaPago));

        // Mapeos para entidades relacionadas (simplificados para Finanzas)
        CreateMap<Alumno, AlumnoDto>()
            .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src =>
                $"{src.Nombre} {src.Apellido}".Trim()));

        CreateMap<ConceptoCobro, ConceptoCobroDto>();

        CreateMap<CicloEscolar, CicloEscolarDto>();

        CreateMap<Pago, PagoDto>();
    }
}
