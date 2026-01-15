using System.ComponentModel.DataAnnotations;

namespace ERPEscolar.API.DTOs.Fiscal;

/// <summary>
/// DTO básico de BitacoraFiscal
/// </summary>
public class BitacoraFiscalDto
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID del CFDI relacionado
    /// </summary>
    public int? CFDIId { get; set; }

    /// <summary>
    /// ID del cargo relacionado
    /// </summary>
    public int? CargoId { get; set; }

    /// <summary>
    /// Tipo de evento
    /// </summary>
    public string Evento { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del evento
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Detalle del error (si aplica)
    /// </summary>
    public string? DetalleError { get; set; }

    /// <summary>
    /// ID del usuario que realizó la acción
    /// </summary>
    public string? UsuarioId { get; set; }

    /// <summary>
    /// IP de origen
    /// </summary>
    public string? IPOrigen { get; set; }

    /// <summary>
    /// Timestamp del evento
    /// </summary>
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// DTO básico de ConfiguracionCFDI
/// </summary>
public class ConfiguracionCFDIDto
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID de la escuela
    /// </summary>
    public int SchoolId { get; set; }

    /// <summary>
    /// Proveedor de timbrado
    /// </summary>
    public string ProvedorTimbrado { get; set; } = string.Empty;

    /// <summary>
    /// URL del servicio web de timbrado
    /// </summary>
    public string UrlWSTimbrado { get; set; } = string.Empty;

    /// <summary>
    /// URL del servicio web de cancelación
    /// </summary>
    public string UrlWSCancelacion { get; set; } = string.Empty;

    /// <summary>
    /// Usuario del PAC
    /// </summary>
    public string Usuario_PAC { get; set; } = string.Empty;

    /// <summary>
    /// Nombre comercial para PDFs
    /// </summary>
    public string NombreComercial { get; set; } = string.Empty;

    /// <summary>
    /// Días para mantener cadena original
    /// </summary>
    public int DiasCadenaOriginal { get; set; }

    /// <summary>
    /// Reintentos máximos
    /// </summary>
    public int ReintentosMaximos { get; set; }

    /// <summary>
    /// Tiempo de espera entre reintentos (segundos)
    /// </summary>
    public int TiempoEsperaReintentos { get; set; }

    /// <summary>
    /// ¿Está activa la configuración?
    /// </summary>
    public bool Activa { get; set; }
}

/// <summary>
/// DTO básico de ComplementoEducativo
/// </summary>
public class ComplementoEducativoDto
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID del CFDI relacionado
    /// </summary>
    public int CFDIId { get; set; }

    /// <summary>
    /// Nivel educativo
    /// </summary>
    public string NivelEducativo { get; set; } = string.Empty;

    /// <summary>
    /// CURP del alumno
    /// </summary>
    public string CURP { get; set; } = string.Empty;

    /// <summary>
    /// Clave de Centro de Trabajo
    /// </summary>
    public string ClaveCT { get; set; } = string.Empty;

    /// <summary>
    /// Número de examen general
    /// </summary>
    public string? NoExamenGeneral { get; set; }

    /// <summary>
    /// Fecha de expedición
    /// </summary>
    public DateTime FechaExpedicion { get; set; }
}

/// <summary>
/// DTO para crear timbrado de CFDI
/// </summary>
public class TimbrarCFDIDto
{
    /// <summary>
    /// ID del CFDI a timbrar
    /// </summary>
    [Required(ErrorMessage = "El ID del CFDI es requerido")]
    public int CFDIId { get; set; }

    /// <summary>
    /// Forzar timbrado (ignorar validaciones previas)
    /// </summary>
    public bool ForzarTimbrado { get; set; } = false;
}

/// <summary>
/// DTO para cancelar CFDI
/// </summary>
public class CancelarCFDIDto
{
    /// <summary>
    /// ID del CFDI a cancelar
    /// </summary>
    [Required(ErrorMessage = "El ID del CFDI es requerido")]
    public int CFDIId { get; set; }

    /// <summary>
    /// Razón de cancelación
    /// </summary>
    [Required(ErrorMessage = "La razón de cancelación es requerida")]
    [StringLength(500, ErrorMessage = "La razón no puede exceder 500 caracteres")]
    public string RazonCancelacion { get; set; } = string.Empty;
}

/// <summary>
/// DTO de respuesta para operaciones de timbrado
/// </summary>
public class TimbradoResponseDto
{
    /// <summary>
    /// ¿Fue exitoso el timbrado?
    /// </summary>
    public bool Exitoso { get; set; }

    /// <summary>
    /// UUID generado (si exitoso)
    /// </summary>
    public string? UUID { get; set; }

    /// <summary>
    /// Mensaje de resultado
    /// </summary>
    public string Mensaje { get; set; } = string.Empty;

    /// <summary>
    /// Detalle del error (si aplica)
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Fecha de timbrado
    /// </summary>
    public DateTime? FechaTimbrado { get; set; }
}
