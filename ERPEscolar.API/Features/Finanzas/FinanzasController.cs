
using ERPEscolar.API.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ERPEscolar.API.Features.Finanzas;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Proteger todo el controlador, solo usuarios autenticados pueden acceder.
public class FinanzasController : ControllerBase
{
    private readonly FinanzasService _finanzasService;

    public FinanzasController(FinanzasService finanzasService)
    {
        _finanzasService = finanzasService;
    }

    /// <summary>
    /// Endpoint para iniciar el proceso de generación masiva de cargos mensuales (colegiaturas).
    /// </summary>
    [HttpPost("generar-cargos-mensuales")]
    public async Task<IActionResult> GenerarCargos([FromBody] GenerarCargosRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var result = await _finanzasService.GenerarCargosMensualesAsync(request.Year, request.Month);
            return Ok(new { message = result });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return StatusCode(500, new { message = "Ocurrió un error inesperado en el servidor al generar los cargos." });
        }
    }

    /// <summary>
    /// Endpoint para registrar un nuevo pago y aplicarlo a un cargo existente.
    /// </summary>
    /// <param name="request">Los datos del pago a registrar.</param>
    /// <returns>El pago registrado.</returns>
    [HttpPost("registrar-pago")]
    public async Task<IActionResult> RegistrarPago([FromBody] RegistrarPagoRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var nuevoPago = await _finanzasService.RegistrarPagoAsync(request);
            return Ok(nuevoPago);
        }
        catch (Exception ex)
        {
            // Capturar excepciones específicas del servicio para dar respuestas claras
            Console.Error.WriteLine(ex);
            return BadRequest(new { message = ex.Message });
        }
    }
}
