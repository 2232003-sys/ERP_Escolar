namespace ERPEscolar.API.DTOs.ControlEscolar;

/// <summary>
/// DTO para obtener datos de inscripción (GET).
/// </summary>
public class InscripcionDto
{
    public int Id { get; set; }
    public int AlumnoId { get; set; }
    public int GrupoId { get; set; }
    public int CicloEscolarId { get; set; }
    public DateTime FechaInscripcion { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
}

/// <summary>
/// DTO para crear inscripción (POST).
/// </summary>
public class CreateInscripcionDto
{
    /// <summary>
    /// ID del alumno a inscribir.
    /// </summary>
    public int AlumnoId { get; set; }

    /// <summary>
    /// ID del grupo en el que se inscribe.
    /// </summary>
    public int GrupoId { get; set; }

    /// <summary>
    /// Ciclo escolar (debe coincidir con el del grupo).
    /// </summary>
    public int CicloEscolarId { get; set; }

    /// <summary>
    /// Fecha de inscripción (opcional, default ahora).
    /// </summary>
    public DateTime? FechaInscripcion { get; set; }
}

/// <summary>
/// DTO para actualizar inscripción (PUT).
/// </summary>
public class UpdateInscripcionDto
{
    /// <summary>
    /// Nuevo grupo (cambiar de grupo en mismo ciclo).
    /// </summary>
    public int? GrupoId { get; set; }

    /// <summary>
    /// Nueva fecha de inscripción.
    /// </summary>
    public DateTime? FechaInscripcion { get; set; }
}

/// <summary>
/// DTO para inscripción con datos completos.
/// </summary>
public class InscripcionFullDataDto : InscripcionDto
{
    /// <summary>
    /// Nombre completo del alumno.
    /// </summary>
    public string AlumnoNombre { get; set; } = null!;

    /// <summary>
    /// Matrícula del alumno.
    /// </summary>
    public string? AlumnoMatricula { get; set; }

    /// <summary>
    /// Nombre del grupo.
    /// </summary>
    public string GrupoNombre { get; set; } = null!;

    /// <summary>
    /// Grado del grupo.
    /// </summary>
    public string GrupoGrado { get; set; } = null!;

    /// <summary>
    /// Nombre del ciclo escolar.
    /// </summary>
    public string CicloNombre { get; set; } = null!;

    /// <summary>
    /// Cantidad de materias en el grupo.
    /// </summary>
    public int TotalMaterias { get; set; }

    /// <summary>
    /// Cantidad de asistencias registradas.
    /// </summary>
    public int TotalAsistencias { get; set; }

    /// <summary>
    /// Cantidad de calificaciones registradas.
    /// </summary>
    public int TotalCalificaciones { get; set; }
}

/// <summary>
/// DTO para paginación de resultados de inscripciones.
/// </summary>
public class PaginatedInscripcionesDto
{
    /// <summary>
    /// Lista de inscripciones.
    /// </summary>
    public List<InscripcionDto> Items { get; set; } = [];

    /// <summary>
    /// Total de registros en BD (sin paginación).
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Número de página actual.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Cantidad de items por página.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de páginas.
    /// </summary>
    public int TotalPages => (TotalItems + PageSize - 1) / PageSize;

    /// <summary>
    /// Si hay página siguiente.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Si hay página anterior.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
}
