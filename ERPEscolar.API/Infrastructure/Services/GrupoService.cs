using AutoMapper;
using ERPEscolar.API.Core.Exceptions;
using ERPEscolar.API.Data;
using ERPEscolar.API.DTOs.ControlEscolar;
using ERPEscolar.API.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ERPEscolar.API.Infrastructure.Services;

/// <summary>
/// Contrato para operaciones de negocio sobre Grupos.
/// Maneja CRUD, soft delete/restore, búsqueda paginada y validaciones.
/// </summary>
public interface IGrupoService
{
    /// <summary>
    /// Crea un nuevo grupo con validaciones de negocio.
    /// Valida: unicidad (SchoolId, CicloEscolarId, Nombre), 
    /// existencia de escuela, ciclo escolar y docente tutor si aplica.
    /// </summary>
    Task<GrupoDto> CreateGrupoAsync(CreateGrupoDto request);

    /// <summary>
    /// Obtiene un grupo por su ID (solo si está activo).
    /// </summary>
    Task<GrupoDto> GetByIdAsync(int id);

    /// <summary>
    /// Obtiene un grupo con datos relacionados (CicloEscolar, DocenteTutor, Inscripciones).
    /// </summary>
    Task<GrupoFullDataDto> GetByIdFullAsync(int id);

    /// <summary>
    /// Obtiene todos los grupos activos con búsqueda y paginación.
    /// </summary>
    Task<PaginatedGruposDto> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null);

    /// <summary>
    /// Actualiza un grupo existente (debe estar activo).
    /// Re-valida unicidad si el nombre cambió.
    /// </summary>
    Task<GrupoDto> UpdateGrupoAsync(int id, UpdateGrupoDto request);

    /// <summary>
    /// Soft delete: marca el grupo como inactivo.
    /// </summary>
    Task SoftDeleteAsync(int id);

    /// <summary>
    /// Restaura un grupo previamente eliminado (marca como activo).
    /// </summary>
    Task RestoreAsync(int id);

    /// <summary>
    /// Verifica si un grupo existe y está activo.
    /// </summary>
    Task<bool> ExistsAsync(int id);
}

/// <summary>
/// Implementación de servicios de negocio para Grupos.
/// Gestiona creación, actualización, lectura y eliminación lógica de grupos.
/// Valida reglas de negocio: unicidad, integridad referencial, soft delete.
/// </summary>
public class GrupoService : IGrupoService
{
    private readonly AppDbContext _context;
    private readonly IValidator<CreateGrupoDto> _createValidator;
    private readonly IValidator<UpdateGrupoDto> _updateValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<GrupoService> _logger;

    public GrupoService(
        AppDbContext context,
        IValidator<CreateGrupoDto> createValidator,
        IValidator<UpdateGrupoDto> updateValidator,
        IMapper mapper,
        ILogger<GrupoService> logger)
    {
        _context = context;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GrupoDto> CreateGrupoAsync(CreateGrupoDto request)
    {
        _logger.LogInformation("Iniciando creación de grupo: {Nombre}", request.Nombre);

        // Validación con FluentValidation
        var validationResult = await _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new Core.Exceptions.ValidationException(errors);
        }

        // Verificar que la escuela existe y está activa
        var school = await _context.Schools
            .FirstOrDefaultAsync(s => s.Id == request.SchoolId && s.Activo);
        if (school == null)
        {
            _logger.LogWarning("Escuela con ID {SchoolId} no encontrada o inactiva", request.SchoolId);
            throw new BusinessException($"La escuela con ID {request.SchoolId} no existe o no está activa.");
        }

        // Verificar que el ciclo escolar existe y está activo
        var ciclo = await _context.CiclosEscolares
            .FirstOrDefaultAsync(c => c.Id == request.CicloEscolarId 
                && c.SchoolId == request.SchoolId 
                && c.Activo);
        if (ciclo == null)
        {
            _logger.LogWarning("Ciclo escolar con ID {CicloId} no encontrado en escuela {SchoolId}", 
                request.CicloEscolarId, request.SchoolId);
            throw new BusinessException($"El ciclo escolar con ID {request.CicloEscolarId} no existe en esta escuela.");
        }

        // Verificar unicidad: (SchoolId, CicloEscolarId, Nombre) con filtro Activo=true
        var nombreNormalizado = request.Nombre.Trim();
        var existingGrupo = await _context.Grupos
            .FirstOrDefaultAsync(g => g.SchoolId == request.SchoolId
                && g.CicloEscolarId == request.CicloEscolarId
                && g.Nombre == nombreNormalizado
                && g.Activo);
        if (existingGrupo != null)
        {
            _logger.LogWarning("Grupo duplicado: SchoolId={SchoolId}, CicloId={CicloId}, Nombre={Nombre}",
                request.SchoolId, request.CicloEscolarId, request.Nombre);
            throw new BusinessException(
                $"Ya existe un grupo con el nombre '{request.Nombre}' en este ciclo escolar.");
        }

        // Validar docente tutor si viene especificado
        if (request.DocenteTutorId.HasValue)
        {
            var docente = await _context.Docentes
                .FirstOrDefaultAsync(d => d.Id == request.DocenteTutorId.Value
                    && d.SchoolId == request.SchoolId
                    && d.Activo);
            if (docente == null)
            {
                _logger.LogWarning("Docente tutor con ID {DocenteId} no encontrado en escuela {SchoolId}",
                    request.DocenteTutorId.Value, request.SchoolId);
                throw new BusinessException(
                    $"El docente tutor con ID {request.DocenteTutorId} no existe en esta escuela o no está activo.");
            }
        }

        // Crear grupo
        var grupo = _mapper.Map<Grupo>(request);
        _context.Grupos.Add(grupo);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Grupo creado exitosamente con ID {GrupoId}", grupo.Id);
        return _mapper.Map<GrupoDto>(grupo);
    }

    public async Task<GrupoDto> GetByIdAsync(int id)
    {
        var grupo = await _context.Grupos
            .FirstOrDefaultAsync(g => g.Id == id && g.Activo);
        
        if (grupo == null)
        {
            _logger.LogWarning("Grupo con ID {GrupoId} no encontrado", id);
            throw new NotFoundException($"El grupo con ID {id} no existe.");
        }

        return _mapper.Map<GrupoDto>(grupo);
    }

    public async Task<GrupoFullDataDto> GetByIdFullAsync(int id)
    {
        var grupo = await _context.Grupos
            .Include(g => g.CicloEscolar)
            .Include(g => g.DocenteTutor)
            .Include(g => g.Inscripciones.Where(i => i.Activo))
            .FirstOrDefaultAsync(g => g.Id == id && g.Activo);
        
        if (grupo == null)
        {
            _logger.LogWarning("Grupo con ID {GrupoId} no encontrado (lectura completa)", id);
            throw new NotFoundException($"El grupo con ID {id} no existe.");
        }

        return _mapper.Map<GrupoFullDataDto>(grupo);
    }

    public async Task<PaginatedGruposDto> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Límite máximo

        var query = _context.Grupos
            .Where(g => g.Activo)
            .AsQueryable();

        // Búsqueda por nombre si viene searchTerm
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchNormalized = searchTerm.Trim().ToLower();
            query = query.Where(g => g.Nombre.ToLower().Contains(searchNormalized));
        }

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var grupos = await query
            .OrderBy(g => g.Nombre)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var items = grupos.Select(g => _mapper.Map<GrupoDto>(g)).ToList();

        return new PaginatedGruposDto
        {
            Items = items,
            TotalItems = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize,
           //TotalPages = totalPages
        
        
        };
    }

    public async Task<GrupoDto> UpdateGrupoAsync(int id, UpdateGrupoDto request)
    {
        _logger.LogInformation("Iniciando actualización de grupo {GrupoId}", id);

        // Validación con FluentValidation
        var validationResult = await _updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new Core.Exceptions.ValidationException(errors);
        }

        // Obtener grupo y verificar que existe y está activo
        var grupo = await _context.Grupos
            .FirstOrDefaultAsync(g => g.Id == id);
        
        if (grupo == null)
        {
            _logger.LogWarning("Grupo con ID {GrupoId} no encontrado para actualización", id);
            throw new NotFoundException($"El grupo con ID {id} no existe.");
        }

        if (!grupo.Activo)
        {
            _logger.LogWarning("Intento de actualizar grupo inactivo {GrupoId}", id);
            throw new BusinessException($"No se puede actualizar un grupo eliminado.");
        }

        // Re-validar unicidad solo si el nombre cambió
        var nombreNormalizado = request.Nombre.Trim();
        if (grupo.Nombre != nombreNormalizado)
        {
            var existingGrupo = await _context.Grupos
                .FirstOrDefaultAsync(g => g.SchoolId == grupo.SchoolId
                    && g.CicloEscolarId == grupo.CicloEscolarId
                    && g.Nombre == nombreNormalizado
                    && g.Activo
                    && g.Id != id); // Excluir el grupo actual
            if (existingGrupo != null)
            {
                _logger.LogWarning("Intento de crear duplicado al actualizar: Nombre={Nombre}", request.Nombre);
                throw new BusinessException(
                    $"Ya existe un grupo con el nombre '{request.Nombre}' en este ciclo escolar.");
            }
        }

        // Re-validar docente tutor solo si cambió
        if (request.DocenteTutorId != grupo.DocenteTutorId)
        {
            if (request.DocenteTutorId.HasValue)
            {
                var docente = await _context.Docentes
                    .FirstOrDefaultAsync(d => d.Id == request.DocenteTutorId.Value
                        && d.SchoolId == grupo.SchoolId
                        && d.Activo);
                if (docente == null)
                {
                    _logger.LogWarning("Docente tutor inválido al actualizar grupo {GrupoId}", id);
                    throw new BusinessException(
                        $"El docente tutor con ID {request.DocenteTutorId} no existe en esta escuela o no está activo.");
                }
            }
        }

        // Actualizar solo los campos permitidos
        grupo.Nombre = nombreNormalizado;
        grupo.CapacidadMaxima = request.CapacidadMaxima;
        grupo.DocenteTutorId = request.DocenteTutorId;

        _context.Grupos.Update(grupo);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Grupo {GrupoId} actualizado exitosamente", id);
        return _mapper.Map<GrupoDto>(grupo);
    }

    public async Task SoftDeleteAsync(int id)
    {
        _logger.LogInformation("Iniciando soft delete de grupo {GrupoId}", id);

        var grupo = await _context.Grupos
            .FirstOrDefaultAsync(g => g.Id == id);
        
        if (grupo == null)
        {
            _logger.LogWarning("Grupo con ID {GrupoId} no encontrado para eliminación", id);
            throw new NotFoundException($"El grupo con ID {id} no existe.");
        }

        if (!grupo.Activo)
        {
            _logger.LogWarning("Intento de eliminar grupo ya inactivo {GrupoId}", id);
            throw new BusinessException($"El grupo ya está eliminado.");
        }

        grupo.Activo = false;
        _context.Grupos.Update(grupo);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Grupo {GrupoId} eliminado exitosamente (soft delete)", id);
    }

    public async Task RestoreAsync(int id)
    {
        _logger.LogInformation("Iniciando restauración de grupo {GrupoId}", id);

        var grupo = await _context.Grupos
            .FirstOrDefaultAsync(g => g.Id == id);
        
        if (grupo == null)
        {
            _logger.LogWarning("Grupo con ID {GrupoId} no encontrado para restauración", id);
            throw new NotFoundException($"El grupo con ID {id} no existe.");
        }

        if (grupo.Activo)
        {
            _logger.LogWarning("Intento de restaurar grupo ya activo {GrupoId}", id);
            throw new BusinessException($"El grupo ya está activo.");
        }

        grupo.Activo = true;
        _context.Grupos.Update(grupo);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Grupo {GrupoId} restaurado exitosamente", id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Grupos
            .AnyAsync(g => g.Id == id && g.Activo);
    }
}
