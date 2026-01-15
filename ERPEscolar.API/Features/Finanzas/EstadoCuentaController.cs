using ERPEscolar.API.DTOs.Finanzas;
using ERPEscolar.API.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPEscolar.API.Features.Finanzas;

[Authorize]
[ApiController]
[Route("api/finanzas/estado-cuenta")]
public class EstadoCuentaController : ControllerBase
{
    private readonly IEstadoCuentaService _estadoCuentaService;

    public EstadoCuentaController(IEstadoCuentaService estadoCuentaService)
    {
        _estadoCuentaService = estadoCuentaService;
    }

    [HttpGet("{alumnoId}")]
    public async Task<ActionResult<EstadoCuentaDto>> GetEstadoCuenta(int alumnoId)
    {
        var estadoCuenta = await _estadoCuentaService.GetEstadoCuentaAsync(alumnoId);
        return Ok(estadoCuenta);
    }

    [HttpGet("{alumnoId}/historial")]
    public async Task<ActionResult<EstadoCuentaHistorialDto>> GetHistorialEstadosCuenta(
        int alumnoId,
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null)
    {
        var historial = await _estadoCuentaService.GetHistorialEstadosCuentaAsync(alumnoId, desde, hasta);
        return Ok(historial);
    }

    [HttpGet("{alumnoId}/pdf")]
    public async Task<IActionResult> DescargarEstadoCuentaPdf(int alumnoId)
    {
        var pdfBytes = await _estadoCuentaService.GenerarEstadoCuentaPdfAsync(alumnoId);

        return File(pdfBytes, "application/pdf", $"estado-cuenta-{alumnoId}.pdf");
    }

    [HttpPost("{alumnoId}/enviar-email")]
    public async Task<IActionResult> EnviarEstadoCuentaPorEmail(int alumnoId, [FromBody] EnviarEstadoCuentaEmailDto dto)
    {
        await _estadoCuentaService.EnviarEstadoCuentaPorEmailAsync(alumnoId, dto);
        return Ok(new { message = "Estado de cuenta enviado por email exitosamente" });
    }
}