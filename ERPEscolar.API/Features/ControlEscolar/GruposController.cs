
using ERPEscolar.DTOs.ControlEscolar;
using ERPEscolar.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ERPEscolar.API.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace ERPEscolar.Features.ControlEscolar
{
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var grupos = await _grupoService.GetAllAsync();
                return Ok(grupos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los grupos.");
                return StatusCode(500, "Ocurrió un error interno al procesar la solicitud.");
            }
        }

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
                _logger.LogWarning(ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el grupo con ID {Id}", id);
                return StatusCode(500, "Ocurrió un error interno.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin,ControlEscolar")]
        public async Task<IActionResult> Create([FromBody] CreateGrupoDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var grupoDto = await _grupoService.CreateGrupoAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = grupoDto.Id }, grupoDto);
            }
            catch (ConflictException ex)
            {
                _logger.LogWarning(ex.Message);
                return Conflict(new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el grupo.");
                return StatusCode(500, "Ocurrió un error interno.");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin,ControlEscolar")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGrupoDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var grupoDto = await _grupoService.UpdateGrupoAsync(id, request);
                return Ok(grupoDto);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el grupo con ID {Id}", id);
                return StatusCode(500, "Ocurrió un error interno.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin,ControlEscolar")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _grupoService.SoftDeleteAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error al desactivar el grupo con ID {Id}", id);
                return StatusCode(500, "Ocurrió un error interno.");
            }
        }
        
        [HttpPatch("{id}/restore")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                await _grupoService.RestoreAsync(id);
                return Ok(new { message = "Grupo reactivado correctamente." });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (ConflictException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reactivar el grupo con ID {Id}", id);
                return StatusCode(500, "Ocurrió un error interno.");
            }
        }
    }
}
