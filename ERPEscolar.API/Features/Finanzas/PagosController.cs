using ERPEscolar.API.DTOs.Finanzas;
using ERPEscolar.API.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPEscolar.API.Features.Finanzas;

[Authorize]
[ApiController]
[Route("api/finanzas/pagos")]
public class PagosController : ControllerBase
{
    private readonly IPagoService _pagoService;

    public PagosController(IPagoService pagoService)
    {
        _pagoService = pagoService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedPagosDto>> GetPagos(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] int? alumnoId = null)
    {
        var result = await _pagoService.GetPagosAsync(pageNumber, pageSize, search, alumnoId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PagoDto>> GetPago(int id)
    {
        var pago = await _pagoService.GetPagoByIdAsync(id);
        return Ok(pago);
    }

    [HttpPost]
    public async Task<ActionResult<PagoDto>> CreatePago([FromBody] CreatePagoDto dto)
    {
        var pago = await _pagoService.CreatePagoAsync(dto);
        return CreatedAtAction(nameof(GetPago), new { id = pago.Id }, pago);
    }

    [HttpPost("transferencia")]
    public async Task<ActionResult<PagoDto>> CreatePagoTransferencia([FromBody] PagoTransferenciaDto dto)
    {
        var pago = await _pagoService.CreatePagoTransferenciaAsync(dto);
        return CreatedAtAction(nameof(GetPago), new { id = pago.Id }, pago);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PagoDto>> UpdatePago(int id, [FromBody] UpdatePagoDto dto)
    {
        var pago = await _pagoService.UpdatePagoAsync(id, dto);
        return Ok(pago);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePago(int id)
    {
        await _pagoService.DeletePagoAsync(id);
        return NoContent();
    }

    [HttpGet("alumno/{alumnoId}")]
    public async Task<ActionResult<IEnumerable<PagoDto>>> GetPagosByAlumno(int alumnoId)
    {
        var pagos = await _pagoService.GetPagosByAlumnoAsync(alumnoId);
        return Ok(pagos);
    }

    [HttpGet("pendientes-conciliacion")]
    public async Task<ActionResult<IEnumerable<PagoDto>>> GetPagosPendientesConciliacion()
    {
        var pagos = await _pagoService.GetPagosPendientesConciliacionAsync();
        return Ok(pagos);
    }
}