using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERPEscolar.API.DTOs.ControlEscolar;
using ERPEscolar.API.Infrastructure.Services;
using ERPEscolar.API.Core.Exceptions;

namespace ERPEscolar.API.Features.ControlEscolar;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AsistenciasController : ControllerBase
{
    private readonly IAsistenciaService _asistenciaService;
    private readonly ILogger<AsistenciasController> _logger;

    public AsistenciasController(IAsistenciaService asistenciaService, ILogger<AsistenciasController> logger)
    {
        _asistenciaService = asistenciaService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _asistenciaService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(500, "Error");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _asistenciaService.GetByIdAsync(id);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(500, "Error");
        }
    }

    [HttpGet("{id}/completo")]
    public async Task<IActionResult> GetByIdFull(int id)
    {
        try
        {
            var result = await _asistenciaService.GetByIdFullAsync(id);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(500, "Error");
        }
    }

    [HttpGet("inscripcion/{inscripcionId}")]
    public async Task<IActionResult> GetByInscripcion(int inscripcionId)
    {
        try
        {
            var result = await _asistenciaService.GetByInscripcionAsync(inscripcionId);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(500, "Error");
        }
    }

    [HttpGet("alumno/{alumnoId}")]
    public async Task<IActionResult> GetByAlumno(int alumnoId)
    {
        try
        {
            var result = await _asistenciaService.GetByAlumnoAsync(alumnoId);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(500, "Error");
        }
    }

    [HttpGet("grupo/{grupoId}/fecha")]
    public async Task<IActionResult> GetPorGrupoEnFecha(int grupoId, [FromQuery] DateTime fecha)
    {
        try
        {
            var result = await _asistenciaService.GetPorGrupoEnFechaAsync(grupoId, fecha);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(500, "Error");
        }
    }

    [HttpGet("grupo/{grupoId}/rango")]
    public async Task<IActionResult> GetPorGrupoEnRango(int grupoId, [FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
    {
        try
        {
            var result = await _asistenciaService.GetPorGrupoEnRangoAsync(grupoId, fechaInicio, fechaFin);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(500, "Error");
        }
    }

    [HttpGet("inscripcion/{inscripcionId}/porcentaje")]
    public async Task<IActionResult> GetAsistenciaPercentage(int inscripcionId)
    {
        try
        {
            var result = await _asistenciaService.GetAsistenciaPercentageAsync(inscripcionId);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(500, "Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAsistenciaDto request)
    {
        try
        {
            var result = await _asistenciaService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Core.Exceptions.ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
        catch (BusinessException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(500, "Error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAsistenciaDto request)
    {
        try
        {
            var result = await _asistenciaService.UpdateAsync(id, request);
            return Ok(result);
        }
        catch (Core.Exceptions.ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(500, "Error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _asistenciaService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(500, "Error");
        }
    }

    [HttpPatch("{id}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        try
        {
            await _asistenciaService.RestoreAsync(id);
            return Ok("Asistencia restaurada");
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(500, "Error");
        }
    }
}
