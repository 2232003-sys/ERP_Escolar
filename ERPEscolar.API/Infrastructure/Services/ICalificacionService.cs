using ERPEscolar.API.DTOs.ControlEscolar;

namespace ERPEscolar.API.Infrastructure.Services;

/// <summary>
/// Interfaz para el servicio de calificaciones
/// </summary>
public interface ICalificacionService
{
    /// <summary>
    /// Registra una nueva calificación
    /// </summary>
    Task<CalificacionDto> RegisterCalificacionAsync(CreateCalificacionDto request);

    /// <summary>
    /// Actualiza una calificación existente
    /// </summary>
    Task<CalificacionDto> UpdateCalificacionAsync(int id, UpdateCalificacionDto request);

    /// <summary>
    /// Obtiene una calificación por ID
    /// </summary>
    Task<CalificacionDto> GetByIdAsync(int id);

    /// <summary>
    /// Obtiene calificaciones de un grupo-materia
    /// </summary>
    Task<GrupoCalificacionesDto> GetCalificacionesByGrupoAsync(int grupoMateriaId, int periodoCalificacionId);

    /// <summary>
    /// Obtiene expediente académico de un alumno
    /// </summary>
    Task<ExpedienteAcademicoDto> GetExpedienteAcademicoAsync(int alumnoId);

    /// <summary>
    /// Genera boleta de calificaciones para un alumno y período
    /// </summary>
    Task<BoletaDto> GenerateBoletaAsync(int alumnoId, int periodoCalificacionId);

    /// <summary>
    /// Calcula promedio de una materia para un alumno
    /// </summary>
    Task<decimal> CalculatePromedioMateriaAsync(int alumnoId, int materiaId);

    /// <summary>
    /// Calcula promedio final de un alumno
    /// </summary>
    Task<decimal> CalculatePromedioFinalAsync(int alumnoId);

    /// <summary>
    /// Valida si se puede cerrar un período de calificaciones
    /// </summary>
    Task<bool> ValidarCierrePeriodoAsync(int periodoCalificacionId);

    /// <summary>
    /// Lista calificaciones con paginación y filtros
    /// </summary>
    Task<PaginatedCalificacionesDto> GetAllAsync(int pageNumber = 1, int pageSize = 10, int? alumnoId = null, int? grupoMateriaId = null, int? periodoId = null);
}