using ERPEscolar.API.DTOs.ControlEscolar;
using ERPEscolar.API.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPEscolar.API.Features.ControlEscolar;

/// <summary>
/// Controller para gestión de calificaciones
/// </summary>
[ApiController]
[Route("api/control-escolar/[controller]")]
[Authorize]
public class CalificacionesController : ControllerBase
{
    private readonly ICalificacionService _calificacionService;
    private readonly ILogger<CalificacionesController> _logger;

    public CalificacionesController(
        ICalificacionService calificacionService,
        ILogger<CalificacionesController> logger)
    {
        _calificacionService = calificacionService;
        _logger = logger;
    }

    /// <summary>
    /// Registra una nueva calificación
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CalificacionDto>> CreateCalificacion([FromBody] CreateCalificacionDto request)
    {
        try
        {
            _logger.LogInformation("Registrando nueva calificación para alumno {AlumnoId}", request.AlumnoId);
            var result = await _calificacionService.RegisterCalificacionAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar calificación");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza una calificación existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<CalificacionDto>> UpdateCalificacion(int id, [FromBody] UpdateCalificacionDto request)
    {
        try
        {
            _logger.LogInformation("Actualizando calificación {Id}", id);
            var result = await _calificacionService.UpdateCalificacionAsync(id, request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar calificación {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene una calificación por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CalificacionDto>> GetById(int id)
    {
        try
        {
            var result = await _calificacionService.GetByIdAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener calificación {Id}", id);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lista calificaciones con paginación y filtros
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaginatedCalificacionesDto>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? alumnoId = null,
        [FromQuery] int? grupoMateriaId = null,
        [FromQuery] int? periodoId = null)
    {
        try
        {
            var result = await _calificacionService.GetAllAsync(pageNumber, pageSize, alumnoId, grupoMateriaId, periodoId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar calificaciones");
            return BadRequest(new { message = ex.Message });
        }
    }
}