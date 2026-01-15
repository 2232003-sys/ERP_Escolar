using System.ComponentModel.DataAnnotations;

namespace ERPEscolar.API.DTOs.ControlEscolar;

/// <summary>
/// DTO básico de Asistencia - Respuesta GET
/// </summary>
public class AsistenciaDto
{
    /// <summary>
    /// Identificador único de la asistencia
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador de la inscripción del alumno
    /// </summary>
    public int InscripcionId { get; set; }

    /// <summary>
    /// Identificador del grupo/materia que se impartía
    /// </summary>
    public int GrupoMateriaId { get; set; }

    /// <summary>
    /// Fecha de la clase
    /// </summary>
    public DateTime Fecha { get; set; }

    /// <summary>
    /// Estado de asistencia: Presente, Ausente, Justificado, Retraso
    /// </summary>
    public string Estado { get; set; } = string.Empty;

    /// <summary>
    /// Observaciones o motivo de la ausencia
    /// </summary>
    public string? Observaciones { get; set; }

    /// <summary>
    /// Quién registró la asistencia (nombre del docente)
    /// </summary>
    public string? RegistradoPor { get; set; }

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    public DateTime FechaCreacion { get; set; }
}

/// <summary>
/// DTO para crear una nueva asistencia - POST
/// </summary>
public class CreateAsistenciaDto
{
    /// <summary>
    /// Identificador de la inscripción del alumno
    /// </summary>
    [Required(ErrorMessage = "La inscripción es requerida")]
    public int InscripcionId { get; set; }

    /// <summary>
    /// Identificador del grupo/materia que se impartía
    /// </summary>
    [Required(ErrorMessage = "La materia es requerida")]
    public int GrupoMateriaId { get; set; }

    /// <summary>
    /// Fecha de la clase (no puede ser futura)
    /// </summary>
    [Required(ErrorMessage = "La fecha es requerida")]
    public DateTime Fecha { get; set; }

    /// <summary>
    /// Estado: Presente | Ausente | Justificado | Retraso
    /// </summary>
    [Required(ErrorMessage = "El estado de asistencia es requerido")]
    [StringLength(20, ErrorMessage = "El estado debe tener máximo 20 caracteres")]
    public string Estado { get; set; } = string.Empty;

    /// <summary>
    /// Observaciones (opcional)
    /// </summary>
    [StringLength(500, ErrorMessage = "Las observaciones deben tener máximo 500 caracteres")]
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO para actualizar una asistencia - PUT
/// </summary>
public class UpdateAsistenciaDto
{
    /// <summary>
    /// Estado actualizado
    /// </summary>
    [Required(ErrorMessage = "El estado de asistencia es requerido")]
    [StringLength(20, ErrorMessage = "El estado debe tener máximo 20 caracteres")]
    public string Estado { get; set; } = string.Empty;

    /// <summary>
    /// Observaciones actualizadas
    /// </summary>
    [StringLength(500, ErrorMessage = "Las observaciones deben tener máximo 500 caracteres")]
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO de Asistencia con datos completos (relaciones incluidas)
/// </summary>
public class AsistenciaFullDataDto
{
    /// <summary>
    /// Identificador de la asistencia
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del alumno
    /// </summary>
    public string? AlumnoNombre { get; set; }

    /// <summary>
    /// Materia/Asignatura
    /// </summary>
    public string? MateriaNombre { get; set; }

    /// <summary>
    /// Fecha de la clase
    /// </summary>
    public DateTime Fecha { get; set; }

    /// <summary>
    /// Estado de asistencia
    /// </summary>
    public string Estado { get; set; } = string.Empty;

    /// <summary>
    /// Observaciones
    /// </summary>
    public string? Observaciones { get; set; }

    /// <summary>
    /// Quién registró (docente)
    /// </summary>
    public string? RegistradoPor { get; set; }

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    public DateTime FechaCreacion { get; set; }
}

/// <summary>
/// DTO paginado de Asistencias
/// </summary>
public class PaginatedAsistenciasDto
{
    /// <summary>
    /// Lista de asistencias
    /// </summary>
    public List<AsistenciaDto> Items { get; set; } = [];

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
