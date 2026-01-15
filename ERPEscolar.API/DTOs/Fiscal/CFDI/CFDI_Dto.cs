using System.ComponentModel.DataAnnotations;
using ERPEscolar.API.DTOs.Finanzas;

namespace ERPEscolar.API.DTOs.Fiscal;

/// <summary>
/// DTO básico de CFDI - Respuesta GET
/// </summary>
public class CFDIDto
{
    /// <summary>
    /// Identificador único del CFDI
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del cargo relacionado
    /// </summary>
    public int CargoId { get; set; }

    /// <summary>
    /// UUID del SAT (folio fiscal)
    /// </summary>
    public string UUID { get; set; } = string.Empty;

    /// <summary>
    /// Serie de la factura
    /// </summary>
    public string Serie { get; set; } = string.Empty;

    /// <summary>
    /// Folio de la factura
    /// </summary>
    public string Folio { get; set; } = string.Empty;

    /// <summary>
    /// RFC del emisor (escuela)
    /// </summary>
    public string RFC_Emisor { get; set; } = string.Empty;

    /// <summary>
    /// RFC del receptor
    /// </summary>
    public string RFC_Receptor { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del receptor
    /// </summary>
    public string NombreReceptor { get; set; } = string.Empty;

    /// <summary>
    /// Subtotal del CFDI
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Descuento aplicado
    /// </summary>
    public decimal Descuento { get; set; }

    /// <summary>
    /// IVA aplicado
    /// </summary>
    public decimal IVA { get; set; }

    /// <summary>
    /// Total del CFDI
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Estado del CFDI: Borrador, Timbrada, Cancelada, Error
    /// </summary>
    public string Estado { get; set; } = string.Empty;

    /// <summary>
    /// Nivel educativo para complemento IEDU
    /// </summary>
    public string? NivelEducativo { get; set; }

    /// <summary>
    /// CURP del alumno
    /// </summary>
    public string? CURP_Alumno { get; set; }

    /// <summary>
    /// Clave de Centro de Trabajo
    /// </summary>
    public string? ClaveCT { get; set; }

    /// <summary>
    /// Fecha de timbrado
    /// </summary>
    public DateTime FechaTimbrado { get; set; }

    /// <summary>
    /// Fecha de cancelación (si aplica)
    /// </summary>
    public DateTime? FechaCancelacion { get; set; }

    /// <summary>
    /// Razón de cancelación
    /// </summary>
    public string? RazonCancelacion { get; set; }

    /// <summary>
    /// Error de timbrado (si aplica)
    /// </summary>
    public string? ErrorTimbrado { get; set; }
}

/// <summary>
/// DTO para crear un nuevo CFDI - POST
/// </summary>
public class CreateCFDIDto
{
    /// <summary>
    /// Identificador del cargo a facturar
    /// </summary>
    [Required(ErrorMessage = "El cargo es requerido")]
    public int CargoId { get; set; }

    /// <summary>
    /// RFC del receptor (tutor/alumno)
    /// </summary>
    [Required(ErrorMessage = "El RFC del receptor es requerido")]
    [RegularExpression(@"^[A-ZÑ&]{3,4}[0-9]{6}[A-Z0-9]{3}$", ErrorMessage = "RFC inválido")]
    public string RFC_Receptor { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del receptor
    /// </summary>
    [Required(ErrorMessage = "El nombre del receptor es requerido")]
    [StringLength(254, ErrorMessage = "El nombre no puede exceder 254 caracteres")]
    public string NombreReceptor { get; set; } = string.Empty;

    /// <summary>
    /// Nivel educativo para complemento IEDU
    /// </summary>
    [Required(ErrorMessage = "El nivel educativo es requerido")]
    [StringLength(50, ErrorMessage = "El nivel educativo no puede exceder 50 caracteres")]
    public string NivelEducativo { get; set; } = string.Empty;

    /// <summary>
    /// CURP del alumno
    /// </summary>
    [Required(ErrorMessage = "La CURP del alumno es requerida")]
    [RegularExpression(@"^[A-Z]{1}[AEIOU]{1}[A-Z]{2}[0-9]{2}(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1])[HM]{1}(AS|BC|BS|CC|CS|CH|CL|CM|DF|DG|GT|GR|HG|JC|MC|MN|MS|NT|NL|OC|PL|QT|QR|SP|SL|SR|TC|TS|TL|TM|VZ|YN|ZS|NE)[B-DF-HJ-NP-TV-Z]{3}[0-9A-Z]{1}[0-9]{1}$", ErrorMessage = "CURP inválida")]
    public string CURP_Alumno { get; set; } = string.Empty;

    /// <summary>
    /// Clave de Centro de Trabajo (SEP)
    /// </summary>
    [Required(ErrorMessage = "La clave CT es requerida")]
    [StringLength(10, ErrorMessage = "La clave CT debe tener máximo 10 caracteres")]
    public string ClaveCT { get; set; } = string.Empty;

    /// <summary>
    /// Serie de la factura (opcional, se genera automáticamente si no se proporciona)
    /// </summary>
    [StringLength(25, ErrorMessage = "La serie no puede exceder 25 caracteres")]
    public string? Serie { get; set; }

    /// <summary>
    /// Folio de la factura (opcional, se genera automáticamente si no se proporciona)
    /// </summary>
    [StringLength(40, ErrorMessage = "El folio no puede exceder 40 caracteres")]
    public string? Folio { get; set; }
}

/// <summary>
/// DTO para actualizar un CFDI - PUT
/// </summary>
public class UpdateCFDIDto
{
    /// <summary>
    /// Nuevo estado del CFDI
    /// </summary>
    [Required(ErrorMessage = "El estado es requerido")]
    [StringLength(20, ErrorMessage = "El estado debe tener máximo 20 caracteres")]
    public string Estado { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de cancelación (requerida si estado es Cancelada)
    /// </summary>
    public DateTime? FechaCancelacion { get; set; }

    /// <summary>
    /// Razón de cancelación
    /// </summary>
    [StringLength(500, ErrorMessage = "La razón de cancelación no puede exceder 500 caracteres")]
    public string? RazonCancelacion { get; set; }

    /// <summary>
    /// Error de timbrado
    /// </summary>
    [StringLength(1000, ErrorMessage = "El error de timbrado no puede exceder 1000 caracteres")]
    public string? ErrorTimbrado { get; set; }
}

/// <summary>
/// DTO de CFDI con datos completos (relaciones incluidas)
/// </summary>
public class CFDIFullDataDto
{
    /// <summary>
    /// Identificador único del CFDI
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Datos del cargo relacionado
    /// </summary>
    public CargoDto? Cargo { get; set; }

    /// <summary>
    /// UUID del SAT
    /// </summary>
    public string UUID { get; set; } = string.Empty;

    /// <summary>
    /// Serie de la factura
    /// </summary>
    public string Serie { get; set; } = string.Empty;

    /// <summary>
    /// Folio de la factura
    /// </summary>
    public string Folio { get; set; } = string.Empty;

    /// <summary>
    /// RFC del emisor
    /// </summary>
    public string RFC_Emisor { get; set; } = string.Empty;

    /// <summary>
    /// RFC del receptor
    /// </summary>
    public string RFC_Receptor { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del receptor
    /// </summary>
    public string NombreReceptor { get; set; } = string.Empty;

    /// <summary>
    /// Subtotal
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Descuento
    /// </summary>
    public decimal Descuento { get; set; }

    /// <summary>
    /// IVA
    /// </summary>
    public decimal IVA { get; set; }

    /// <summary>
    /// Total
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Estado
    /// </summary>
    public string Estado { get; set; } = string.Empty;

    /// <summary>
    /// Nivel educativo
    /// </summary>
    public string? NivelEducativo { get; set; }

    /// <summary>
    /// CURP del alumno
    /// </summary>
    public string? CURP_Alumno { get; set; }

    /// <summary>
    /// Clave CT
    /// </summary>
    public string? ClaveCT { get; set; }

    /// <summary>
    /// Fecha de timbrado
    /// </summary>
    public DateTime FechaTimbrado { get; set; }

    /// <summary>
    /// Fecha de cancelación
    /// </summary>
    public DateTime? FechaCancelacion { get; set; }

    /// <summary>
    /// Razón de cancelación
    /// </summary>
    public string? RazonCancelacion { get; set; }

    /// <summary>
    /// Error de timbrado
    /// </summary>
    public string? ErrorTimbrado { get; set; }

    /// <summary>
    /// Cadena original
    /// </summary>
    public string? CadenaOriginal { get; set; }

    /// <summary>
    /// XML con timbrado
    /// </summary>
    public string? XMLConTimbrado { get; set; }

    /// <summary>
    /// Lista de bitácoras fiscales
    /// </summary>
    public List<BitacoraFiscalDto> Bitacoras { get; set; } = [];
}

/// <summary>
/// DTO paginado de CFDI
/// </summary>
public class PaginatedCFDISDto
{
    /// <summary>
    /// Lista de CFDI
    /// </summary>
    public List<CFDIDto> Items { get; set; } = [];

    /// <summary>
    /// Número de página actual (1-indexed)
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Cantidad de registros por página
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de registros
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// Total de páginas
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

    /// <summary>
    /// ¿Hay página siguiente?
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// ¿Hay página anterior?
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
}
