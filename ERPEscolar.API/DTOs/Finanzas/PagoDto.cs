using System.ComponentModel.DataAnnotations;

namespace ERPEscolar.API.DTOs.Finanzas;

public class PagoDto
{
    public int Id { get; set; }
    public int AlumnoId { get; set; }
    public int? ColegiaturaId { get; set; } // Changed from CargoId to ColegiaturaId
    public string Folio { get; set; } = null!;
    public decimal Monto { get; set; }
    public string Metodo { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public string? ReferenciaExterna { get; set; }
    public DateTime FechaPago { get; set; }
    public DateTime? FechaDepósito { get; set; }
    public string? BancoOrigen { get; set; }
    public string? Comprobante { get; set; }
    public string? Observacion { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }

    // Navigation properties
    public string AlumnoNombre { get; set; } = null!;
    public string? ColegiaturaFolio { get; set; }
}

public class CreatePagoDto
{
    [Required]
    public int AlumnoId { get; set; }

    public int? ColegiaturaId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Monto { get; set; }

    [Required]
    [StringLength(50)]
    public string Metodo { get; set; } = null!;

    [StringLength(100)]
    public string? ReferenciaExterna { get; set; }

    public DateTime? FechaPago { get; set; }

    [StringLength(100)]
    public string? BancoOrigen { get; set; }

    [StringLength(500)]
    public string? Observacion { get; set; }
}

public class UpdatePagoDto
{
    [Range(0.01, double.MaxValue)]
    public decimal? Monto { get; set; }

    [StringLength(50)]
    public string? Metodo { get; set; }

    [StringLength(100)]
    public string? ReferenciaExterna { get; set; }

    public DateTime? FechaPago { get; set; }

    [StringLength(100)]
    public string? BancoOrigen { get; set; }

    [StringLength(500)]
    public string? Observacion { get; set; }
}

public class PagoTransferenciaDto
{
    [Required]
    public int AlumnoId { get; set; }

    public int? ColegiaturaId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Monto { get; set; }

    [Required]
    [StringLength(100)]
    public string ReferenciaExterna { get; set; }

    [StringLength(100)]
    public string? BancoOrigen { get; set; }

    [StringLength(500)]
    public string? Observacion { get; set; }
}

public class PaginatedPagosDto
{
    public IEnumerable<PagoDto> Pagos { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public class PagoFullDataDto : PagoDto
{
    // Additional data if needed
}