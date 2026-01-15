using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ERPEscolar.API.Data;
using ERPEscolar.API.DTOs.ControlEscolar;
using ERPEscolar.API.Models;
using ERPEscolar.API.Core.Exceptions;

namespace ERPEscolar.API.Infrastructure.Services;

/// <summary>
/// Interfaz del servicio de Asistencias
/// </summary>
public interface IAsistenciaService
{
    /// <summary>
    /// Crear una nueva asistencia
    /// </summary>
    Task<AsistenciaDto> CreateAsync(CreateAsistenciaDto request);

    /// <summary>
    /// Obtener una asistencia por ID
    /// </summary>
    Task<AsistenciaDto> GetByIdAsync(int id);

    /// <summary>
    /// Obtener una asistencia con datos completos (incluye alumno, materia, etc.)
    /// </summary>
    Task<AsistenciaFullDataDto> GetByIdFullAsync(int id);

    /// <summary>
    /// Obtener todas las asistencias con paginación
    /// </summary>
    Task<PaginatedAsistenciasDto> GetAllAsync(int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Obtener asistencias de una inscripción específica
    /// </summary>
    Task<List<AsistenciaDto>> GetByInscripcionAsync(int inscripcionId);

    /// <summary>
    /// Obtener asistencias de un alumno (todas sus inscripciones)
    /// </summary>
    Task<List<AsistenciaDto>> GetByAlumnoAsync(int alumnoId);

    /// <summary>
    /// Obtener asistencias de un grupo en una fecha específica
    /// </summary>
    Task<List<AsistenciaDto>> GetPorGrupoEnFechaAsync(int grupoId, DateTime fecha);

    /// <summary>
    /// Obtener asistencias de un grupo en rango de fechas
    /// </summary>
    Task<List<AsistenciaDto>> GetPorGrupoEnRangoAsync(int grupoId, DateTime fechaInicio, DateTime fechaFin);

    /// <summary>
    /// Calcular porcentaje de asistencia de un alumno en un grupo
    /// </summary>
    Task<double> GetAsistenciaPercentageAsync(int inscripcionId);

    /// <summary>
    /// Actualizar una asistencia existente
    /// </summary>
    Task<AsistenciaDto> UpdateAsync(int id, UpdateAsistenciaDto request);

    /// <summary>
    /// Eliminar lógicamente una asistencia
    /// </summary>
    Task<bool> SoftDeleteAsync(int id);

    /// <summary>
    /// Restaurar una asistencia eliminada lógicamente
    /// </summary>
    Task<bool> RestoreAsync(int id);

    /// <summary>
    /// Verificar si existe una asistencia
    /// </summary>
    Task<bool> ExistsAsync(int id);
}

/// <summary>
/// Implementación del servicio de Asistencias
/// </summary>
public class AsistenciaService : IAsistenciaService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateAsistenciaDto> _createValidator;
    private readonly IValidator<UpdateAsistenciaDto> _updateValidator;
    private readonly ILogger<AsistenciaService> _logger;

    public AsistenciaService(
        AppDbContext context,
        IMapper mapper,
        IValidator<CreateAsistenciaDto> createValidator,
        IValidator<UpdateAsistenciaDto> updateValidator,
        ILogger<AsistenciaService> logger)
    {
        _context = context;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    /// <summary>
    /// Crear una nueva asistencia con validaciones complejas
    /// </summary>
    public async Task<AsistenciaDto> CreateAsync(CreateAsistenciaDto request)
    {
        _logger.LogInformation("CreateAsync: Iniciando creación de asistencia");

        // Validar DTO
        var validationResult = await _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new Core.Exceptions.ValidationException(errors);
        }

        // Verificar que la inscripción existe y está activa
        var inscripcion = await _context.Set<Inscripcion>()
            .Include(i => i.Alumno)
            .Include(i => i.Grupo)
            .FirstOrDefaultAsync(i => i.Id == request.InscripcionId && i.Activo);

        if (inscripcion == null)
        {
            _logger.LogWarning("CreateAsync: Inscripción no encontrada o inactiva: {Id}", request.InscripcionId);
            throw new BusinessException("La inscripción no existe o está inactiva");
        }

        // Verificar que el grupo/materia existe y está activo
        var grupoMateria = await _context.Set<GrupoMateria>()
            .FirstOrDefaultAsync(gm => gm.Id == request.GrupoMateriaId && gm.Activo);

        if (grupoMateria == null)
        {
            _logger.LogWarning("CreateAsync: GrupoMateria no encontrada o inactiva: {Id}", request.GrupoMateriaId);
            throw new BusinessException("La materia no existe o está inactiva");
        }

        // Verificar que la materia pertenece al mismo grupo de la inscripción
        if (grupoMateria.GrupoId != inscripcion.GrupoId)
        {
            _logger.LogWarning("CreateAsync: La materia no pertenece al grupo de la inscripción");
            throw new BusinessException("La materia no pertenece al grupo del alumno inscrito");
        }

        // Verificar que no existe ya un registro de asistencia para este alumno+materia en esa fecha
        var existeAsistencia = await _context.Set<Asistencia>()
            .AnyAsync(a => a.InscripcionId == request.InscripcionId 
                && a.GrupoMateriaId == request.GrupoMateriaId 
                && a.Fecha.Date == request.Fecha.Date
                && a.Activo);

        if (existeAsistencia)
        {
            _logger.LogWarning("CreateAsync: Ya existe asistencia registrada para este alumno+materia en esa fecha");
            throw new BusinessException("Ya existe un registro de asistencia para esta combinación en esa fecha");
        }

        // Mapear y crear
        var asistencia = _mapper.Map<Asistencia>(request);
        asistencia.FechaCreacion = DateTime.UtcNow;

        _context.Set<Asistencia>().Add(asistencia);
        await _context.SaveChangesAsync();

        _logger.LogInformation("CreateAsync: Asistencia creada exitosamente. ID: {Id}", asistencia.Id);
        return _mapper.Map<AsistenciaDto>(asistencia);
    }

    /// <summary>
    /// Obtener asistencia por ID
    /// </summary>
    public async Task<AsistenciaDto> GetByIdAsync(int id)
    {
        var asistencia = await _context.Set<Asistencia>()
            .FirstOrDefaultAsync(a => a.Id == id && a.Activo);

        if (asistencia == null)
        {
            throw new NotFoundException("Asistencia no encontrada");
        }

        return _mapper.Map<AsistenciaDto>(asistencia);
    }

    /// <summary>
    /// Obtener asistencia con datos completos (incluye relaciones)
    /// </summary>
    public async Task<AsistenciaFullDataDto> GetByIdFullAsync(int id)
    {
        var asistencia = await _context.Set<Asistencia>()
            .Include(a => a.Inscripcion)
                .ThenInclude(i => i.Alumno)
            .Include(a => a.Inscripcion)
                .ThenInclude(i => i.Grupo)
                .ThenInclude(g => g.CicloEscolar)
            .Include(a => a.Inscripcion)
                .ThenInclude(i => i.Grupo)
                .ThenInclude(g => g.DocenteTutor)
            .Include(a => a.GrupoMateria)
                .ThenInclude(gm => gm.Materia)
            .FirstOrDefaultAsync(a => a.Id == id && a.Activo);

        if (asistencia == null)
        {
            throw new NotFoundException("Asistencia no encontrada");
        }

        return _mapper.Map<AsistenciaFullDataDto>(asistencia);
    }

    /// <summary>
    /// Obtener todas las asistencias con paginación
    /// </summary>
    public async Task<PaginatedAsistenciasDto> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        pageSize = Math.Min(pageSize, 100); // Máximo 100 registros
        pageNumber = Math.Max(pageNumber, 1); // Mínimo página 1

        var query = _context.Set<Asistencia>()
            .Where(a => a.Activo)
            .OrderByDescending(a => a.Fecha)
            .ThenBy(a => a.InscripcionId);

        var totalRecords = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedAsistenciasDto
        {
            Items = _mapper.Map<List<AsistenciaDto>>(items),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords
        };
    }

    /// <summary>
    /// Obtener asistencias de una inscripción
    /// </summary>
    public async Task<List<AsistenciaDto>> GetByInscripcionAsync(int inscripcionId)
    {
        var inscripcion = await _context.Set<Inscripcion>()
            .FirstOrDefaultAsync(i => i.Id == inscripcionId);

        if (inscripcion == null)
        {
            throw new NotFoundException("Inscripción no encontrada");
        }

        var asistencias = await _context.Set<Asistencia>()
            .Where(a => a.InscripcionId == inscripcionId && a.Activo)
            .OrderByDescending(a => a.Fecha)
            .ToListAsync();

        return _mapper.Map<List<AsistenciaDto>>(asistencias);
    }

    /// <summary>
    /// Obtener asistencias de un alumno (todas sus inscripciones)
    /// </summary>
    public async Task<List<AsistenciaDto>> GetByAlumnoAsync(int alumnoId)
    {
        var alumno = await _context.Set<Alumno>()
            .FirstOrDefaultAsync(a => a.Id == alumnoId);

        if (alumno == null)
        {
            throw new NotFoundException("Alumno no encontrado");
        }

        var asistencias = await _context.Set<Asistencia>()
            .Include(a => a.Inscripcion)
            .Where(a => a.Inscripcion.AlumnoId == alumnoId && a.Activo)
            .OrderByDescending(a => a.Fecha)
            .ToListAsync();

        return _mapper.Map<List<AsistenciaDto>>(asistencias);
    }

    /// <summary>
    /// Obtener asistencias de un grupo en una fecha específica
    /// </summary>
    public async Task<List<AsistenciaDto>> GetPorGrupoEnFechaAsync(int grupoId, DateTime fecha)
    {
        var asistencias = await _context.Set<Asistencia>()
            .Include(a => a.Inscripcion)
            .Where(a => a.Inscripcion.GrupoId == grupoId
                && a.Fecha.Date == fecha.Date
                && a.Activo)
            .OrderBy(a => a.Inscripcion.Alumno.Apellido)
            .ThenBy(a => a.Inscripcion.Alumno.Nombre)
            .ToListAsync();

        return _mapper.Map<List<AsistenciaDto>>(asistencias);
    }

    /// <summary>
    /// Obtener asistencias de un grupo en rango de fechas
    /// </summary>
    public async Task<List<AsistenciaDto>> GetPorGrupoEnRangoAsync(int grupoId, DateTime fechaInicio, DateTime fechaFin)
    {
        var asistencias = await _context.Set<Asistencia>()
            .Include(a => a.Inscripcion)
            .Where(a => a.Inscripcion.GrupoId == grupoId
                && a.Fecha.Date >= fechaInicio.Date
                && a.Fecha.Date <= fechaFin.Date
                && a.Activo)
            .OrderBy(a => a.Fecha)
            .ThenBy(a => a.Inscripcion.Alumno.Apellido)
            .ToListAsync();

        return _mapper.Map<List<AsistenciaDto>>(asistencias);
    }

    /// <summary>
    /// Calcular porcentaje de asistencia de un alumno en una inscripción
    /// </summary>
    public async Task<double> GetAsistenciaPercentageAsync(int inscripcionId)
    {
        var inscripcion = await _context.Set<Inscripcion>()
            .FirstOrDefaultAsync(i => i.Id == inscripcionId && i.Activo);

        if (inscripcion == null)
        {
            throw new NotFoundException("Inscripción no encontrada");
        }

        // Total de clases dictadas en este grupo en este ciclo
        var totalClasesEnGrupo = await _context.Set<Asistencia>()
            .Include(a => a.Inscripcion)
            .Where(a => a.Inscripcion.GrupoId == inscripcion.GrupoId && a.Activo)
            .Select(a => a.Fecha.Date)
            .Distinct()
            .CountAsync();

        if (totalClasesEnGrupo == 0)
        {
            return 100.0; // Si no hay clases registradas, asumimos 100%
        }

        // Asistencias del alumno donde Estado = "Presente" o "Retraso" (se cuenta como asistencia)
        var asistenciasDelAlumno = await _context.Set<Asistencia>()
            .Where(a => a.InscripcionId == inscripcionId
                && (a.Estado == "Presente" || a.Estado == "Retraso")
                && a.Activo)
            .CountAsync();

        var percentage = (double)asistenciasDelAlumno / totalClasesEnGrupo * 100;
        return Math.Round(percentage, 2);
    }

    /// <summary>
    /// Actualizar una asistencia
    /// </summary>
    public async Task<AsistenciaDto> UpdateAsync(int id, UpdateAsistenciaDto request)
    {
        _logger.LogInformation("UpdateAsync: Iniciando actualización de asistencia {Id}", id);

        // Validar DTO
        var validationResult = await _updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new Core.Exceptions.ValidationException(errors);
        }

        var asistencia = await _context.Set<Asistencia>()
            .FirstOrDefaultAsync(a => a.Id == id && a.Activo);

        if (asistencia == null)
        {
            throw new NotFoundException("Asistencia no encontrada");
        }

        // Aplicar cambios
        _mapper.Map(request, asistencia);

        _context.Set<Asistencia>().Update(asistencia);
        await _context.SaveChangesAsync();

        _logger.LogInformation("UpdateAsync: Asistencia actualizada exitosamente. ID: {Id}", id);
        return _mapper.Map<AsistenciaDto>(asistencia);
    }

    /// <summary>
    /// Eliminación lógica de una asistencia
    /// </summary>
    public async Task<bool> SoftDeleteAsync(int id)
    {
        var asistencia = await _context.Set<Asistencia>()
            .FirstOrDefaultAsync(a => a.Id == id && a.Activo);

        if (asistencia == null)
        {
            throw new NotFoundException("Asistencia no encontrada");
        }

        asistencia.Activo = false;
        _context.Set<Asistencia>().Update(asistencia);
        await _context.SaveChangesAsync();

        _logger.LogInformation("SoftDeleteAsync: Asistencia eliminada lógicamente. ID: {Id}", id);
        return true;
    }

    /// <summary>
    /// Restaurar una asistencia eliminada lógicamente
    /// </summary>
    public async Task<bool> RestoreAsync(int id)
    {
        var asistencia = await _context.Set<Asistencia>()
            .FirstOrDefaultAsync(a => a.Id == id && !a.Activo);

        if (asistencia == null)
        {
            throw new NotFoundException("Asistencia no encontrada o ya está activa");
        }

        asistencia.Activo = true;
        _context.Set<Asistencia>().Update(asistencia);
        await _context.SaveChangesAsync();

        _logger.LogInformation("RestoreAsync: Asistencia restaurada. ID: {Id}", id);
        return true;
    }

    /// <summary>
    /// Verificar si existe una asistencia
    /// </summary>
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Set<Asistencia>()
            .AnyAsync(a => a.Id == id && a.Activo);
    }
}
