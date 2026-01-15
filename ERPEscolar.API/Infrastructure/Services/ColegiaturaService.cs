using ERPEscolar.API.Data;
using ERPEscolar.API.Models;
using ERPEscolar.API.DTOs.Finanzas;
using ERPEscolar.API.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace ERPEscolar.API.Infrastructure.Services;

/// <summary>
/// Servicio de gestión de colegiaturas con validaciones de negocio.
/// </summary>
public interface IColegiaturaService
{
    Task<ColegiaturaDto> CreateColegiaturaAsync(CreateColegiaturaDto request);
    Task<ColegiaturaDto> GetByIdAsync(int id);
    Task<ColegiaturaFullDataDto> GetByIdFullAsync(int id);
    Task<PaginatedColegiaturasDto> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null);
    Task<ColegiaturaDto> UpdateColegiaturaAsync(int id, UpdateColegiaturaDto request);
    Task<bool> SoftDeleteAsync(int id);
    Task<bool> RestoreAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<List<ColegiaturaDto>> GetColegiaturasByAlumnoAsync(int alumnoId);
    Task<List<ColegiaturaDto>> GetColegiaturasPendientesAsync(int alumnoId);
}

public class ColegiaturaService : IColegiaturaService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ColegiaturaService> _logger;
    private readonly IValidator<CreateColegiaturaDto> _createValidator;
    private readonly IValidator<UpdateColegiaturaDto> _updateValidator;

    public ColegiaturaService(
        AppDbContext context,
        ILogger<ColegiaturaService> logger,
        IValidator<CreateColegiaturaDto> createValidator,
        IValidator<UpdateColegiaturaDto> updateValidator)
    {
        _context = context;
        _logger = logger;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>
    /// Crear una nueva colegiatura con validaciones.
    /// </summary>
    public async Task<ColegiaturaDto> CreateColegiaturaAsync(CreateColegiaturaDto request)
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

            // Verificar que el alumno existe y está activo
            var alumno = await _context.Alumnos
                .FirstOrDefaultAsync(a => a.Id == request.AlumnoId && a.Activo);
            if (alumno == null)
                throw new NotFoundException("Alumno", request.AlumnoId);

            // Verificar que el concepto de cobro existe y está activo
            var conceptoCobro = await _context.ConceptosCobro
                .FirstOrDefaultAsync(c => c.Id == request.ConceptoCobroId && c.Activo);
            if (conceptoCobro == null)
                throw new NotFoundException("ConceptoCobro", request.ConceptoCobroId);

            // Verificar que el ciclo escolar existe y está activo
            var cicloEscolar = await _context.CiclosEscolares
                .FirstOrDefaultAsync(c => c.Id == request.CicloEscolarId && c.Activo);
            if (cicloEscolar == null)
                throw new NotFoundException("CicloEscolar", request.CicloEscolarId);

            // Verificar que no exista una colegiatura duplicada para el mismo alumno, concepto, ciclo y mes
            var colegiaturaExists = await _context.Cargos
                .AnyAsync(c => c.AlumnoId == request.AlumnoId &&
                              c.ConceptoCobroId == request.ConceptoCobroId &&
                              c.CicloEscolarId == request.CicloEscolarId &&
                              c.Mes == request.Mes &&
                              c.Activo);
            if (colegiaturaExists)
                throw new BusinessException($"Ya existe una colegiatura activa para el alumno, concepto, ciclo y mes especificados.");

            // Generar folio único
            var folio = await GenerateFolioAsync();

            // Calcular subtotal y total
            var subtotal = request.Monto - request.Descuento + request.Recargo;
            var total = subtotal * (1 + request.IVA);

            // Crear colegiatura
            var colegiatura = new Cargo
            {
                AlumnoId = request.AlumnoId,
                ConceptoCobroId = request.ConceptoCobroId,
                CicloEscolarId = request.CicloEscolarId,
                Folio = folio,
                Mes = request.Mes,
                Monto = request.Monto,
                Descuento = request.Descuento,
                Recargo = request.Recargo,
                IVA = request.IVA,
                Estado = "Pendiente",
                MontoRecibido = 0,
                FechaEmision = DateTime.UtcNow,
                FechaVencimiento = request.FechaVencimiento,
                Observacion = request.Observacion?.Trim(),
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Cargos.Add(colegiatura);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Colegiatura creada: ID={colegiatura.Id}, Folio={colegiatura.Folio}, Total={colegiatura.Total:C}");

            return MapToDto(colegiatura);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error de BD al crear colegiatura");
            throw new BusinessException("Error al guardar la colegiatura. Verifique los datos e intente nuevamente.");
        }
    }

    /// <summary>
    /// Obtener colegiatura por ID.
    /// </summary>
    public async Task<ColegiaturaDto> GetByIdAsync(int id)
    {
        var colegiatura = await _context.Cargos
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.Activo);

        if (colegiatura == null)
            throw new NotFoundException("Colegiatura", id);

        return MapToDto(colegiatura);
    }

    /// <summary>
    /// Obtener colegiatura con datos completos (relaciones incluidas).
    /// </summary>
    public async Task<ColegiaturaFullDataDto> GetByIdFullAsync(int id)
    {
        var colegiatura = await _context.Cargos
            .Include(c => c.Alumno)
            .Include(c => c.ConceptoCobro)
            .Include(c => c.CicloEscolar)
            .Include(c => c.Pagos.Where(p => p.Activo))
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.Activo);

        if (colegiatura == null)
            throw new NotFoundException("Colegiatura", id);

        return MapToFullDto(colegiatura);
    }

    /// <summary>
    /// Obtener todas las colegiaturas con paginación y búsqueda.
    /// </summary>
    public async Task<PaginatedColegiaturasDto> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
    {
        var query = _context.Cargos
            .Include(c => c.Alumno)
            .Include(c => c.ConceptoCobro)
            .Include(c => c.CicloEscolar)
            .Where(c => c.Activo);

        // Aplicar búsqueda si se proporciona
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim().ToLower();
            query = query.Where(c =>
                c.Folio.ToLower().Contains(search) ||
                c.Alumno!.Nombre.ToLower().Contains(search) ||
                c.Alumno!.Apellido.ToLower().Contains(search) ||
                c.ConceptoCobro!.Nombre.ToLower().Contains(search) ||
                c.Estado.ToLower().Contains(search));
        }

        var totalRecords = await query.CountAsync();

        var colegiaturas = await query
            .OrderByDescending(c => c.FechaEmision)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return new PaginatedColegiaturasDto
        {
            Items = colegiaturas.Select(MapToDto).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords
        };
    }

    /// <summary>
    /// Actualizar una colegiatura.
    /// </summary>
    public async Task<ColegiaturaDto> UpdateColegiaturaAsync(int id, UpdateColegiaturaDto request)
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

            var colegiatura = await _context.Cargos.FindAsync(id);
            if (colegiatura == null || !colegiatura.Activo)
                throw new NotFoundException("Colegiatura", id);

            // Validaciones de negocio para actualización
            if (request.Estado == "Pagado" && colegiatura.MontoRecibido < colegiatura.Total)
                throw new BusinessException("No se puede marcar como pagado una colegiatura con monto recibido insuficiente.");

            if (request.Estado == "Cancelado" && colegiatura.MontoRecibido > 0)
                throw new BusinessException("No se puede cancelar una colegiatura que ya tiene pagos aplicados.");

            // Actualizar campos
            colegiatura.Estado = request.Estado;
            colegiatura.MontoRecibido = request.MontoRecibido;
            colegiatura.Observacion = request.Observacion?.Trim();

            // Si se marca como pagado y no tiene fecha de pago, asignar fecha actual
            if (request.Estado == "Pagado" && !colegiatura.FechaPago.HasValue)
                colegiatura.FechaPago = DateTime.UtcNow;
            else if (request.Estado != "Pagado")
                colegiatura.FechaPago = request.FechaPago;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Colegiatura actualizada: ID={colegiatura.Id}, Estado={colegiatura.Estado}");

            return MapToDto(colegiatura);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error de BD al actualizar colegiatura");
            throw new BusinessException("Error al actualizar la colegiatura. Verifique los datos e intente nuevamente.");
        }
    }

    /// <summary>
    /// Soft delete de colegiatura.
    /// </summary>
    public async Task<bool> SoftDeleteAsync(int id)
    {
        var colegiatura = await _context.Cargos.FindAsync(id);
        if (colegiatura == null)
            throw new NotFoundException("Colegiatura", id);

        if (!colegiatura.Activo)
            throw new BusinessException("La colegiatura ya está eliminada.");

        // Verificar que no tenga pagos aplicados
        var hasPagos = await _context.Pagos
            .AnyAsync(p => p.CargoId == id && p.Activo);
        if (hasPagos)
            throw new BusinessException("No se puede eliminar una colegiatura que tiene pagos aplicados.");

        colegiatura.Activo = false;
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Colegiatura eliminada: ID={id}");
        return true;
    }

    /// <summary>
    /// Restaurar colegiatura eliminada.
    /// </summary>
    public async Task<bool> RestoreAsync(int id)
    {
        var colegiatura = await _context.Cargos.FindAsync(id);
        if (colegiatura == null)
            throw new NotFoundException("Colegiatura", id);

        if (colegiatura.Activo)
            throw new BusinessException("La colegiatura ya está activa.");

        colegiatura.Activo = true;
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Colegiatura restaurada: ID={id}");
        return true;
    }

    /// <summary>
    /// Verificar si existe una colegiatura.
    /// </summary>
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Cargos.AnyAsync(c => c.Id == id && c.Activo);
    }

    /// <summary>
    /// Obtener colegiaturas de un alumno específico.
    /// </summary>
    public async Task<List<ColegiaturaDto>> GetColegiaturasByAlumnoAsync(int alumnoId)
    {
        // Verificar que el alumno existe
        var alumnoExists = await _context.Alumnos.AnyAsync(a => a.Id == alumnoId && a.Activo);
        if (!alumnoExists)
            throw new NotFoundException("Alumno", alumnoId);

        var colegiaturas = await _context.Cargos
            .Include(c => c.ConceptoCobro)
            .Include(c => c.CicloEscolar)
            .Where(c => c.AlumnoId == alumnoId && c.Activo)
            .OrderByDescending(c => c.FechaEmision)
            .AsNoTracking()
            .ToListAsync();

        return colegiaturas.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Obtener colegiaturas pendientes de un alumno.
    /// </summary>
    public async Task<List<ColegiaturaDto>> GetColegiaturasPendientesAsync(int alumnoId)
    {
        // Verificar que el alumno existe
        var alumnoExists = await _context.Alumnos.AnyAsync(a => a.Id == alumnoId && a.Activo);
        if (!alumnoExists)
            throw new NotFoundException("Alumno", alumnoId);

        var colegiaturas = await _context.Cargos
            .Include(c => c.ConceptoCobro)
            .Include(c => c.CicloEscolar)
            .Where(c => c.AlumnoId == alumnoId && c.Activo &&
                       (c.Estado == "Pendiente" || c.Estado == "Parcial"))
            .OrderBy(c => c.FechaVencimiento ?? c.FechaEmision)
            .AsNoTracking()
            .ToListAsync();

        return colegiaturas.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Generar folio único para colegiatura.
    /// </summary>
    private async Task<string> GenerateFolioAsync()
    {
        var date = DateTime.UtcNow;
        var baseFolio = $"COLEGIATURA-{date:yyyyMMdd}";
        var counter = 1;
        var folio = $"{baseFolio}-{counter:D4}";

        while (await _context.Cargos.AnyAsync(c => c.Folio == folio))
        {
            counter++;
            folio = $"{baseFolio}-{counter:D4}";
        }

        return folio;
    }

    /// <summary>
    /// Mapear entidad Cargo a DTO básico.
    /// </summary>
    private static ColegiaturaDto MapToDto(Cargo colegiatura)
    {
        return new ColegiaturaDto
        {
            Id = colegiatura.Id,
            AlumnoId = colegiatura.AlumnoId,
            ConceptoCobroId = colegiatura.ConceptoCobroId,
            CicloEscolarId = colegiatura.CicloEscolarId,
            Folio = colegiatura.Folio,
            Mes = colegiatura.Mes,
            Monto = colegiatura.Monto,
            Descuento = colegiatura.Descuento,
            Recargo = colegiatura.Recargo,
            Subtotal = colegiatura.Subtotal,
            IVA = colegiatura.IVA,
            Total = colegiatura.Total,
            Estado = colegiatura.Estado,
            MontoRecibido = colegiatura.MontoRecibido,
            FechaEmision = colegiatura.FechaEmision,
            FechaVencimiento = colegiatura.FechaVencimiento,
            FechaPago = colegiatura.FechaPago,
            Observacion = colegiatura.Observacion
        };
    }

    /// <summary>
    /// Mapear entidad Cargo a DTO completo.
    /// </summary>
    private static ColegiaturaFullDataDto MapToFullDto(Cargo colegiatura)
    {
        return new ColegiaturaFullDataDto
        {
            Id = colegiatura.Id,
            Alumno = colegiatura.Alumno != null ? new AlumnoDto
            {
                Id = colegiatura.Alumno.Id,
                NombreCompleto = $"{colegiatura.Alumno.Nombre} {colegiatura.Alumno.Apellido}".Trim(),
                Matricula = colegiatura.Alumno.Matricula,
                CURP = colegiatura.Alumno.CURP
            } : null,
            ConceptoCobro = colegiatura.ConceptoCobro != null ? new ConceptoCobroDto
            {
                Id = colegiatura.ConceptoCobro.Id,
                Nombre = colegiatura.ConceptoCobro.Nombre,
                Descripcion = colegiatura.ConceptoCobro.Descripcion,
                TipoConcepto = colegiatura.ConceptoCobro.TipoConcepto,
                MontoBase = colegiatura.ConceptoCobro.MontoBase
            } : null,
            CicloEscolar = colegiatura.CicloEscolar != null ? new CicloEscolarDto
            {
                Id = colegiatura.CicloEscolar.Id,
                Nombre = colegiatura.CicloEscolar.Nombre
            } : null,
            Folio = colegiatura.Folio,
            Mes = colegiatura.Mes,
            Monto = colegiatura.Monto,
            Descuento = colegiatura.Descuento,
            Recargo = colegiatura.Recargo,
            Subtotal = colegiatura.Subtotal,
            IVA = colegiatura.IVA,
            Total = colegiatura.Total,
            Estado = colegiatura.Estado,
            MontoRecibido = colegiatura.MontoRecibido,
            FechaEmision = colegiatura.FechaEmision,
            FechaVencimiento = colegiatura.FechaVencimiento,
            FechaPago = colegiatura.FechaPago,
            Observacion = colegiatura.Observacion,
            Pagos = colegiatura.Pagos?.Select(p => new PagoDto
            {
                Id = p.Id,
                Monto = p.Monto,
                Metodo = p.Metodo,
                Estado = p.Estado,
                FechaPago = p.FechaPago,
                ReferenciaExterna = p.ReferenciaExterna
            }).ToList() ?? []
        };
    }
}