using AutoMapper;
using ERPEscolar.API.Models;
using ERPEscolar.API.DTOs.Fiscal;
using ERPEscolar.API.DTOs.Finanzas;

namespace ERPEscolar.API.Infrastructure.Mappings;

/// <summary>
/// AutoMapper Profile para mapeos entre CFDI y sus DTOs.
/// Configura las transformaciones automáticas de datos.
/// </summary>
public class CFDIProfile : Profile
{
    public CFDIProfile()
    {
        // CreateCFDIDto -> CFDI (creación)
        CreateMap<CreateCFDIDto, CFDI>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UUID, opt => opt.Ignore())  // Se genera en timbrado
            .ForMember(dest => dest.Serie, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.Serie) ? "A" : src.Serie))
            .ForMember(dest => dest.Folio, opt => opt.Ignore())  // Se genera automáticamente
            .ForMember(dest => dest.RFC_Emisor, opt => opt.Ignore())  // Se obtiene de configuración
            .ForMember(dest => dest.Subtotal, opt => opt.Ignore())  // Se calcula del cargo
            .ForMember(dest => dest.Descuento, opt => opt.Ignore())
            .ForMember(dest => dest.IVA, opt => opt.Ignore())
            .ForMember(dest => dest.Total, opt => opt.Ignore())
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(_ => "Borrador"))
            .ForMember(dest => dest.RazonCancelacion, opt => opt.Ignore())
            .ForMember(dest => dest.RutaXML, opt => opt.Ignore())
            .ForMember(dest => dest.RutaPDF, opt => opt.Ignore())
            .ForMember(dest => dest.XMLConTimbrado, opt => opt.Ignore())
            .ForMember(dest => dest.CadenaOriginal, opt => opt.Ignore())
            .ForMember(dest => dest.FechaTimbrado, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.FechaCancelacion, opt => opt.Ignore())
            .ForMember(dest => dest.ErrorTimbrado, opt => opt.Ignore())
            .ForMember(dest => dest.ReintentosTimbrado, opt => opt.MapFrom(_ => 0))
            .ForMember(dest => dest.Cargo, opt => opt.Ignore())
            .ForMember(dest => dest.Bitacoras, opt => opt.Ignore());

        // CFDI -> CFDIDto (lectura básica)
        CreateMap<CFDI, CFDIDto>()
            .ForMember(dest => dest.FechaTimbrado, opt => opt.MapFrom(src => src.FechaTimbrado))
            .ForMember(dest => dest.FechaCancelacion, opt => opt.MapFrom(src => src.FechaCancelacion));

        // CFDI -> CFDIFullDataDto (lectura con relaciones)
        CreateMap<CFDI, CFDIFullDataDto>()
            .ForMember(dest => dest.Cargo, opt => opt.MapFrom(src => src.Cargo))
            .ForMember(dest => dest.Bitacoras, opt => opt.MapFrom(src => src.Bitacoras.OrderByDescending(b => b.Timestamp)))
            .ForMember(dest => dest.FechaTimbrado, opt => opt.MapFrom(src => src.FechaTimbrado))
            .ForMember(dest => dest.FechaCancelacion, opt => opt.MapFrom(src => src.FechaCancelacion));

        // UpdateCFDIDto -> CFDI (actualización parcial)
        CreateMap<UpdateCFDIDto, CFDI>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CargoId, opt => opt.Ignore())
            .ForMember(dest => dest.UUID, opt => opt.Ignore())
            .ForMember(dest => dest.Serie, opt => opt.Ignore())
            .ForMember(dest => dest.Folio, opt => opt.Ignore())
            .ForMember(dest => dest.RFC_Emisor, opt => opt.Ignore())
            .ForMember(dest => dest.RFC_Receptor, opt => opt.Ignore())
            .ForMember(dest => dest.NombreReceptor, opt => opt.Ignore())
            .ForMember(dest => dest.Subtotal, opt => opt.Ignore())
            .ForMember(dest => dest.Descuento, opt => opt.Ignore())
            .ForMember(dest => dest.IVA, opt => opt.Ignore())
            .ForMember(dest => dest.Total, opt => opt.Ignore())
            .ForMember(dest => dest.NivelEducativo, opt => opt.Ignore())
            .ForMember(dest => dest.CURP_Alumno, opt => opt.Ignore())
            .ForMember(dest => dest.ClaveCT, opt => opt.Ignore())
            .ForMember(dest => dest.RutaXML, opt => opt.Ignore())
            .ForMember(dest => dest.RutaPDF, opt => opt.Ignore())
            .ForMember(dest => dest.XMLConTimbrado, opt => opt.Ignore())
            .ForMember(dest => dest.CadenaOriginal, opt => opt.Ignore())
            .ForMember(dest => dest.ReintentosTimbrado, opt => opt.Ignore())
            .ForMember(dest => dest.Cargo, opt => opt.Ignore())
            .ForMember(dest => dest.Bitacoras, opt => opt.Ignore())
            .ForMember(dest => dest.FechaTimbrado, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCancelacion, opt => opt.MapFrom(src =>
                src.Estado == "Cancelada" && !src.FechaCancelacion.HasValue ? DateTime.UtcNow : src.FechaCancelacion));

        // Mapeos para entidades relacionadas
        CreateMap<Cargo, CargoDto>();

        CreateMap<BitacoraFiscal, BitacoraFiscalDto>();

        CreateMap<ConfiguracionCFDI, ConfiguracionCFDIDto>();

        CreateMap<ComplementoEducativo, ComplementoEducativoDto>();
    }
}
