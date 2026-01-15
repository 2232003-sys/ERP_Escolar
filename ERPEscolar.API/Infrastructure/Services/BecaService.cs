using ERPEscolar.API.Core.Exceptions;
using ERPEscolar.API.Data;
using ERPEscolar.API.DTOs.Finanzas;
using ERPEscolar.API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ERPEscolar.API.Infrastructure.Services;

public interface IBecaService
{
    Task<PaginatedBecasDto> GetBecasAsync(int pageNumber = 1, int pageSize = 10, string? search = null, int? alumnoId = null);
    Task<BecaDto> GetBecaByIdAsync(int id);
    Task<BecaDto> CreateBecaAsync(CreateBecaDto dto);
    Task<BecaDto> UpdateBecaAsync(int id, UpdateBecaDto dto);
    Task DeleteBecaAsync(int id);
    Task<IEnumerable<BecaDto>> GetBecasByAlumnoAsync(int alumnoId);
    Task<IEnumerable<BecaDto>> GetBecasActivasAsync();
}

public class BecaService : IBecaService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public BecaService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedBecasDto> GetBecasAsync(int pageNumber = 1, int pageSize = 10, string? search = null, int? alumnoId = null)
    {
        var query = _context.Becas
            .Include(b => b.Alumno)
            .AsQueryable();

        if (alumnoId.HasValue)
        {
            query = query.Where(b => b.AlumnoId == alumnoId.Value);
        }

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(b => b.Nombre.Contains(search) ||
                                    b.Alumno.Nombre.Contains(search) ||
                                    b.Alumno.Apellido.Contains(search));
        }

        var totalCount = await query.CountAsync();
        var becas = await query
            .OrderByDescending(b => b.FechaCreacion)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var becaDtos = _mapper.Map<IEnumerable<BecaDto>>(becas);

        return new PaginatedBecasDto
        {
            Becas = becaDtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<BecaDto> GetBecaByIdAsync(int id)
    {
        var beca = await _context.Becas
            .Include(b => b.Alumno)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (beca == null)
        {
            throw new NotFoundException("Beca no encontrada");
        }

        return _mapper.Map<BecaDto>(beca);
    }

    public async Task<BecaDto> CreateBecaAsync(CreateBecaDto dto)
    {
        // Validate alumno exists
        var alumno = await _context.Alumnos.FindAsync(dto.AlumnoId);
        if (alumno == null)
        {
            throw new NotFoundException("Alumno no encontrado");
        }

        // Check for overlapping active scholarships
        var overlappingBeca = await _context.Becas
            .Where(b => b.AlumnoId == dto.AlumnoId && b.Activa &&
                       ((dto.FechaFin.HasValue && b.FechaFin.HasValue &&
                         dto.FechaInicio < b.FechaFin && dto.FechaFin > b.FechaInicio) ||
                        (!dto.FechaFin.HasValue && b.FechaFin.HasValue && dto.FechaInicio < b.FechaFin) ||
                        (dto.FechaFin.HasValue && !b.FechaFin.HasValue && dto.FechaFin > b.FechaInicio)))
            .FirstOrDefaultAsync();

        if (overlappingBeca != null)
        {
            throw new BusinessException("El alumno ya tiene una beca activa en el período especificado");
        }

        var beca = _mapper.Map<Beca>(dto);
        _context.Becas.Add(beca);
        await _context.SaveChangesAsync();

        // Reload with includes
        await _context.Entry(beca).Reference(b => b.Alumno).LoadAsync();

        return _mapper.Map<BecaDto>(beca);
    }

    public async Task<BecaDto> UpdateBecaAsync(int id, UpdateBecaDto dto)
    {
        var beca = await _context.Becas
            .Include(b => b.Alumno)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (beca == null)
        {
            throw new NotFoundException("Beca no encontrada");
        }

        // If updating dates, check for overlaps
        if (dto.FechaInicio.HasValue || dto.FechaFin.HasValue || dto.Activa.HasValue)
        {
            var fechaInicio = dto.FechaInicio ?? beca.FechaInicio;
            var fechaFin = dto.FechaFin ?? beca.FechaFin;
            var activa = dto.Activa ?? beca.Activa;

            if (activa)
            {
                var overlappingBeca = await _context.Becas
                    .Where(b => b.Id != id && b.AlumnoId == beca.AlumnoId && b.Activa &&
                               ((fechaFin.HasValue && b.FechaFin.HasValue &&
                                 fechaInicio < b.FechaFin && fechaFin > b.FechaInicio) ||
                                (!fechaFin.HasValue && b.FechaFin.HasValue && fechaInicio < b.FechaFin) ||
                                (fechaFin.HasValue && !b.FechaFin.HasValue && fechaFin > b.FechaInicio)))
                    .FirstOrDefaultAsync();

                if (overlappingBeca != null)
                {
                    throw new BusinessException("El alumno ya tiene una beca activa en el período especificado");
                }
            }
        }

        _mapper.Map(dto, beca);
        beca.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<BecaDto>(beca);
    }

    public async Task DeleteBecaAsync(int id)
    {
        var beca = await _context.Becas.FindAsync(id);
        if (beca == null)
        {
            throw new NotFoundException("Beca no encontrada");
        }

        _context.Becas.Remove(beca);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<BecaDto>> GetBecasByAlumnoAsync(int alumnoId)
    {
        var becas = await _context.Becas
            .Include(b => b.Alumno)
            .Where(b => b.AlumnoId == alumnoId)
            .OrderByDescending(b => b.FechaCreacion)
            .ToListAsync();

        return _mapper.Map<IEnumerable<BecaDto>>(becas);
    }

    public async Task<IEnumerable<BecaDto>> GetBecasActivasAsync()
    {
        var becas = await _context.Becas
            .Include(b => b.Alumno)
            .Where(b => b.Activa)
            .OrderBy(b => b.Alumno.Nombre)
            .ThenBy(b => b.Nombre)
            .ToListAsync();

        return _mapper.Map<IEnumerable<BecaDto>>(becas);
    }
}