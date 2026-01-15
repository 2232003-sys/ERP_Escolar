using ERPEscolar.API.Core.Exceptions;
using ERPEscolar.API.Data;
using ERPEscolar.API.DTOs.Finanzas;
using ERPEscolar.API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ERPEscolar.API.Infrastructure.Services;

public interface IPagoService
{
    Task<PaginatedPagosDto> GetPagosAsync(int pageNumber = 1, int pageSize = 10, string? search = null, int? alumnoId = null);
    Task<PagoDto> GetPagoByIdAsync(int id);
    Task<PagoDto> CreatePagoAsync(CreatePagoDto dto);
    Task<PagoDto> CreatePagoTransferenciaAsync(PagoTransferenciaDto dto);
    Task<PagoDto> UpdatePagoAsync(int id, UpdatePagoDto dto);
    Task DeletePagoAsync(int id);
    Task<IEnumerable<PagoDto>> GetPagosByAlumnoAsync(int alumnoId);
    Task<IEnumerable<PagoDto>> GetPagosPendientesConciliacionAsync();
}

public class PagoService : IPagoService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public PagoService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedPagosDto> GetPagosAsync(int pageNumber = 1, int pageSize = 10, string? search = null, int? alumnoId = null)
    {
        var query = _context.Pagos
            .Include(p => p.Alumno)
            .Include(p => p.Cargo)
            .Where(p => p.Activo)
            .AsQueryable();

        if (alumnoId.HasValue)
        {
            query = query.Where(p => p.AlumnoId == alumnoId.Value);
        }

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Folio.Contains(search) ||
                                    p.ReferenciaExterna.Contains(search) ||
                                    p.Alumno.Nombre.Contains(search) ||
                                    p.Alumno.Apellido.Contains(search));
        }

        var totalCount = await query.CountAsync();
        var pagos = await query
            .OrderByDescending(p => p.FechaPago)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var pagoDtos = _mapper.Map<IEnumerable<PagoDto>>(pagos);

        return new PaginatedPagosDto
        {
            Pagos = pagoDtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagoDto> GetPagoByIdAsync(int id)
    {
        var pago = await _context.Pagos
            .Include(p => p.Alumno)
            .Include(p => p.Cargo)
            .FirstOrDefaultAsync(p => p.Id == id && p.Activo);

        if (pago == null)
        {
            throw new NotFoundException("Pago no encontrado");
        }

        return _mapper.Map<PagoDto>(pago);
    }

    public async Task<PagoDto> CreatePagoAsync(CreatePagoDto dto)
    {
        // Validate alumno exists
        var alumno = await _context.Alumnos.FindAsync(dto.AlumnoId);
        if (alumno == null)
        {
            throw new NotFoundException("Alumno no encontrado");
        }

        // Validate colegiatura if provided
        if (dto.ColegiaturaId.HasValue)
        {
            var colegiatura = await _context.Cargos.FindAsync(dto.ColegiaturaId.Value);
            if (colegiatura == null)
            {
                throw new NotFoundException("Colegiatura no encontrada");
            }
        }

        var pago = _mapper.Map<Pago>(dto);
        pago.Folio = await GenerateFolioAsync();

        _context.Pagos.Add(pago);
        await _context.SaveChangesAsync();

        // Reload with includes
        await _context.Entry(pago).Reference(p => p.Alumno).LoadAsync();
        if (pago.CargoId.HasValue)
        {
            await _context.Entry(pago).Reference(p => p.Cargo).LoadAsync();
        }

        return _mapper.Map<PagoDto>(pago);
    }

    public async Task<PagoDto> CreatePagoTransferenciaAsync(PagoTransferenciaDto dto)
    {
        // Validate alumno exists
        var alumno = await _context.Alumnos.FindAsync(dto.AlumnoId);
        if (alumno == null)
        {
            throw new NotFoundException("Alumno no encontrado");
        }

        // Validate colegiatura if provided
        if (dto.ColegiaturaId.HasValue)
        {
            var colegiatura = await _context.Cargos.FindAsync(dto.ColegiaturaId.Value);
            if (colegiatura == null)
            {
                throw new NotFoundException("Colegiatura no encontrada");
            }
        }

        var pago = _mapper.Map<Pago>(dto);
        pago.Folio = await GenerateFolioAsync();
        pago.Estado = "Registrado"; // Transferencias need verification

        _context.Pagos.Add(pago);
        await _context.SaveChangesAsync();

        // Reload with includes
        await _context.Entry(pago).Reference(p => p.Alumno).LoadAsync();
        if (pago.CargoId.HasValue)
        {
            await _context.Entry(pago).Reference(p => p.Cargo).LoadAsync();
        }

        return _mapper.Map<PagoDto>(pago);
    }

    public async Task<PagoDto> UpdatePagoAsync(int id, UpdatePagoDto dto)
    {
        var pago = await _context.Pagos
            .Include(p => p.Alumno)
            .Include(p => p.Cargo)
            .FirstOrDefaultAsync(p => p.Id == id && p.Activo);

        if (pago == null)
        {
            throw new NotFoundException("Pago no encontrado");
        }

        // Only allow updates if not verified/deposited
        if (pago.Estado == "Verificado" || pago.Estado == "Depositado")
        {
            throw new BadRequestException("No se puede editar un pago verificado o depositado");
        }

        _mapper.Map(dto, pago);
        pago.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<PagoDto>(pago);
    }

    public async Task DeletePagoAsync(int id)
    {
        var pago = await _context.Pagos.FindAsync(id);
        if (pago == null || !pago.Activo)
        {
            throw new NotFoundException("Pago no encontrado");
        }

        // Only allow deletion if not verified/deposited
        if (pago.Estado == "Verificado" || pago.Estado == "Depositado")
        {
            throw new BadRequestException("No se puede eliminar un pago verificado o depositado");
        }

        pago.Activo = false;
        pago.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<PagoDto>> GetPagosByAlumnoAsync(int alumnoId)
    {
        var pagos = await _context.Pagos
            .Include(p => p.Alumno)
            .Include(p => p.Cargo)
            .Where(p => p.AlumnoId == alumnoId && p.Activo)
            .OrderByDescending(p => p.FechaPago)
            .ToListAsync();

        return _mapper.Map<IEnumerable<PagoDto>>(pagos);
    }

    public async Task<IEnumerable<PagoDto>> GetPagosPendientesConciliacionAsync()
    {
        var pagos = await _context.Pagos
            .Include(p => p.Alumno)
            .Include(p => p.Cargo)
            .Where(p => p.Activo && (p.Estado == "Registrado" || p.Estado == "Verificado"))
            .OrderBy(p => p.FechaPago)
            .ToListAsync();

        return _mapper.Map<IEnumerable<PagoDto>>(pagos);
    }

    private async Task<string> GenerateFolioAsync()
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var count = await _context.Pagos.CountAsync(p => p.Folio.StartsWith($"PAGO-{today}"));
        return $"PAGO-{today}-{count + 1:D4}";
    }
}