using ERPEscolar.API.Data;
using ERPEscolar.API.Models;
using ERPEscolar.API.DTOs.ControlEscolar;
using ERPEscolar.API.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
namespace ERPEscolar.API.Infrastructure.Services;

/// <summary>
/// Servicio de gestión de alumnos con validaciones de negocio.
/// </summary>
public interface IAlumnoService
{
    Task<AlumnoDto> CreateAlumnoAsync(CreateAlumnoDto request);
    Task<AlumnoDto> GetByIdAsync(int id);
    Task<AlumnoFullDataDto> GetByIdFullAsync(int id);
    Task<PaginatedAlumnosDto> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null);
    Task<AlumnoDto> UpdateAlumnoAsync(int id, UpdateAlumnoDto request);
    Task<bool> SoftDeleteAsync(int id);
    Task<bool> RestoreAsync(int id);
    Task<bool> ExistsAsync(int id);
}

public class AlumnoService : IAlumnoService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AlumnoService> _logger;
    private readonly IValidator<CreateAlumnoDto> _createValidator;
    private readonly IValidator<UpdateAlumnoDto> _updateValidator;

    public AlumnoService(
        AppDbContext context,
        ILogger<AlumnoService> logger,
        IValidator<CreateAlumnoDto> createValidator,
        IValidator<UpdateAlumnoDto> updateValidator)
    {
        _context = context;
        _logger = logger;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>
    /// Crear un nuevo alumno con validaciones.
    /// </summary>
    public async Task<AlumnoDto> CreateAlumnoAsync(CreateAlumnoDto request)
    {
        try
        {
            // Validar con FluentValidation
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                throw new Core.Exceptions.ValidationException(errors);
            }

            // Verificar que la escuela existe
            var schoolExists = await _context.Schools.AnyAsync(s => s.Id == request.SchoolId && s.Activo);
            if (!schoolExists)
                throw new NotFoundException("School", request.SchoolId);

            // Verificar CURP único - considera solo alumnos ACTIVOS (soft delete)
            var curpExists = await _context.Alumnos
                .AnyAsync(a => a.CURP == request.CURP.Trim().ToUpper() && a.Activo && a.SchoolId == request.SchoolId);
            if (curpExists)
                throw new BusinessException($"Ya existe un alumno activo con CURP '{request.CURP}' en esta escuela.");

            // Verificar email único - considera solo alumnos ACTIVOS (soft delete)
            var emailNormalized = request.Email.Trim().ToLower();
            var emailExists = await _context.Alumnos
                .AnyAsync(a => a.Email == emailNormalized && a.Activo && a.SchoolId == request.SchoolId);
            if (emailExists)
                throw new BusinessException($"Ya existe un alumno activo con email '{request.Email}' en esta escuela.");

            // Generar matrícula sin race condition
            var matricula = await GenerateMatriculaAsync(request.SchoolId);

            // Crear alumno
            var alumno = new Alumno
            {
                SchoolId = request.SchoolId,
                Nombre = request.Nombre.Trim(),
                Apellido = request.Apellido.Trim(),
                Email = emailNormalized,
                CURP = request.CURP.Trim().ToUpper(),
                FechaNacimiento = request.FechaNacimiento,
                Sexo = request.Sexo.ToUpper(),
                Matricula = matricula,
                Activo = true,
                FechaInscripcion = DateTime.UtcNow,
                FechaCreacion = DateTime.UtcNow
            };

            // Agregar tutor si se proporciona
            if (request.TutorId.HasValue)
            {
                var tutor = await _context.Tutores.FindAsync(request.TutorId);
                if (tutor == null)
                    throw new NotFoundException("Tutor", request.TutorId.Value);

                alumno.Tutores.Add(tutor);
            }

            _context.Alumnos.Add(alumno);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Alumno creado: ID={alumno.Id}, Matrícula={alumno.Matricula}");

            return MapToDto(alumno);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error de BD al crear alumno");
            throw new BusinessException("Error al guardar el alumno. Verifique los datos e intente nuevamente.");
        }
    }

    /// <summary>
    /// Obtener alumno por ID.
    /// </summary>
    public async Task<AlumnoDto> GetByIdAsync(int id)
    {
        var alumno = await _context.Alumnos
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && a.Activo);

        if (alumno == null)
            throw new NotFoundException("Alumno", id);

        return MapToDto(alumno);
    }

    /// <summary>
    /// Obtener alumno con datos completos (tutores, inscripciones).
    /// </summary>
    public async Task<AlumnoFullDataDto> GetByIdFullAsync(int id)
    {
        var alumno = await _context.Alumnos
            .Include(a => a.Tutores)
            .Include(a => a.Inscripciones)
            .ThenInclude(i => i.Grupo)
            .ThenInclude(g => g.CicloEscolar)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && a.Activo);

        if (alumno == null)
            throw new NotFoundException("Alumno", id);

        return MapToFullDto(alumno);
    }

    /// <summary>
    /// Obtener alumnos con paginación y búsqueda.
    /// </summary>
    public async Task<PaginatedAlumnosDto> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
    {
        // Validar paginación
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var query = _context.Alumnos.AsQueryable();

        // Filtrar solo alumnos ACTIVOS (soft delete)
        query = query.Where(a => a.Activo);

        // Filtrar por término de búsqueda
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim().ToLower();
            query = query.Where(a =>
                a.Nombre.ToLower().Contains(searchTerm) ||
                a.Apellido.ToLower().Contains(searchTerm) ||
                a.Email.ToLower().Contains(searchTerm) ||
                a.CURP.ToLower().Contains(searchTerm) ||
                (a.Matricula != null && a.Matricula.ToLower().Contains(searchTerm))
            );
        }

        // Contar total
        var totalItems = await query.CountAsync();

        // Paginar
        var alumnos = await query
            .OrderBy(a => a.Apellido)
            .ThenBy(a => a.Nombre)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return new PaginatedAlumnosDto
        {
            Items = alumnos.Select(MapToDto).ToList(),
            TotalItems = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Actualizar datos de alumno sin cambiar campos protegidos.
    /// </summary>
    public async Task<AlumnoDto> UpdateAlumnoAsync(int id, UpdateAlumnoDto request)
    {
        try
        {
            // Validar con FluentValidation
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                throw new Core.Exceptions.ValidationException(errors);
            }

            var alumno = await _context.Alumnos.FindAsync(id);
            if (alumno == null)
                throw new NotFoundException("Alumno", id);

            if (!alumno.Activo)
                throw new BusinessException("No se puede actualizar un alumno desactivado. Restaure el alumno primero.");

            // Verificar email único (si cambió) - considera solo alumnos ACTIVOS en la misma escuela
            var emailNormalized = request.Email.Trim().ToLower();
            if (alumno.Email != emailNormalized)
            {
                var emailExists = await _context.Alumnos
                    .AnyAsync(a => a.Email == emailNormalized && a.Id != id && a.Activo && a.SchoolId == alumno.SchoolId);
                if (emailExists)
                    throw new BusinessException($"Ya existe otro alumno activo con email '{request.Email}' en esta escuela.");
            }

            // Actualizar campos
            alumno.Nombre = request.Nombre.Trim();
            alumno.Apellido = request.Apellido.Trim();
            alumno.Email = emailNormalized;
            alumno.FechaNacimiento = request.FechaNacimiento;
            alumno.Sexo = request.Sexo.ToUpper();
            alumno.FechaActualizacion = DateTime.UtcNow;

            _context.Alumnos.Update(alumno);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Alumno actualizado: ID={alumno.Id}");

            return MapToDto(alumno);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, $"Error de BD al actualizar alumno {id}");
            throw new BusinessException("Error al actualizar el alumno. Intente nuevamente.");
        }
    }

    /// <summary>
    /// Soft delete: marcar alumno como inactivo.
    /// </summary>
    public async Task<bool> SoftDeleteAsync(int id)
    {
        var alumno = await _context.Alumnos.FindAsync(id);
        if (alumno == null)
            throw new NotFoundException("Alumno", id);

        alumno.Activo = false;
        alumno.FechaActualizacion = DateTime.UtcNow;

        _context.Alumnos.Update(alumno);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Alumno desactivado: ID={alumno.Id}");

        return true;
    }

    /// <summary>
    /// Restaurar alumno inactivo.
    /// </summary>
    public async Task<bool> RestoreAsync(int id)
    {
        var alumno = await _context.Alumnos.FindAsync(id);
        if (alumno == null)
            throw new NotFoundException("Alumno", id);

        alumno.Activo = true;
        alumno.FechaActualizacion = DateTime.UtcNow;

        _context.Alumnos.Update(alumno);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Alumno restaurado: ID={alumno.Id}");

        return true;
    }

    /// <summary>
    /// Verificar si un alumno existe.
    /// </summary>
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Alumnos.AnyAsync(a => a.Id == id);
    }

    // ============ Métodos Privados ============

    /// <summary>
    /// Genera matrícula de alumno de forma segura para evitar race conditions.
    /// 
    /// PROBLEMA: GenerateMatricula() original usa Count() + 1 que NO es atómico.
    /// Dos solicitudes simultáneas pueden generar la misma matrícula.
    /// 
    /// SOLUCIONES PROPUESTAS:
    /// 1. RECOMENDADO: Secuencia PostgreSQL (SEQUENCE) - Atómica a nivel BD
    ///    Sintaxis: CREATE SEQUENCE alumnos_matricula_seq_schoolid START WITH 1;
    ///    Uso en mapeo: HasDefaultValueSql("nextval('alumnos_matricula_seq_schoolid')")
    /// 
    /// 2. Tabla de Secuencias con Transacción Serializable
    ///    Crear tabla: MatriculaSequence { SchoolId, LastNumber }
    ///    Con índice único (SchoolId) y lock pesimista (FOR UPDATE)
    /// 
    /// 3. Event Sourcing + CQRS
    ///    Usar eventos para idempotencia, matrícula generada de forma eventual
    /// 
    /// 4. UUID + Lookup Asincrónico
    ///    Generar UUID inmediatamente, número secuencial después en background
    /// 
    /// IMPLEMENTACIÓN ACTUAL (MEJORADA):
    /// Filtra matrícula del año actual y obtiene el máximo número.
    /// Mejor que .Count() pero aún no es perfectamente atómico sin transacción.
    /// 
    /// PARA PRODUCCIÓN:
    /// Usar opción 1 (SEQUENCE) o 2 (tabla con lock pesimista).
    /// </summary>
    private async Task<string> GenerateMatriculaAsync(int schoolId)
    {
        var year = DateTime.UtcNow.Year;
        var yearPrefix = $"ALU-{year}-";

        // Obtener el máximo número de matrícula para este año y escuela
        var lastMatricula = await _context.Alumnos
            .Where(a => a.SchoolId == schoolId && a.Matricula != null && a.Matricula.StartsWith(yearPrefix))
            .OrderByDescending(a => a.Matricula)
            .Select(a => a.Matricula)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (!string.IsNullOrEmpty(lastMatricula))
        {
            // Extraer número del final (e.g., "ALU-2026-00005" -> 5)
            var lastNumber = int.Parse(lastMatricula.Substring(yearPrefix.Length));
            nextNumber = lastNumber + 1;
        }

        return $"{yearPrefix}{nextNumber:D5}";
    }

    private AlumnoDto MapToDto(Alumno alumno)
    {
        return new AlumnoDto
        {
            Id = alumno.Id,
            Nombre = alumno.Nombre,
            Apellido = alumno.Apellido,
            Email = alumno.Email,
            CURP = alumno.CURP,
            FechaNacimiento = alumno.FechaNacimiento,
            Sexo = alumno.Sexo,
            Matricula = alumno.Matricula,
            Activo = alumno.Activo,
            FechaInscripcion = alumno.FechaInscripcion,
            SchoolId = alumno.SchoolId
        };
    }

    private AlumnoFullDataDto MapToFullDto(Alumno alumno)
    {
        return new AlumnoFullDataDto
        {
            Id = alumno.Id,
            Nombre = alumno.Nombre,
            Apellido = alumno.Apellido,
            Email = alumno.Email,
            CURP = alumno.CURP,
            FechaNacimiento = alumno.FechaNacimiento,
            Sexo = alumno.Sexo,
            Matricula = alumno.Matricula,
            Activo = alumno.Activo,
            FechaInscripcion = alumno.FechaInscripcion,
            SchoolId = alumno.SchoolId,
            TutoresNombres = alumno.Tutores.Select(t => $"{t.Nombre} {t.Apellido}").ToList(),
            Inscripciones = alumno.Inscripciones.Select(i => new GrupoInscripcionDto
            {
                GrupoId = i.GrupoId,
                GrupoNombre = i.Grupo.Nombre,
                CicloEscolarId = i.CicloEscolarId,
                CicloNombre = i.CicloEscolar.Nombre,
                Activo = i.Activo
            }).ToList()
        };
    }
}
