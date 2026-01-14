namespace ERPEscolar.API.Models;

/// <summary>
/// Concepto de cobro (Cuota, Inscripción, Transporte, etc.).
/// </summary>
public class ConceptoCobro
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public string Nombre { get; set; } = null!; // "Cuota Mensual", "Inscripción", etc.
    public string Clave { get; set; } = null!; // Para CFDI
    public string? Descripcion { get; set; }
    public decimal MontoBase { get; set; }
    public string TipoConcepto { get; set; } = null!; // "Fijo", "Variable", "Descuento", "Recargo"
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public School School { get; set; } = null!;
    public ICollection<Cargo> Cargos { get; set; } = [];
}

/// <summary>
/// Cargo facturado (deuda) a un alumno.
/// </summary>
public class Cargo
{
    public int Id { get; set; }
    public int AlumnoId { get; set; }
    public int ConceptoCobroId { get; set; }
    public int CicloEscolarId { get; set; }
    public string Folio { get; set; } = null!; // Identificador único
    public string Mes { get; set; } = null!; // "2024-01", "2024-02", etc. (para facturas mensuales)
    public decimal Monto { get; set; }
    public decimal Descuento { get; set; } = 0;
    public decimal Recargo { get; set; } = 0;
    public decimal Subtotal => Monto - Descuento + Recargo;
    public decimal IVA { get; set; } = 0; // 16% default en México
    public decimal Total => Subtotal + IVA;
    public string Estado { get; set; } = "Pendiente"; // "Pendiente", "Parcial", "Pagado", "Cancelado"
    public decimal MontoRecibido { get; set; } = 0;
    public DateTime FechaEmision { get; set; } = DateTime.UtcNow;
    public DateTime? FechaVencimiento { get; set; }
    public DateTime? FechaPago { get; set; }
    public string? Observacion { get; set; }

    public Alumno Alumno { get; set; } = null!;
    public ConceptoCobro ConceptoCobro { get; set; } = null!;
    public CicloEscolar CicloEscolar { get; set; } = null!;
    public ICollection<Pago> Pagos { get; set; } = [];
    public ICollection<CFDI> CFDIs { get; set; } = [];
}

/// <summary>
/// Pago registrado (transferencia, efectivo, etc.).
/// </summary>
public class Pago
{
    public int Id { get; set; }
    public int AlumnoId { get; set; }
    public int? CargoId { get; set; }
    public string Folio { get; set; } = null!;
    public decimal Monto { get; set; }
    public string Metodo { get; set; } = null!; // "Transferencia", "Efectivo", "Tarjeta", "Cheque"
    public string Estado { get; set; } = "Registrado"; // "Registrado", "Verificado", "Depositado"
    public string? ReferenciaExterna { get; set; } // Número de transacción, número de cheque, etc.
    public DateTime FechaPago { get; set; } = DateTime.UtcNow;
    public DateTime? FechaDepósito { get; set; }
    public string? BancoOrigen { get; set; }
    public string? Comprobante { get; set; } // Ruta del archivo comprobante (XML/PDF/imagen)
    public string? Observacion { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public Alumno Alumno { get; set; } = null!;
    public Cargo? Cargo { get; set; }
}

/// <summary>
/// Descuento/Beca asignada a alumno.
/// </summary>
public class Beca
{
    public int Id { get; set; }
    public int AlumnoId { get; set; }
    public string Nombre { get; set; } = null!; // "Beca 50%", "Hermano", etc.
    public string Tipo { get; set; } = null!; // "Porcentaje", "Fijo"
    public decimal Valor { get; set; } // % si Porcentaje, $ si Fijo
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? Justificacion { get; set; }
    public bool Activa { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public Alumno Alumno { get; set; } = null!;
}

/// <summary>
/// Configuración de cálculo de IVA y retenciones (pueden variar por concepto).
/// </summary>
public class ConfiguracionFiscal
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public string RFC { get; set; } = null!;
    public string RazonSocial { get; set; } = null!;
    public string RegimenFiscal { get; set; } = null!; // "Régimen General", "Persona Moral", etc.
    public decimal PorcentajeIVA { get; set; } = 0.16m;
    public decimal PorcentajeISR { get; set; } = 0; // Si aplica
    public string CertificadoRutaArchivo { get; set; } = null!; // Ruta del .cer (para timbrado)
    public string ClavePrivadaRutaArchivo { get; set; } = null!; // Ruta del .key
    public string ClavePrivada { get; set; } = null!; // Contraseña del .key
    public string ProveedorTimbrado { get; set; } = null!; // "FINKOK", "QUADRUM", etc.
    public string? TokenTimbrado { get; set; }
    public bool Activa { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaActualizacion { get; set; }

    public School School { get; set; } = null!;
}
