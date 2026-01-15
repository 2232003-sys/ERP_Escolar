using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPEscolar.API.Infrastructure.Services;
using ERPEscolar.API.Core.Exceptions;
using ERPEscolar.API.DTOs.Finanzas;

namespace ERPEscolar.API.Features.Finanzas;

/// <summary>
/// Controller para gestión de cargos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CargosController : ControllerBase
{
    private readonly ICargoService _cargoService;
    private readonly ILogger<CargosController> _logger;

    public CargosController(ICargoService cargoService, ILogger<CargosController> logger)
    {
        _cargoService = cargoService;
        _logger = logger;
    }

    /// <summary>
    /// Obtener listado de cargos con paginación y búsqueda.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
    {
        try
        {
            var result = await _cargoService.GetAllAsync(pageNumber, pageSize, searchTerm);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cargos");
            return StatusCode(500, new { message = "Error al obtener cargos." });
        }
    }

    /// <summary>
    /// Obtener cargo por ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var cargo = await _cargoService.GetByIdAsync(id);
            return Ok(cargo);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cargo {Id}", id);
            return StatusCode(500, new { message = "Error al obtener el cargo." });
        }
    }

    /// <summary>
    /// Obtener cargo con datos completos (relaciones incluidas).
    /// </summary>
    [HttpGet("{id}/full")]
    public async Task<IActionResult> GetByIdFull(int id)
    {
        try
        {
            var cargo = await _cargoService.GetByIdFullAsync(id);
            return Ok(cargo);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cargo completo {Id}", id);
            return StatusCode(500, new { message = "Error al obtener el cargo completo." });
        }
    }

    /// <summary>
    /// Crear un nuevo cargo.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCargoDto request)
    {
        try
        {
            var cargo = await _cargoService.CreateCargoAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = cargo.Id }, cargo);
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
            _logger.LogError(ex, "Error al crear cargo");
            return StatusCode(500, new { message = "Error al crear el cargo." });
        }
    }

    /// <summary>
    /// Actualizar un cargo existente.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCargoDto request)
    {
        try
        {
            var cargo = await _cargoService.UpdateCargoAsync(id, request);
            return Ok(cargo);
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
            _logger.LogError(ex, "Error al actualizar cargo {Id}", id);
            return StatusCode(500, new { message = "Error al actualizar el cargo." });
        }
    }

    /// <summary>
    /// Eliminar un cargo (soft delete).
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _cargoService.SoftDeleteAsync(id);
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
            _logger.LogError(ex, "Error al eliminar cargo {Id}", id);
            return StatusCode(500, new { message = "Error al eliminar el cargo." });
        }
    }

    /// <summary>
    /// Restaurar un cargo eliminado.
    /// </summary>
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        try
        {
            await _cargoService.RestoreAsync(id);
            return Ok(new { message = "Cargo restaurado exitosamente." });
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
            _logger.LogError(ex, "Error al restaurar cargo {Id}", id);
            return StatusCode(500, new { message = "Error al restaurar el cargo." });
        }
    }

    /// <summary>
    /// Obtener cargos de un alumno específico.
    /// </summary>
    [HttpGet("alumno/{alumnoId}")]
    public async Task<IActionResult> GetByAlumno(int alumnoId)
    {
        try
        {
            var cargos = await _cargoService.GetCargosByAlumnoAsync(alumnoId);
            return Ok(cargos);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cargos del alumno {AlumnoId}", alumnoId);
            return StatusCode(500, new { message = "Error al obtener los cargos del alumno." });
        }
    }

    /// <summary>
    /// Obtener cargos pendientes de un alumno.
    /// </summary>
    [HttpGet("alumno/{alumnoId}/pendientes")]
    public async Task<IActionResult> GetPendientesByAlumno(int alumnoId)
    {
        try
        {
            var cargos = await _cargoService.GetCargosPendientesAsync(alumnoId);
            return Ok(cargos);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cargos pendientes del alumno {AlumnoId}", alumnoId);
            return StatusCode(500, new { message = "Error al obtener los cargos pendientes del alumno." });
        }
    }
}
