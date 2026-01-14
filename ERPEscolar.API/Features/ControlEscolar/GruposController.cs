using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPEscolar.API.Infrastructure.Services;
using ERPEscolar.API.DTOs.ControlEscolar;
using FluentValidation;

namespace ERPEscolar.API.Features.ControlEscolar;

/// <summary>
/// Controller para gestión de grupos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GruposController : ControllerBase
{
    private readonly IGrupoService _grupoService;
    private readonly ILogger<GruposController> _logger;

    public GruposController(IGrupoService grupoService, ILogger<GruposController> logger)
    {
        _grupoService = grupoService;
        _logger = logger;
    }

    /// <summary>
    /// Obtener listado de grupos con paginación y búsqueda.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
    {
        try
        {
            var result = await _grupoService.GetAllAsync(pageNumber, pageSize, searchTerm);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener grupos");
            return StatusCode(500, new { message = "Error al obtener grupos." });
        }
    }

    /// <summary>
    /// Obtener grupo por ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var grupo = await _grupoService.GetByIdAsync(id);
            return Ok(grupo);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener grupo {id}");
            return StatusCode(500, new { message = "Error al obtener grupo." });
        }
    }

    /// <summary>
    /// Obtener grupo con datos completos (ciclo escolar, docente tutor, inscripciones).
    /// </summary>
    [HttpGet("{id}/completo")]
    public async Task<IActionResult> GetByIdFull(int id)
    {
        try
        {
            var grupo = await _grupoService.GetByIdFullAsync(id);
            return Ok(grupo);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener grupo completo {id}");
            return StatusCode(500, new { message = "Error al obtener grupo." });
        }
    }

    /// <summary>
    /// Crear un nuevo grupo.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin TI,Control Escolar")]
    public async Task<IActionResult> Create([FromBody] CreateGrupoDto request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var grupo = await _grupoService.CreateGrupoAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = grupo.Id }, grupo);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            return BadRequest(new { message = "Validación fallida", errors });
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Error de negocio al crear grupo");
            return Conflict(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear grupo");
            return StatusCode(500, new { message = "Error al crear grupo." });
        }
    }

    /// <summary>
    /// Actualizar datos de grupo.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin TI,Control Escolar")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateGrupoDto request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var grupo = await _grupoService.UpdateGrupoAsync(id, request);
            return Ok(grupo);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            return BadRequest(new { message = "Validación fallida", errors });
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Error de negocio al actualizar grupo");
            return Conflict(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar grupo {id}");
            return StatusCode(500, new { message = "Error al actualizar grupo." });
        }
    }

    /// <summary>
    /// Desactivar grupo (soft delete).
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin TI,Control Escolar")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _grupoService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (BusinessException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al desactivar grupo {id}");
            return StatusCode(500, new { message = "Error al desactivar grupo." });
        }
    }

    /// <summary>
    /// Restaurar grupo desactivado.
    /// </summary>
    [HttpPatch("{id}/restore")]
    [Authorize(Roles = "SuperAdmin,Admin TI,Control Escolar")]
    public async Task<IActionResult> Restore(int id)
    {
        try
        {
            await _grupoService.RestoreAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (BusinessException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al restaurar grupo {id}");
            return StatusCode(500, new { message = "Error al restaurar grupo." });
        }
    }
}
