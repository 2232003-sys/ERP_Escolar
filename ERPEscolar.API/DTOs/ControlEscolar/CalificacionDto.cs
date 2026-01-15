using System.ComponentModel.DataAnnotations;

namespace ERPEscolar.API.DTOs.ControlEscolar;

/// <summary>
/// DTO para respuesta de calificación
/// </summary>
public class CalificacionDto
{
    public int Id { get; set; }
    public int AlumnoId { get; set; }
    public int GrupoMateriaId { get; set; }
    public int PeriodoCalificacionId { get; set; }
    public decimal Calificacion1 { get; set; }
    public string? Observacion { get; set; }
    public bool Aprobado { get; set; }
    public DateTime FechaCalificacion { get; set; }
    public int? DocenteQueQualificaId { get; set; }

    // Datos relacionados
    public string AlumnoNombre { get; set; } = null!;
    public string MateriaNombre { get; set; } = null!;
    public string PeriodoNombre { get; set; } = null!;
    public string? DocenteNombre { get; set; }
}

/// <summary>
/// DTO para crear calificación
/// </summary>
public class CreateCalificacionDto
{
    [Required]
    public int AlumnoId { get; set; }

    [Required]
    public int GrupoMateriaId { get; set; }

    [Required]
    public int PeriodoCalificacionId { get; set; }

    [Required]
    [Range(0, 10)]
    public decimal Calificacion1 { get; set; }

    public string? Observacion { get; set; }

    public int? DocenteQueQualificaId { get; set; }
}

/// <summary>
/// DTO para actualizar calificación
/// </summary>
public class UpdateCalificacionDto
{
    [Required]
    [Range(0, 10)]
    public decimal Calificacion1 { get; set; }

    public string? Observacion { get; set; }
}

/// <summary>
/// DTO para calificaciones de un grupo
/// </summary>
public class GrupoCalificacionesDto
{
    public int GrupoMateriaId { get; set; }
    public string GrupoNombre { get; set; } = null!;
    public string MateriaNombre { get; set; } = null!;
    public string PeriodoNombre { get; set; } = null!;
    public List<CalificacionDto> Calificaciones { get; set; } = new();
}

/// <summary>
/// DTO para expediente académico de alumno
/// </summary>
public class ExpedienteAcademicoDto
{
    public int AlumnoId { get; set; }
    public string AlumnoNombre { get; set; } = null!;
    public string Matricula { get; set; } = null!;
    public List<MateriaCalificacionesDto> Materias { get; set; } = new();
    public decimal PromedioGeneral { get; set; }
}

/// <summary>
/// DTO para calificaciones de una materia específica
/// </summary>
public class MateriaCalificacionesDto
{
    public int MateriaId { get; set; }
    public string MateriaNombre { get; set; } = null!;
    public List<CalificacionDto> Calificaciones { get; set; } = new();
    public decimal PromedioMateria { get; set; }
}

/// <summary>
/// DTO para boleta por período
/// </summary>
public class BoletaDto
{
    public int AlumnoId { get; set; }
    public string AlumnoNombre { get; set; } = null!;
    public string Matricula { get; set; } = null!;
    public string PeriodoNombre { get; set; } = null!;
    public DateTime FechaGeneracion { get; set; }
    public List<BoletaMateriaDto> Materias { get; set; } = new();
    public decimal PromedioPeriodo { get; set; }
    public bool Aprobado { get; set; }
}

/// <summary>
/// DTO para materia en boleta
/// </summary>
public class BoletaMateriaDto
{
    public string MateriaNombre { get; set; } = null!;
    public decimal Calificacion { get; set; }
    public bool Aprobado { get; set; }
    public string? Observacion { get; set; }
}

/// <summary>
/// DTO para paginación de calificaciones
/// </summary>
public class PaginatedCalificacionesDto
{
    public List<CalificacionDto> Items { get; set; } = new();
    public int TotalItems { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}