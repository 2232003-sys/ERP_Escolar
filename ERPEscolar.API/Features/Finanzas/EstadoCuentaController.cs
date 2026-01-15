
using ERPEscolar.API.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ERPEscolar.API.Features.Finanzas;

[ApiController]
[Route("api/estado-cuenta")]
[Authorize] // Proteger todo el controlador
public class EstadoCuentaController : ControllerBase
{
    private readonly EstadoCuentaService _estadoCuentaService;

    public EstadoCuentaController(EstadoCuentaService estadoCuentaService)
    {
        _estadoCuentaService = estadoCuentaService;
    }

    /// <summary>
    /// Obtiene el estado de cuenta financiero completo de un alumno específico.
    /// </summary>
    /// <param name="alumnoId">El ID del alumno a consultar.</param>
    /// <returns>Un DTO con el detalle de cargos, pagos y saldo del alumno.</returns>
    [HttpGet("{alumnoId:int}")]
    public async Task<IActionResult> GetEstadoCuenta(int alumnoId)
    {
        if (alumnoId <= 0)
        {
            return BadRequest(new { message = "El ID del alumno no es válido." });
        }

        try
        {
            var estadoCuenta = await _estadoCuentaService.GetEstadoCuentaAsync(alumnoId);
            return Ok(estadoCuenta);
        }
        catch (Exception ex)
        {
            // Si el servicio lanza una excepción (ej: alumno no encontrado), devolver un 404.
            return NotFound(new { message = ex.Message });
        }
    }
}
