namespace ERPEscolar.API.Models;

/// <summary>
/// Factura electrónica (CFDI 4.0).
/// Almacena metadatos y referencias a XML/PDF/UUID.
/// </summary>
public class CFDI
{
    public int Id { get; set; }
    public int CargoId { get; set; }
    public string UUID { get; set; } = null!; // Folio fiscal del SAT
    public string Serie { get; set; } = null!; // Serie de la factura
    public string Folio { get; set; } = null!; // Folio de la factura
    public string RFC_Emisor { get; set; } = null!; // RFC de la escuela
    public string RFC_Receptor { get; set; } = null!; // RFC del tutor/alumno
    public string NombreReceptor { get; set; } = null!;
    public decimal Subtotal { get; set; }
    public decimal Descuento { get; set; } = 0;
    public decimal IVA { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = "Borrador"; // "Borrador", "Timbrada", "Cancelada", "Error"
    public string? RazonCancelacion { get; set; }
    
    // Almacenamiento de documentos
    public string? RutaXML { get; set; } // Blob Storage path
    public string? RutaPDF { get; set; } // Blob Storage path
    public string? XMLConTimbrado { get; set; } // XML firmado y timbrado (completo)
    
    // Complemento Educativo (IEDU 1.0)
    public string? NivelEducativo { get; set; } // "Preescolar", "Primaria", "Secundaria", etc.
    public string? CURP_Alumno { get; set; }
    public string? ClaveCT { get; set; } // Clave de Centro de Trabajo
    
    // Auditoría fiscal
    public string? CadenaOriginal { get; set; } // Para validación
    public DateTime FechaTimbrado { get; set; } = DateTime.UtcNow;
    public DateTime? FechaCancelacion { get; set; }
    public string? ErrorTimbrado { get; set; }
    public int ReintentosTimbrado { get; set; } = 0;
    
    public Cargo Cargo { get; set; } = null!;
    public ICollection<BitacoraFiscal> Bitacoras { get; set; } = [];
}

/// <summary>
/// Bitácora fiscal (auditoría de cambios, intentos de timbrado, etc.).
/// Requerimiento legal: trazabilidad completa de operaciones fiscales.
/// </summary>
public class BitacoraFiscal
{
    public int Id { get; set; }
    public int? CFDIId { get; set; }
    public int? CargoId { get; set; }
    public string Evento { get; set; } = null!; // "Creación", "Intento_Timbrado", "Timbrado_Exitoso", "Cancelación", "Error"
    public string Descripcion { get; set; } = null!;
    public string? DetalleError { get; set; }
    public string? UsuarioId { get; set; }
    public string? IPOrigen { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public CFDI? CFDI { get; set; }
    public Cargo? Cargo { get; set; }
}

/// <summary>
/// Configuración de CFDI/Timbrado (credenciales de PAC, URLs, etc.).
/// </summary>
public class ConfiguracionCFDI
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public string ProvedorTimbrado { get; set; } = null!; // "FINKOK", "QUADRUM", etc.
    public string UrlWSTimbrado { get; set; } = null!;
    public string UrlWSCancelacion { get; set; } = null!;
    public string Usuario_PAC { get; set; } = null!;
    public string Contrasena_PAC { get; set; } = null!; // Encriptada
    public string NombreComercial { get; set; } = null!; // Para PDF
    public string LogoUrlRuta { get; set; } = null!; // Blob path
    public int DiasCadenaOriginal { get; set; } = 30; // Días a mantener la cadena original en BD
    public int ReintentosMaximos { get; set; } = 3;
    public int TiempoEsperaReintentos { get; set; } = 300; // segundos
    public bool Activa { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaActualizacion { get; set; }

    public School School { get; set; } = null!;
}

/// <summary>
/// Complemento educativo (IEDU) - Datos específicos para facturación educativa.
/// </summary>
public class ComplementoEducativo
{
    public int Id { get; set; }
    public int CFDIId { get; set; }
    public string NivelEducativo { get; set; } = null!; // "Preescolar", "Primaria", "Secundaria", "Bachillerato", "Profesional", "Postgrado"
    public string CURP { get; set; } = null!;
    public string ClaveCT { get; set; } = null!; // Clave de Centro de Trabajo (SEP)
    public string? NoExamenGeneral { get; set; }
    public DateTime FechaExpedicion { get; set; }
    public bool ValidadoSAT { get; set; } = false;

    public CFDI CFDI { get; set; } = null!;
}
