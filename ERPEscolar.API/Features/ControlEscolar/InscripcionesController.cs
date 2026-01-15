
using ERPEscolar.DTOs.ControlEscolar;
using ERPEscolar.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ERPEscolar.API.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace ERPEscolar.Features.ControlEscolar
{
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

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin,ControlEscolar")]
        public async Task<IActionResult> Create([FromBody] CreateInscripcionDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var inscripcion = await _inscripcionService.CreateInscripcionAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = inscripcion.Id }, inscripcion);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ConflictException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la inscripción.");
                return StatusCode(500, "Ocurrió un error interno al procesar la solicitud.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var inscripcion = await _inscripcionService.GetByIdAsync(id);
                return Ok(inscripcion);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la inscripción con ID: {Id}", id);
                return StatusCode(500, "Ocurrió un error interno.");
            }
        }

        [HttpGet("alumno/{alumnoId}")]
        public async Task<IActionResult> GetByAlumnoId(int alumnoId)
        {
            var inscripciones = await _inscripcionService.GetByAlumnoIdAsync(alumnoId);
            return Ok(inscripciones);
        }

        [HttpGet("grupo/{grupoId}")]
        public async Task<IActionResult> GetByGrupoId(int grupoId)
        {
            var inscripciones = await _inscripcionService.GetByGrupoIdAsync(grupoId);
            return Ok(inscripciones);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var inscripciones = await _inscripcionService.GetAllAsync();
            return Ok(inscripciones);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin,ControlEscolar")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _inscripcionService.DeleteAsync(id);
                return NoContent(); // 204 No Content es la respuesta estándar para un delete exitoso
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la inscripción con ID: {Id}", id);
                return StatusCode(500, "Ocurrió un error interno.");
            }
        }
    }
}
