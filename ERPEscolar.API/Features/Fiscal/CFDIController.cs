using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPEscolar.API.Infrastructure.Services;
using ERPEscolar.API.Core.Exceptions;
using ERPEscolar.API.DTOs.Fiscal;

namespace ERPEscolar.API.Features.Fiscal;

/// <summary>
/// Controller para gestión de CFDI (Comprobantes Fiscales Digitales por Internet).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CFDIController : ControllerBase
{
    private readonly ICFDIService _cfdiService;
    private readonly ILogger<CFDIController> _logger;

    public CFDIController(ICFDIService cfdiService, ILogger<CFDIController> logger)
    {
        _cfdiService = cfdiService;
        _logger = logger;
    }

    /// <summary>
    /// Obtener listado de CFDI con paginación y búsqueda.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
    {
        try
        {
            var result = await _cfdiService.GetAllAsync(pageNumber, pageSize, searchTerm);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener CFDI");
            return StatusCode(500, new { message = "Error al obtener CFDI." });
        }
    }

    /// <summary>
    /// Obtener CFDI por ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var cfdi = await _cfdiService.GetByIdAsync(id);
            return Ok(cfdi);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener CFDI {Id}", id);
            return StatusCode(500, new { message = "Error al obtener el CFDI." });
        }
    }

    /// <summary>
    /// Obtener CFDI con datos completos (relaciones incluidas).
    /// </summary>
    [HttpGet("{id}/full")]
    public async Task<IActionResult> GetByIdFull(int id)
    {
        try
        {
            var cfdi = await _cfdiService.GetByIdFullAsync(id);
            return Ok(cfdi);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener CFDI completo {Id}", id);
            return StatusCode(500, new { message = "Error al obtener el CFDI completo." });
        }
    }

    /// <summary>
    /// Crear un nuevo CFDI.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCFDIDto request)
    {
        try
        {
            var cfdi = await _cfdiService.CreateCFDIAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = cfdi.Id }, cfdi);
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
            _logger.LogError(ex, "Error al crear CFDI");
            return StatusCode(500, new { message = "Error al crear el CFDI." });
        }
    }

    /// <summary>
    /// Actualizar un CFDI existente.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCFDIDto request)
    {
        try
        {
            var cfdi = await _cfdiService.UpdateCFDIAsync(id, request);
            return Ok(cfdi);
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
            _logger.LogError(ex, "Error al actualizar CFDI {Id}", id);
            return StatusCode(500, new { message = "Error al actualizar el CFDI." });
        }
    }

    /// <summary>
    /// Timbrar un CFDI (generar UUID y marcar como timbrado).
    /// </summary>
    [HttpPost("{id}/timbrar")]
    public async Task<IActionResult> Timbrar(int id, [FromBody] TimbrarCFDIDto? request = null)
    {
        try
        {
            var timbrarRequest = request ?? new TimbrarCFDIDto { CFDIId = id };
            var result = await _cfdiService.TimbrarCFDIAsync(timbrarRequest);

            if (result.Exitoso)
                return Ok(result);
            else
                return BadRequest(result);
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
            _logger.LogError(ex, "Error al timbrar CFDI {Id}", id);
            return StatusCode(500, new { message = "Error al timbrar el CFDI." });
        }
    }

    /// <summary>
    /// Cancelar un CFDI timbrado.
    /// </summary>
    [HttpPost("{id}/cancelar")]
    public async Task<IActionResult> Cancelar(int id, [FromBody] CancelarCFDIDto request)
    {
        try
        {
            request.CFDIId = id; // Asegurar que el ID coincida
            var result = await _cfdiService.CancelarCFDIAsync(request);

            if (result.Exitoso)
                return Ok(result);
            else
                return BadRequest(result);
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
            _logger.LogError(ex, "Error al cancelar CFDI {Id}", id);
            return StatusCode(500, new { message = "Error al cancelar el CFDI." });
        }
    }

    /// <summary>
    /// Obtener CFDI de un cargo específico.
    /// </summary>
    [HttpGet("cargo/{cargoId}")]
    public async Task<IActionResult> GetByCargo(int cargoId)
    {
        try
        {
            var cfdis = await _cfdiService.GetCFDISByCargoAsync(cargoId);
            return Ok(cfdis);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener CFDI del cargo {CargoId}", cargoId);
            return StatusCode(500, new { message = "Error al obtener los CFDI del cargo." });
        }
    }

    /// <summary>
    /// Obtener CFDI por estado.
    /// </summary>
    [HttpGet("estado/{estado}")]
    public async Task<IActionResult> GetByEstado(string estado)
    {
        try
        {
            var cfdis = await _cfdiService.GetCFDISByEstadoAsync(estado);
            return Ok(cfdis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener CFDI por estado {Estado}", estado);
            return StatusCode(500, new { message = "Error al obtener los CFDI por estado." });
        }
    }

    /// <summary>
    /// Obtener CFDI timbrados (para reporte).
    /// </summary>
    [HttpGet("timbrados")]
    public async Task<IActionResult> GetTimbrados([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _cfdiService.GetAllAsync(pageNumber, pageSize, null);
            // Filtrar solo timbrados en el cliente (podría optimizarse en el servicio)
            result.Items = result.Items.Where(c => c.Estado == "Timbrada").ToList();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener CFDI timbrados");
            return StatusCode(500, new { message = "Error al obtener CFDI timbrados." });
        }
    }

    /// <summary>
    /// Obtener CFDI en borrador (para procesar).
    /// </summary>
    [HttpGet("borradores")]
    public async Task<IActionResult> GetBorradores([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _cfdiService.GetAllAsync(pageNumber, pageSize, null);
            // Filtrar solo borradores en el cliente (podría optimizarse en el servicio)
            result.Items = result.Items.Where(c => c.Estado == "Borrador").ToList();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener CFDI en borrador");
            return StatusCode(500, new { message = "Error al obtener CFDI en borrador." });
        }
    }
}
