using System.ComponentModel.DataAnnotations;

namespace ERPEscolar.API.DTOs.Finanzas;

/// <summary>
/// DTO básico de Cargo - Respuesta GET
/// </summary>
public class CargoDto
{
    /// <summary>
    /// Identificador único del cargo
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del alumno
    /// </summary>
    public int AlumnoId { get; set; }

    /// <summary>
    /// Identificador del concepto de cobro
    /// </summary>
    public int ConceptoCobroId { get; set; }

    /// <summary>
    /// Identificador del ciclo escolar
    /// </summary>
    public int CicloEscolarId { get; set; }

    /// <summary>
    /// Folio único del cargo
    /// </summary>
    public string Folio { get; set; } = string.Empty;

    /// <summary>
    /// Mes del cargo (formato YYYY-MM)
    /// </summary>
    public string Mes { get; set; } = string.Empty;

    /// <summary>
    /// Monto base del cargo
    /// </summary>
    public decimal Monto { get; set; }

    /// <summary>
    /// Descuento aplicado
    /// </summary>
    public decimal Descuento { get; set; }

    /// <summary>
    /// Recargo aplicado
    /// </summary>
    public decimal Recargo { get; set; }

    /// <summary>
    /// Subtotal (Monto - Descuento + Recargo)
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// IVA aplicado
    /// </summary>
    public decimal IVA { get; set; }

    /// <summary>
    /// Total del cargo
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Estado del cargo: Pendiente, Parcial, Pagado, Cancelado
    /// </summary>
    public string Estado { get; set; } = string.Empty;

    /// <summary>
    /// Monto recibido hasta ahora
    /// </summary>
    public decimal MontoRecibido { get; set; }

    /// <summary>
    /// Fecha de emisión del cargo
    /// </summary>
    public DateTime FechaEmision { get; set; }

    /// <summary>
    /// Fecha de vencimiento
    /// </summary>
    public DateTime? FechaVencimiento { get; set; }

    /// <summary>
    /// Fecha de pago completo
    /// </summary>
    public DateTime? FechaPago { get; set; }

    /// <summary>
    /// Observaciones del cargo
    /// </summary>
    public string? Observacion { get; set; }
}

/// <summary>
/// DTO para crear un nuevo cargo - POST
/// </summary>
public class CreateCargoDto
{
    /// <summary>
    /// Identificador del alumno
    /// </summary>
    [Required(ErrorMessage = "El alumno es requerido")]
    public int AlumnoId { get; set; }

    /// <summary>
    /// Identificador del concepto de cobro
    /// </summary>
    [Required(ErrorMessage = "El concepto de cobro es requerido")]
    public int ConceptoCobroId { get; set; }

    /// <summary>
    /// Identificador del ciclo escolar
    /// </summary>
    [Required(ErrorMessage = "El ciclo escolar es requerido")]
    public int CicloEscolarId { get; set; }

    /// <summary>
    /// Mes del cargo (formato YYYY-MM)
    /// </summary>
    [Required(ErrorMessage = "El mes es requerido")]
    [RegularExpression(@"^\d{4}-\d{2}$", ErrorMessage = "El mes debe tener formato YYYY-MM")]
    public string Mes { get; set; } = string.Empty;

    /// <summary>
    /// Monto base del cargo
    /// </summary>
    [Required(ErrorMessage = "El monto es requerido")]
    [Range(0.01, 999999.99, ErrorMessage = "El monto debe ser mayor a 0")]
    public decimal Monto { get; set; }

    /// <summary>
    /// Descuento aplicado
    /// </summary>
    [Range(0, 999999.99, ErrorMessage = "El descuento no puede ser negativo")]
    public decimal Descuento { get; set; } = 0;

    /// <summary>
    /// Recargo aplicado
    /// </summary>
    [Range(0, 999999.99, ErrorMessage = "El recargo no puede ser negativo")]
    public decimal Recargo { get; set; } = 0;

    /// <summary>
    /// IVA aplicado (por defecto 16%)
    /// </summary>
    [Range(0, 1, ErrorMessage = "El IVA debe estar entre 0 y 1")]
    public decimal IVA { get; set; } = 0.16m;

    /// <summary>
    /// Fecha de vencimiento
    /// </summary>
    public DateTime? FechaVencimiento { get; set; }

    /// <summary>
    /// Observaciones
    /// </summary>
    [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
    public string? Observacion { get; set; }
}

/// <summary>
/// DTO para actualizar un cargo - PUT
/// </summary>
public class UpdateCargoDto
{
    /// <summary>
    /// Nuevo estado del cargo
    /// </summary>
    [Required(ErrorMessage = "El estado es requerido")]
    [StringLength(20, ErrorMessage = "El estado debe tener máximo 20 caracteres")]
    public string Estado { get; set; } = string.Empty;

    /// <summary>
    /// Monto recibido actualizado
    /// </summary>
    [Range(0, 999999.99, ErrorMessage = "El monto recibido no puede ser negativo")]
    public decimal MontoRecibido { get; set; }

    /// <summary>
    /// Fecha de pago (si se marca como pagado)
    /// </summary>
    public DateTime? FechaPago { get; set; }

    /// <summary>
    /// Observaciones actualizadas
    /// </summary>
    [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
    public string? Observacion { get; set; }
}

/// <summary>
/// DTO de Cargo con datos completos (relaciones incluidas)
/// </summary>
public class CargoFullDataDto
{
    /// <summary>
    /// Identificador único del cargo
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Datos del alumno
    /// </summary>
    public AlumnoDto? Alumno { get; set; }

    /// <summary>
    /// Datos del concepto de cobro
    /// </summary>
    public ConceptoCobroDto? ConceptoCobro { get; set; }

    /// <summary>
    /// Datos del ciclo escolar
    /// </summary>
    public CicloEscolarDto? CicloEscolar { get; set; }

    /// <summary>
    /// Folio único del cargo
    /// </summary>
    public string Folio { get; set; } = string.Empty;

    /// <summary>
    /// Mes del cargo
    /// </summary>
    public string Mes { get; set; } = string.Empty;

    /// <summary>
    /// Monto base
    /// </summary>
    public decimal Monto { get; set; }

    /// <summary>
    /// Descuento
    /// </summary>
    public decimal Descuento { get; set; }

    /// <summary>
    /// Recargo
    /// </summary>
    public decimal Recargo { get; set; }

    /// <summary>
    /// Subtotal
    /// </summary>
    public decimal Subtotal { get; set; }

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
    /// Monto recibido
    /// </summary>
    public decimal MontoRecibido { get; set; }

    /// <summary>
    /// Fecha de emisión
    /// </summary>
    public DateTime FechaEmision { get; set; }

    /// <summary>
    /// Fecha de vencimiento
    /// </summary>
    public DateTime? FechaVencimiento { get; set; }

    /// <summary>
    /// Fecha de pago
    /// </summary>
    public DateTime? FechaPago { get; set; }

    /// <summary>
    /// Observaciones
    /// </summary>
    public string? Observacion { get; set; }

    /// <summary>
    /// Lista de pagos asociados
    /// </summary>
    public List<PagoDto> Pagos { get; set; } = [];
}

/// <summary>
/// DTO paginado de Cargos
/// </summary>
public class PaginatedCargosDto
{
    /// <summary>
    /// Lista de cargos
    /// </summary>
    public List<CargoDto> Items { get; set; } = [];

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
