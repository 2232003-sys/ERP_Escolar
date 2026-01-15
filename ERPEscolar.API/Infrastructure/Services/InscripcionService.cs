using AutoMapper;
using ERPEscolar.API.Core.Exceptions;
using ERPEscolar.API.Data;
using ERPEscolar.API.DTOs.ControlEscolar;
using ERPEscolar.API.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ERPEscolar.API.Infrastructure.Services;

/// <summary>
/// Contrato para operaciones de negocio sobre Inscripciones.
/// Maneja matriculación, desmatriculación, cambios de grupo.
/// </summary>
public interface IInscripcionService
{
    /// <summary>
    /// Crear una nueva inscripción (matricular alumno en grupo).
    /// Valida: alumno existe, grupo existe, no duplicados, fechas válidas.
    /// </summary>
    Task<InscripcionDto> CreateAsync(CreateInscripcionDto request);

    /// <summary>
    /// Obtener inscripción por ID (solo si está activa).
    /// </summary>
    Task<InscripcionDto> GetByIdAsync(int id);

    /// <summary>
    /// Obtener inscripción con datos relacionados (alumno, grupo, ciclo).
    /// </summary>
    Task<InscripcionFullDataDto> GetByIdFullAsync(int id);

    /// <summary>
    /// Obtener todas las inscripciones con paginación y búsqueda.
    /// </summary>
    Task<PaginatedInscripcionesDto> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null);

    /// <summary>
    /// Obtener inscripciones de un alumno específico.
    /// </summary>
    Task<List<InscripcionDto>> GetByAlumnoAsync(int alumnoId);

    /// <summary>
    /// Obtener inscripciones de un grupo específico.
    /// </summary>
    Task<List<InscripcionDto>> GetByGrupoAsync(int grupoId);

    /// <summary>
    /// Actualizar inscripción (cambiar grupo, fecha).
    /// </summary>
    Task<InscripcionDto> UpdateAsync(int id, UpdateInscripcionDto request);

    /// <summary>
    /// Desactivar inscripción (soft delete).
    /// </summary>
    Task<bool> SoftDeleteAsync(int id);

    /// <summary>
    /// Reactivar inscripción desactivada.
    /// </summary>
    Task<bool> RestoreAsync(int id);

    /// <summary>
    /// Verificar si una inscripción existe.
    /// </summary>
    Task<bool> ExistsAsync(int id);
}

/// <summary>
/// Implementación de servicios de negocio para Inscripciones.
/// Gestiona matriculación, desmatriculación y cambios de grupo.
/// Valida reglas de negocio: unicidad, integridad referencial, soft delete.
/// </summary>
public class InscripcionService : IInscripcionService
{
    private readonly AppDbContext _context;
    private readonly IValidator<CreateInscripcionDto> _createValidator;
    private readonly IValidator<UpdateInscripcionDto> _updateValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<InscripcionService> _logger;

    public InscripcionService(
        AppDbContext context,
        IValidator<CreateInscripcionDto> createValidator,
        IValidator<UpdateInscripcionDto> updateValidator,
        IMapper mapper,
        ILogger<InscripcionService> logger)
    {
        _context = context;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Crear una nueva inscripción (matricular alumno en grupo).
    /// </summary>
    public async Task<InscripcionDto> CreateAsync(CreateInscripcionDto request)
    {
        _logger.LogInformation("Iniciando inscripción de alumno {AlumnoId} en grupo {GrupoId}", 
            request.AlumnoId, request.GrupoId);

        // Validar con FluentValidation
        var validationResult = await _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new Core.Exceptions.ValidationException(errors);
        }

        // Verificar que el alumno existe y está activo
        var alumno = await _context.Alumnos.FindAsync(request.AlumnoId);
        if (alumno == null || !alumno.Activo)
            throw new NotFoundException("Alumno", request.AlumnoId);

        // Verificar que el grupo existe y está activo
        var grupo = await _context.Grupos.FindAsync(request.GrupoId);
        if (grupo == null || !grupo.Activo)
            throw new NotFoundException("Grupo", request.GrupoId);

        // Verificar que el ciclo escolar existe y está activo
        var ciclo = await _context.CiclosEscolares.FindAsync(request.CicloEscolarId);
        if (ciclo == null || !ciclo.Activo)
            throw new NotFoundException("CicloEscolar", request.CicloEscolarId);

        // Verificar que el ciclo del grupo coincide con el solicitado
        if (grupo.CicloEscolarId != request.CicloEscolarId)
            throw new BusinessException($"El grupo {grupo.Nombre} no pertenece al ciclo escolar especificado.");

        // Verificar que el alumno está en la misma escuela que el grupo
        if (alumno.SchoolId != grupo.SchoolId)
            throw new BusinessException("El alumno y el grupo deben estar en la misma escuela.");

        // Verificar que no existe una inscripción activa del alumno en este grupo en este ciclo
        var inscripcionExistente = await _context.Inscripciones
            .FirstOrDefaultAsync(i => i.AlumnoId == request.AlumnoId 
                && i.GrupoId == request.GrupoId 
                && i.CicloEscolarId == request.CicloEscolarId
                && i.Activo);

        if (inscripcionExistente != null)
            throw new BusinessException($"El alumno ya está inscrito en el grupo {grupo.Nombre} en este ciclo.");

        // Crear inscripción
        var inscripcion = new Inscripcion
        {
            AlumnoId = request.AlumnoId,
            GrupoId = request.GrupoId,
            CicloEscolarId = request.CicloEscolarId,
            FechaInscripcion = request.FechaInscripcion ?? DateTime.UtcNow,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Inscripciones.Add(inscripcion);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Inscripción creada exitosamente: AlumnoId={AlumnoId}, GrupoId={GrupoId}, InscripcionId={InscripcionId}",
            request.AlumnoId, request.GrupoId, inscripcion.Id);

        return _mapper.Map<InscripcionDto>(inscripcion);
    }

    /// <summary>
    /// Obtener inscripción por ID.
    /// </summary>
    public async Task<InscripcionDto> GetByIdAsync(int id)
    {
        var inscripcion = await _context.Inscripciones
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id && i.Activo);

        if (inscripcion == null)
            throw new NotFoundException("Inscripción", id);

        return _mapper.Map<InscripcionDto>(inscripcion);
    }

    /// <summary>
    /// Obtener inscripción con datos completos (alumno, grupo, ciclo, etc.).
    /// </summary>
    public async Task<InscripcionFullDataDto> GetByIdFullAsync(int id)
    {
        var inscripcion = await _context.Inscripciones
            .Include(i => i.Alumno)
            .Include(i => i.Grupo)
                .ThenInclude(g => g.GrupoMaterias)
            .Include(i => i.CicloEscolar)
            .Include(i => i.Asistencias)
            .Include(i => i.Calificaciones)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id && i.Activo);

        if (inscripcion == null)
            throw new NotFoundException("Inscripción", id);

        return _mapper.Map<InscripcionFullDataDto>(inscripcion);
    }

    /// <summary>
    /// Obtener inscripciones con paginación y búsqueda.
    /// </summary>
    public async Task<PaginatedInscripcionesDto> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
    {
        // Validar paginación
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var query = _context.Inscripciones.AsQueryable();

        // Filtrar solo inscripciones ACTIVAS
        query = query.Where(i => i.Activo);

        // Filtrar por término de búsqueda
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim().ToLower();
            query = query.Include(i => i.Alumno)
                .Include(i => i.Grupo)
                .Where(i =>
                    i.Alumno.Nombre.ToLower().Contains(searchTerm) ||
                    i.Alumno.Apellido.ToLower().Contains(searchTerm) ||
                    i.Alumno.Matricula!.ToLower().Contains(searchTerm) ||
                    i.Grupo.Nombre.ToLower().Contains(searchTerm)
                );
        }

        // Contar total
        var totalItems = await query.CountAsync();

        // Paginar
        var inscripciones = await query
            .OrderBy(i => i.AlumnoId)
            .ThenBy(i => i.GrupoId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return new PaginatedInscripcionesDto
        {
            Items = inscripciones.Select(i => _mapper.Map<InscripcionDto>(i)).ToList(),
            TotalItems = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Obtener inscripciones de un alumno específico.
    /// </summary>
    public async Task<List<InscripcionDto>> GetByAlumnoAsync(int alumnoId)
    {
        // Verificar que el alumno existe
        var alumnoExists = await _context.Alumnos.AnyAsync(a => a.Id == alumnoId);
        if (!alumnoExists)
            throw new NotFoundException("Alumno", alumnoId);

        var inscripciones = await _context.Inscripciones
            .Where(i => i.AlumnoId == alumnoId && i.Activo)
            .OrderBy(i => i.FechaInscripcion)
            .AsNoTracking()
            .ToListAsync();

        return inscripciones.Select(i => _mapper.Map<InscripcionDto>(i)).ToList();
    }

    /// <summary>
    /// Obtener inscripciones de un grupo específico.
    /// </summary>
    public async Task<List<InscripcionDto>> GetByGrupoAsync(int grupoId)
    {
        // Verificar que el grupo existe
        var grupoExists = await _context.Grupos.AnyAsync(g => g.Id == grupoId);
        if (!grupoExists)
            throw new NotFoundException("Grupo", grupoId);

        var inscripciones = await _context.Inscripciones
            .Where(i => i.GrupoId == grupoId && i.Activo)
            .OrderBy(i => i.AlumnoId)
            .AsNoTracking()
            .ToListAsync();

        return inscripciones.Select(i => _mapper.Map<InscripcionDto>(i)).ToList();
    }

    /// <summary>
    /// Actualizar inscripción (cambiar grupo, fecha).
    /// </summary>
    public async Task<InscripcionDto> UpdateAsync(int id, UpdateInscripcionDto request)
    {
        _logger.LogInformation("Actualizando inscripción {InscripcionId}", id);

        // Validar
        var validationResult = await _updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new Core.Exceptions.ValidationException(errors);
        }

        // Obtener inscripción
        var inscripcion = await _context.Inscripciones.FindAsync(id);
        if (inscripcion == null || !inscripcion.Activo)
            throw new NotFoundException("Inscripción", id);

        // Si cambia el grupo, validar el nuevo grupo
        if (request.GrupoId.HasValue && request.GrupoId != inscripcion.GrupoId)
        {
            var nuevoGrupo = await _context.Grupos.FindAsync(request.GrupoId);
            if (nuevoGrupo == null || !nuevoGrupo.Activo)
                throw new NotFoundException("Grupo", request.GrupoId.Value);

            // Verificar que el nuevo grupo está en el mismo ciclo
            if (nuevoGrupo.CicloEscolarId != inscripcion.CicloEscolarId)
                throw new BusinessException("El nuevo grupo no pertenece al mismo ciclo escolar.");

            // Verificar que no existe una inscripción activa en el nuevo grupo
            var inscripcionEnNuevoGrupo = await _context.Inscripciones
                .FirstOrDefaultAsync(i => i.AlumnoId == inscripcion.AlumnoId 
                    && i.GrupoId == request.GrupoId 
                    && i.CicloEscolarId == inscripcion.CicloEscolarId
                    && i.Activo);

            if (inscripcionEnNuevoGrupo != null)
                throw new BusinessException($"El alumno ya está inscrito en el grupo {nuevoGrupo.Nombre}.");

            inscripcion.GrupoId = request.GrupoId.Value;
        }

        // Actualizar fecha si se proporciona
        if (request.FechaInscripcion.HasValue)
            inscripcion.FechaInscripcion = request.FechaInscripcion.Value;

        _context.Inscripciones.Update(inscripcion);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Inscripción actualizada: {InscripcionId}", id);

        return _mapper.Map<InscripcionDto>(inscripcion);
    }

    /// <summary>
    /// Desactivar inscripción (soft delete).
    /// </summary>
    public async Task<bool> SoftDeleteAsync(int id)
    {
        var inscripcion = await _context.Inscripciones.FindAsync(id);
        if (inscripcion == null)
            throw new NotFoundException("Inscripción", id);

        if (!inscripcion.Activo)
            throw new BusinessException("La inscripción ya está desactivada.");

        inscripcion.Activo = false;
        _context.Inscripciones.Update(inscripcion);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Inscripción desactivada: {InscripcionId}", id);
        return true;
    }

    /// <summary>
    /// Reactivar inscripción desactivada.
    /// </summary>
    public async Task<bool> RestoreAsync(int id)
    {
        var inscripcion = await _context.Inscripciones.FindAsync(id);
        if (inscripcion == null)
            throw new NotFoundException("Inscripción", id);

        if (inscripcion.Activo)
            throw new BusinessException("La inscripción ya está activa.");

        inscripcion.Activo = true;
        _context.Inscripciones.Update(inscripcion);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Inscripción reactivada: {InscripcionId}", id);
        return true;
    }

    /// <summary>
    /// Verificar si una inscripción existe.
    /// </summary>
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Inscripciones
            .AnyAsync(i => i.Id == id && i.Activo);
    }
}
