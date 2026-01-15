using System.ComponentModel.DataAnnotations;

namespace ERPEscolar.API.DTOs.Finanzas;

public class EstadoCuentaDto
{
    public int AlumnoId { get; set; }
    public string AlumnoNombre { get; set; } = null!;
    public string Matricula { get; set; } = null!;
    public decimal SaldoActual { get; set; }
    public decimal TotalCargos { get; set; }
    public decimal TotalPagos { get; set; }
    public decimal SaldoPendiente { get; set; }
    public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;

    public List<CargoEstadoDto> Cargos { get; set; } = [];
    public List<PagoEstadoDto> Pagos { get; set; } = [];
}

public class CargoEstadoDto
{
    public int Id { get; set; }
    public string Folio { get; set; } = null!;
    public string Mes { get; set; } = null!;
    public decimal Monto { get; set; }
    public decimal Descuento { get; set; }
    public decimal Recargo { get; set; }
    public decimal Subtotal { get; set; }
    public decimal IVA { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = null!;
    public decimal MontoRecibido { get; set; }
    public DateTime FechaEmision { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public string Concepto { get; set; } = null!;
}

public class PagoEstadoDto
{
    public int Id { get; set; }
    public string Folio { get; set; } = null!;
    public decimal Monto { get; set; }
    public string Metodo { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public DateTime FechaPago { get; set; }
    public string? ReferenciaExterna { get; set; }
}

public class EstadoCuentaHistorialDto
{
    public IEnumerable<EstadoCuentaDto> EstadosCuenta { get; set; } = [];
    public int TotalCount { get; set; }
}

public class EnviarEstadoCuentaEmailDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    public string? Asunto { get; set; }
    public string? Mensaje { get; set; }
}