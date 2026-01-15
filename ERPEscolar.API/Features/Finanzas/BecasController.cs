using ERPEscolar.API.DTOs.Finanzas;
using ERPEscolar.API.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPEscolar.API.Features.Finanzas;

[Authorize]
[ApiController]
[Route("api/finanzas/becas")]
public class BecasController : ControllerBase
{
    private readonly IBecaService _becaService;

    public BecasController(IBecaService becaService)
    {
        _becaService = becaService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedBecasDto>> GetBecas(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] int? alumnoId = null)
    {
        var result = await _becaService.GetBecasAsync(pageNumber, pageSize, search, alumnoId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BecaDto>> GetBeca(int id)
    {
        var beca = await _becaService.GetBecaByIdAsync(id);
        return Ok(beca);
    }

    [HttpPost]
    public async Task<ActionResult<BecaDto>> CreateBeca([FromBody] CreateBecaDto dto)
    {
        var beca = await _becaService.CreateBecaAsync(dto);
        return CreatedAtAction(nameof(GetBeca), new { id = beca.Id }, beca);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BecaDto>> UpdateBeca(int id, [FromBody] UpdateBecaDto dto)
    {
        var beca = await _becaService.UpdateBecaAsync(id, dto);
        return Ok(beca);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBeca(int id)
    {
        await _becaService.DeleteBecaAsync(id);
        return NoContent();
    }

    [HttpGet("alumno/{alumnoId}")]
    public async Task<ActionResult<IEnumerable<BecaDto>>> GetBecasByAlumno(int alumnoId)
    {
        var becas = await _becaService.GetBecasByAlumnoAsync(alumnoId);
        return Ok(becas);
    }

    [HttpGet("activas")]
    public async Task<ActionResult<IEnumerable<BecaDto>>> GetBecasActivas()
    {
        var becas = await _becaService.GetBecasActivasAsync();
        return Ok(becas);
    }
}