using System.ComponentModel.DataAnnotations;

namespace ERPEscolar.API.DTOs.Finanzas;

/// <summary>
/// DTO básico de Alumno para relaciones en Finanzas
/// </summary>
public class AlumnoDto
{
    /// <summary>
    /// Identificador único del alumno
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre completo del alumno
    /// </summary>
    public string NombreCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Matrícula del alumno
    /// </summary>
    public string Matricula { get; set; } = string.Empty;

    /// <summary>
    /// CURP del alumno
    /// </summary>
    public string CURP { get; set; } = string.Empty;
}

/// <summary>
/// DTO básico de ConceptoCobro para relaciones en Finanzas
/// </summary>
public class ConceptoCobroDto
{
    /// <summary>
    /// Identificador único del concepto
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del concepto
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del concepto
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de concepto
    /// </summary>
    public string TipoConcepto { get; set; } = string.Empty;

    /// <summary>
    /// Monto base del concepto
    /// </summary>
    public decimal MontoBase { get; set; }
}

/// <summary>
/// DTO básico de CicloEscolar para relaciones en Finanzas
/// </summary>
public class CicloEscolarDto
{
    /// <summary>
    /// Identificador único del ciclo escolar
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del ciclo escolar
    /// </summary>
    public string Nombre { get; set; } = string.Empty;
}

/// <summary>
/// DTO básico de Pago para relaciones en Finanzas
/// </summary>
public class PagoDto
{
    /// <summary>
    /// Identificador único del pago
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Monto del pago
    /// </summary>
    public decimal Monto { get; set; }

    /// <summary>
    /// Método de pago
    /// </summary>
    public string Metodo { get; set; } = string.Empty;

    /// <summary>
    /// Estado del pago
    /// </summary>
    public string Estado { get; set; } = string.Empty;

    /// <summary>
    /// Fecha del pago
    /// </summary>
    public DateTime FechaPago { get; set; }

    /// <summary>
    /// Referencia externa del pago
    /// </summary>
    public string? ReferenciaExterna { get; set; }
}
