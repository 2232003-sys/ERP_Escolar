using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPEscolar.API.Infrastructure.Services;
using ERPEscolar.API.Core.Exceptions;
using ERPEscolar.API.DTOs.ControlEscolar;

namespace ERPEscolar.API.Features.ControlEscolar;

/// <summary>
/// Controller para gestión de alumnos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlumnosController : ControllerBase
{
    private readonly IAlumnoService _alumnoService;
    private readonly ILogger<AlumnosController> _logger;

    public AlumnosController(IAlumnoService alumnoService, ILogger<AlumnosController> logger)
    {
        _alumnoService = alumnoService;
        _logger = logger;
    }

    /// <summary>
    /// Obtener listado de alumnos con paginación y búsqueda.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
    {
        try
        {
            var result = await _alumnoService.GetAllAsync(pageNumber, pageSize, searchTerm);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener alumnos");
            return StatusCode(500, new { message = "Error al obtener alumnos." });
        }
    }

    /// <summary>
    /// Obtener alumno por ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var alumno = await _alumnoService.GetByIdAsync(id);
            return Ok(alumno);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener alumno {id}");
            return StatusCode(500, new { message = "Error al obtener alumno." });
        }
    }

    /// <summary>
    /// Obtener alumno con datos completos (tutores e inscripciones).
    /// </summary>
    [HttpGet("{id}/completo")]
    public async Task<IActionResult> GetByIdFull(int id)
    {
        try
        {
            var alumno = await _alumnoService.GetByIdFullAsync(id);
            return Ok(alumno);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener alumno completo {id}");
            return StatusCode(500, new { message = "Error al obtener alumno." });
        }
    }

    /// <summary>
    /// Crear un nuevo alumno.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin TI,Control Escolar")]
    public async Task<IActionResult> Create([FromBody] CreateAlumnoDto request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var alumno = await _alumnoService.CreateAlumnoAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = alumno.Id }, alumno);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message, errors = ex.Errors });
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Error de negocio al crear alumno");
            return Conflict(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear alumno");
            return StatusCode(500, new { message = "Error al crear alumno." });
        }
    }

    /// <summary>
    /// Actualizar datos de alumno.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin TI,Control Escolar")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAlumnoDto request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var alumno = await _alumnoService.UpdateAlumnoAsync(id, request);
            return Ok(alumno);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message, errors = ex.Errors });
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Error de negocio al actualizar alumno");
            return Conflict(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar alumno {id}");
            return StatusCode(500, new { message = "Error al actualizar alumno." });
        }
    }

    /// <summary>
    /// Desactivar alumno (soft delete).
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin TI,Control Escolar")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _alumnoService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al desactivar alumno {id}");
            return StatusCode(500, new { message = "Error al desactivar alumno." });
        }
    }

    /// <summary>
    /// Restaurar alumno desactivado.
    /// </summary>
    [HttpPatch("{id}/restore")]
    [Authorize(Roles = "SuperAdmin,Admin TI,Control Escolar")]
    public async Task<IActionResult> Restore(int id)
    {
        try
        {
            await _alumnoService.RestoreAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al restaurar alumno {id}");
            return StatusCode(500, new { message = "Error al restaurar alumno." });
        }
    }
}
