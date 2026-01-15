using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPEscolar.API.Core.Exceptions;
using ERPEscolar.API.Infrastructure.Services;
using ERPEscolar.API.DTOs.ControlEscolar;

namespace ERPEscolar.API.Features.ControlEscolar;

/// <summary>
/// Controller para gestión de inscripciones (matriculación de alumnos).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InscripcionesController : ControllerBase
{
    private readonly IInscripcionService _inscripcionService;
    private readonly ILogger<InscripcionesController> _logger;

    public InscripcionesController(IInscripcionService inscripcionService, ILogger<InscripcionesController> logger)
    {
        _inscripcionService = inscripcionService;
        _logger = logger;
    }

    /// <summary>
    /// Obtener listado de inscripciones con paginación y búsqueda.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
    {
        try
        {
            var result = await _inscripcionService.GetAllAsync(pageNumber, pageSize, searchTerm);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener inscripciones");
            return StatusCode(500, new { message = "Error al obtener inscripciones." });
        }
    }

    /// <summary>
    /// Obtener inscripción específica por ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _inscripcionService.GetByIdAsync(id);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Inscripción no encontrada: {InscripcionId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener inscripción {InscripcionId}", id);
            return StatusCode(500, new { message = "Error al obtener inscripción." });
        }
    }

    /// <summary>
    /// Obtener inscripción con datos completos (alumno, grupo, ciclo, etc.).
    /// </summary>
    [HttpGet("{id}/completo")]
    public async Task<IActionResult> GetByIdFull(int id)
    {
        try
        {
            var result = await _inscripcionService.GetByIdFullAsync(id);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Inscripción no encontrada: {InscripcionId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener inscripción completa {InscripcionId}", id);
            return StatusCode(500, new { message = "Error al obtener inscripción." });
        }
    }

    /// <summary>
    /// Obtener inscripciones de un alumno específico.
    /// </summary>
    [HttpGet("alumno/{alumnoId}")]
    public async Task<IActionResult> GetByAlumno(int alumnoId)
    {
        try
        {
            var result = await _inscripcionService.GetByAlumnoAsync(alumnoId);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Alumno no encontrado: {AlumnoId}", alumnoId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener inscripciones del alumno {AlumnoId}", alumnoId);
            return StatusCode(500, new { message = "Error al obtener inscripciones." });
        }
    }

    /// <summary>
    /// Obtener inscripciones de un grupo específico.
    /// </summary>
    [HttpGet("grupo/{grupoId}")]
    public async Task<IActionResult> GetByGrupo(int grupoId)
    {
        try
        {
            var result = await _inscripcionService.GetByGrupoAsync(grupoId);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Grupo no encontrado: {GrupoId}", grupoId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener inscripciones del grupo {GrupoId}", grupoId);
            return StatusCode(500, new { message = "Error al obtener inscripciones." });
        }
    }

    /// <summary>
    /// Crear nueva inscripción (matricular alumno en grupo).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin TI,Control Escolar")]
    public async Task<IActionResult> Create([FromBody] CreateInscripcionDto request)
    {
        try
        {
            var result = await _inscripcionService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Core.Exceptions.ValidationException ex)
        {
            _logger.LogWarning("Error de validación al crear inscripción");
            return BadRequest(new { message = "Validación fallida", errors = ex.Errors });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Recurso no encontrado al crear inscripción");
            return NotFound(new { message = ex.Message });
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Error de negocio al crear inscripción");
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear inscripción");
            return StatusCode(500, new { message = "Error al crear inscripción." });
        }
    }

    /// <summary>
    /// Actualizar inscripción (cambiar grupo, fecha).
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin TI,Control Escolar")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateInscripcionDto request)
    {
        try
        {
            var result = await _inscripcionService.UpdateAsync(id, request);
            return Ok(result);
        }
        catch (Core.Exceptions.ValidationException ex)
        {
            _logger.LogWarning("Error de validación al actualizar inscripción {InscripcionId}", id);
            return BadRequest(new { message = "Validación fallida", errors = ex.Errors });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Inscripción no encontrada: {InscripcionId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Error de negocio al actualizar inscripción {InscripcionId}", id);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar inscripción {InscripcionId}", id);
            return StatusCode(500, new { message = "Error al actualizar inscripción." });
        }
    }

    /// <summary>
    /// Desactivar inscripción (soft delete / desmatricular).
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin TI,Control Escolar")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _inscripcionService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Inscripción no encontrada: {InscripcionId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Error de negocio al desactivar inscripción {InscripcionId}", id);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar inscripción {InscripcionId}", id);
            return StatusCode(500, new { message = "Error al desactivar inscripción." });
        }
    }

    /// <summary>
    /// Reactivar inscripción desactivada.
    /// </summary>
    [HttpPatch("{id}/restore")]
    [Authorize(Roles = "SuperAdmin,Admin TI,Control Escolar")]
    public async Task<IActionResult> Restore(int id)
    {
        try
        {
            await _inscripcionService.RestoreAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Inscripción no encontrada: {InscripcionId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Error de negocio al reactivar inscripción {InscripcionId}", id);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reactivar inscripción {InscripcionId}", id);
            return StatusCode(500, new { message = "Error al reactivar inscripción." });
        }
    }
}
