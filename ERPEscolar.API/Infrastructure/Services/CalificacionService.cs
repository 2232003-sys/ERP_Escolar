using AutoMapper;
using ERPEscolar.API.Core.Exceptions;
using ERPEscolar.API.Data;
using ERPEscolar.API.DTOs.ControlEscolar;
using ERPEscolar.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ERPEscolar.API.Infrastructure.Services;

/// <summary>
/// Servicio de calificaciones
/// </summary>
public class CalificacionService : ICalificacionService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CalificacionService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CalificacionDto> RegisterCalificacionAsync(CreateCalificacionDto request)
    {
        // Validar que el alumno existe
        var alumno = await _context.Alumnos.FindAsync(request.AlumnoId);
        if (alumno == null)
            throw new NotFoundException($"Alumno con ID {request.AlumnoId} no encontrado");

        // Validar que el grupo-materia existe
        var grupoMateria = await _context.GrupoMaterias
            .Include(gm => gm.Grupo)
            .Include(gm => gm.Materia)
            .FirstOrDefaultAsync(gm => gm.Id == request.GrupoMateriaId);
        if (grupoMateria == null)
            throw new NotFoundException($"Grupo-Materia con ID {request.GrupoMateriaId} no encontrado");

        // Validar que el período existe
        var periodo = await _context.PeriodosCalificacion.FindAsync(request.PeriodoCalificacionId);
        if (periodo == null)
            throw new NotFoundException($"Período de calificación con ID {request.PeriodoCalificacionId} no encontrado");

        // Validar que el alumno esté inscrito en el grupo
        var inscripcion = await _context.Inscripciones
            .FirstOrDefaultAsync(i => i.AlumnoId == request.AlumnoId && i.GrupoId == grupoMateria.GrupoId && i.Activo);
        if (inscripcion == null)
            throw new BusinessException("El alumno no está inscrito en este grupo");

        // Validar que no exista ya una calificación para esta combinación
        var calificacionExistente = await _context.Calificaciones
            .FirstOrDefaultAsync(c => c.AlumnoId == request.AlumnoId &&
                                    c.GrupoMateriaId == request.GrupoMateriaId &&
                                    c.PeriodoCalificacionId == request.PeriodoCalificacionId);
        if (calificacionExistente != null)
            throw new BusinessException("Ya existe una calificación para este alumno, materia y período");

        // Validar docente si se proporciona
        if (request.DocenteQueQualificaId.HasValue)
        {
            var docente = await _context.Docentes.FindAsync(request.DocenteQueQualificaId.Value);
            if (docente == null)
                throw new NotFoundException($"Docente con ID {request.DocenteQueQualificaId.Value} no encontrado");
        }

        // Crear la calificación
        var calificacion = _mapper.Map<Calificacion>(request);
        calificacion.FechaCalificacion = DateTime.UtcNow;

        _context.Calificaciones.Add(calificacion);
        await _context.SaveChangesAsync();

        // Cargar datos relacionados para el DTO de respuesta
        calificacion = await _context.Calificaciones
            .Include(c => c.Alumno)
            .Include(c => c.GrupoMateria)
                .ThenInclude(gm => gm.Materia)
            .Include(c => c.PeriodoCalificacion)
            .Include(c => c.DocenteQueQualifica)
            .FirstOrDefaultAsync(c => c.Id == calificacion.Id);

        return _mapper.Map<CalificacionDto>(calificacion);
    }

    public async Task<CalificacionDto> UpdateCalificacionAsync(int id, UpdateCalificacionDto request)
    {
        var calificacion = await _context.Calificaciones
            .Include(c => c.Alumno)
            .Include(c => c.GrupoMateria)
                .ThenInclude(gm => gm.Materia)
            .Include(c => c.PeriodoCalificacion)
            .Include(c => c.DocenteQueQualifica)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (calificacion == null)
            throw new NotFoundException($"Calificación con ID {id} no encontrada");

        // Actualizar campos
        calificacion.Calificacion1 = request.Calificacion1;
        calificacion.Observacion = request.Observacion;
        calificacion.Aprobado = request.Calificacion1 >= 6.0m;
        calificacion.FechaCalificacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<CalificacionDto>(calificacion);
    }

    public async Task<CalificacionDto> GetByIdAsync(int id)
    {
        var calificacion = await _context.Calificaciones
            .Include(c => c.Alumno)
            .Include(c => c.GrupoMateria)
                .ThenInclude(gm => gm.Materia)
            .Include(c => c.PeriodoCalificacion)
            .Include(c => c.DocenteQueQualifica)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (calificacion == null)
            throw new NotFoundException($"Calificación con ID {id} no encontrada");

        return _mapper.Map<CalificacionDto>(calificacion);
    }

    public async Task<GrupoCalificacionesDto> GetCalificacionesByGrupoAsync(int grupoMateriaId, int periodoCalificacionId)
    {
        // Validar que el grupo-materia existe
        var grupoMateria = await _context.GrupoMaterias
            .Include(gm => gm.Grupo)
            .Include(gm => gm.Materia)
            .FirstOrDefaultAsync(gm => gm.Id == grupoMateriaId);
        if (grupoMateria == null)
            throw new NotFoundException($"Grupo-Materia con ID {grupoMateriaId} no encontrado");

        // Validar que el período existe
        var periodo = await _context.PeriodosCalificacion.FindAsync(periodoCalificacionId);
        if (periodo == null)
            throw new NotFoundException($"Período de calificación con ID {periodoCalificacionId} no encontrado");

        // Obtener calificaciones del grupo
        var calificaciones = await _context.Calificaciones
            .Include(c => c.Alumno)
            .Include(c => c.GrupoMateria)
                .ThenInclude(gm => gm.Materia)
            .Include(c => c.PeriodoCalificacion)
            .Include(c => c.DocenteQueQualifica)
            .Where(c => c.GrupoMateriaId == grupoMateriaId && c.PeriodoCalificacionId == periodoCalificacionId)
            .ToListAsync();

        var calificacionesDto = _mapper.Map<List<CalificacionDto>>(calificaciones);

        return new GrupoCalificacionesDto
        {
            GrupoMateriaId = grupoMateriaId,
            GrupoNombre = grupoMateria.Grupo.Nombre,
            MateriaNombre = grupoMateria.Materia.Nombre,
            PeriodoNombre = periodo.Nombre,
            Calificaciones = calificacionesDto
        };
    }

    public async Task<ExpedienteAcademicoDto> GetExpedienteAcademicoAsync(int alumnoId)
    {
        // Validar que el alumno existe
        var alumno = await _context.Alumnos.FindAsync(alumnoId);
        if (alumno == null)
            throw new NotFoundException($"Alumno con ID {alumnoId} no encontrado");

        // Obtener todas las calificaciones del alumno
        var calificaciones = await _context.Calificaciones
            .Include(c => c.GrupoMateria)
                .ThenInclude(gm => gm.Materia)
            .Include(c => c.PeriodoCalificacion)
            .Where(c => c.AlumnoId == alumnoId)
            .ToListAsync();

        // Agrupar por materia
        var materiasAgrupadas = calificaciones
            .GroupBy(c => c.GrupoMateria.Materia)
            .Select(g => new MateriaCalificacionesDto
            {
                MateriaId = g.Key.Id,
                MateriaNombre = g.Key.Nombre,
                Calificaciones = _mapper.Map<List<CalificacionDto>>(g.ToList()),
                PromedioMateria = g.Average(c => c.Calificacion1)
            })
            .ToList();

        // Calcular promedio general
        var promedioGeneral = calificaciones.Any() ? calificaciones.Average(c => c.Calificacion1) : 0;

        return new ExpedienteAcademicoDto
        {
            AlumnoId = alumnoId,
            AlumnoNombre = $"{alumno.Nombre} {alumno.Apellido}",
            Matricula = alumno.Matricula,
            Materias = materiasAgrupadas,
            PromedioGeneral = promedioGeneral
        };
    }

    public async Task<BoletaDto> GenerateBoletaAsync(int alumnoId, int periodoCalificacionId)
    {
        // Validar que el alumno existe
        var alumno = await _context.Alumnos.FindAsync(alumnoId);
        if (alumno == null)
            throw new NotFoundException($"Alumno con ID {alumnoId} no encontrado");

        // Validar que el período existe
        var periodo = await _context.PeriodosCalificacion.FindAsync(periodoCalificacionId);
        if (periodo == null)
            throw new NotFoundException($"Período de calificación con ID {periodoCalificacionId} no encontrado");

        // Obtener calificaciones del período
        var calificaciones = await _context.Calificaciones
            .Include(c => c.GrupoMateria)
                .ThenInclude(gm => gm.Materia)
            .Where(c => c.AlumnoId == alumnoId && c.PeriodoCalificacionId == periodoCalificacionId)
            .ToListAsync();

        var materiasBoleta = calificaciones.Select(c => new BoletaMateriaDto
        {
            MateriaNombre = c.GrupoMateria.Materia.Nombre,
            Calificacion = c.Calificacion1,
            Aprobado = c.Aprobado,
            Observacion = c.Observacion
        }).ToList();

        var promedioPeriodo = calificaciones.Any() ? calificaciones.Average(c => c.Calificacion1) : 0;
        var aprobado = promedioPeriodo >= 6.0m;

        return new BoletaDto
        {
            AlumnoId = alumnoId,
            AlumnoNombre = $"{alumno.Nombre} {alumno.Apellido}",
            Matricula = alumno.Matricula,
            PeriodoNombre = periodo.Nombre,
            FechaGeneracion = DateTime.UtcNow,
            Materias = materiasBoleta,
            PromedioPeriodo = promedioPeriodo,
            Aprobado = aprobado
        };
    }

    public async Task<decimal> CalculatePromedioMateriaAsync(int alumnoId, int materiaId)
    {
        var calificaciones = await _context.Calificaciones
            .Include(c => c.GrupoMateria)
            .Where(c => c.AlumnoId == alumnoId && c.GrupoMateria.MateriaId == materiaId)
            .Select(c => c.Calificacion1)
            .ToListAsync();

        return calificaciones.Any() ? calificaciones.Average() : 0;
    }

    public async Task<decimal> CalculatePromedioFinalAsync(int alumnoId)
    {
        var calificaciones = await _context.Calificaciones
            .Where(c => c.AlumnoId == alumnoId)
            .Select(c => c.Calificacion1)
            .ToListAsync();

        return calificaciones.Any() ? calificaciones.Average() : 0;
    }

    public async Task<bool> ValidarCierrePeriodoAsync(int periodoCalificacionId)
    {
        // Validar que el período existe
        var periodo = await _context.PeriodosCalificacion.FindAsync(periodoCalificacionId);
        if (periodo == null)
            throw new NotFoundException($"Período de calificación con ID {periodoCalificacionId} no encontrado");

        // Verificar que todas las calificaciones del período estén registradas
        // (Esta es una validación básica - en un sistema real sería más compleja)
        var totalAlumnosInscritos = await _context.Inscripciones
            .Where(i => i.CicloEscolarId == periodo.CicloEscolarId && i.Activo)
            .CountAsync();

        var totalCalificacionesRegistradas = await _context.Calificaciones
            .Where(c => c.PeriodoCalificacionId == periodoCalificacionId)
            .CountAsync();

        // Para este ejemplo, consideramos que el período puede cerrarse si al menos el 80% de las calificaciones están registradas
        var porcentajeCompletado = totalAlumnosInscritos > 0 ? (decimal)totalCalificacionesRegistradas / totalAlumnosInscritos : 0;

        return porcentajeCompletado >= 0.8m;
    }

    public async Task<PaginatedCalificacionesDto> GetAllAsync(int pageNumber = 1, int pageSize = 10, int? alumnoId = null, int? grupoMateriaId = null, int? periodoId = null)
    {
        var query = _context.Calificaciones
            .Include(c => c.Alumno)
            .Include(c => c.GrupoMateria)
                .ThenInclude(gm => gm.Materia)
            .Include(c => c.PeriodoCalificacion)
            .Include(c => c.DocenteQueQualifica)
            .AsQueryable();

        // Aplicar filtros
        if (alumnoId.HasValue)
            query = query.Where(c => c.AlumnoId == alumnoId.Value);

        if (grupoMateriaId.HasValue)
            query = query.Where(c => c.GrupoMateriaId == grupoMateriaId.Value);

        if (periodoId.HasValue)
            query = query.Where(c => c.PeriodoCalificacionId == periodoId.Value);

        // Obtener total de registros
        var totalItems = await query.CountAsync();

        // Aplicar paginación
        var items = await query
            .OrderByDescending(c => c.FechaCalificacion)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var itemsDto = _mapper.Map<List<CalificacionDto>>(items);

        return new PaginatedCalificacionesDto
        {
            Items = itemsDto,
            TotalItems = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }
}