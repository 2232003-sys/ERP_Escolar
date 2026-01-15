using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPEscolar.API.Infrastructure.Services;
using ERPEscolar.API.Core.Exceptions;
using ERPEscolar.API.DTOs.Finanzas;

namespace ERPEscolar.API.Features.Finanzas;

/// <summary>
/// Controller para gestión de colegiaturas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ColegiaturasController : ControllerBase
{
    private readonly IColegiaturaService _colegiaturaService;
    private readonly ILogger<ColegiaturasController> _logger;

    public ColegiaturasController(IColegiaturaService colegiaturaService, ILogger<ColegiaturasController> logger)
    {
        _colegiaturaService = colegiaturaService;
        _logger = logger;
    }

    /// <summary>
    /// Obtener listado de colegiaturas con paginación y búsqueda.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
    {
        try
        {
            var result = await _colegiaturaService.GetAllAsync(pageNumber, pageSize, searchTerm);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener colegiaturas");
            return StatusCode(500, new { message = "Error al obtener colegiaturas." });
        }
    }

    /// <summary>
    /// Obtener colegiatura por ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var colegiatura = await _colegiaturaService.GetByIdAsync(id);
            return Ok(colegiatura);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener colegiatura {Id}", id);
            return StatusCode(500, new { message = "Error al obtener la colegiatura." });
        }
    }

    /// <summary>
    /// Obtener colegiatura con datos completos (relaciones incluidas).
    /// </summary>
    [HttpGet("{id}/full")]
    public async Task<IActionResult> GetByIdFull(int id)
    {
        try
        {
            var colegiatura = await _colegiaturaService.GetByIdFullAsync(id);
            return Ok(colegiatura);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener colegiatura completa {Id}", id);
            return StatusCode(500, new { message = "Error al obtener la colegiatura completa." });
        }
    }

    /// <summary>
    /// Crear una nueva colegiatura.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateColegiaturaDto request)
    {
        try
        {
            var colegiatura = await _colegiaturaService.CreateColegiaturaAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = colegiatura.Id }, colegiatura);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = "Datos de entrada inválidos", errors = ex.Errors });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear colegiatura");
            return StatusCode(500, new { message = "Error al crear la colegiatura." });
        }
    }

    /// <summary>
    /// Actualizar una colegiatura existente.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateColegiaturaDto request)
    {
        try
        {
            var colegiatura = await _colegiaturaService.UpdateColegiaturaAsync(id, request);
            return Ok(colegiatura);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = "Datos de entrada inválidos", errors = ex.Errors });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar colegiatura {Id}", id);
            return StatusCode(500, new { message = "Error al actualizar la colegiatura." });
        }
    }

    /// <summary>
    /// Eliminar una colegiatura (soft delete).
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _colegiaturaService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar colegiatura {Id}", id);
            return StatusCode(500, new { message = "Error al eliminar la colegiatura." });
        }
    }

    /// <summary>
    /// Restaurar una colegiatura eliminada.
    /// </summary>
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        try
        {
            await _colegiaturaService.RestoreAsync(id);
            return Ok(new { message = "Colegiatura restaurada exitosamente." });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al restaurar colegiatura {Id}", id);
            return StatusCode(500, new { message = "Error al restaurar la colegiatura." });
        }
    }

    /// <summary>
    /// Obtener colegiaturas de un alumno específico.
    /// </summary>
    [HttpGet("alumno/{alumnoId}")]
    public async Task<IActionResult> GetByAlumno(int alumnoId)
    {
        try
        {
            var colegiaturas = await _colegiaturaService.GetColegiaturasByAlumnoAsync(alumnoId);
            return Ok(colegiaturas);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener colegiaturas del alumno {AlumnoId}", alumnoId);
            return StatusCode(500, new { message = "Error al obtener las colegiaturas del alumno." });
        }
    }

    /// <summary>
    /// Obtener colegiaturas pendientes de un alumno.
    /// </summary>
    [HttpGet("alumno/{alumnoId}/pendientes")]
    public async Task<IActionResult> GetPendientesByAlumno(int alumnoId)
    {
        try
        {
            var colegiaturas = await _colegiaturaService.GetColegiaturasPendientesAsync(alumnoId);
            return Ok(colegiaturas);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener colegiaturas pendientes del alumno {AlumnoId}", alumnoId);
            return StatusCode(500, new { message = "Error al obtener las colegiaturas pendientes del alumno." });
        }
    }
}