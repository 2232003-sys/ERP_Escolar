using ERPEscolar.API.Data;
using ERPEscolar.API.Models;
using ERPEscolar.API.DTOs.Finanzas;
using ERPEscolar.API.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace ERPEscolar.API.Infrastructure.Services;

/// <summary>
/// Servicio de gestión de cargos con validaciones de negocio.
/// </summary>
public interface ICargoService
{
    Task<CargoDto> CreateCargoAsync(CreateCargoDto request);
    Task<CargoDto> GetByIdAsync(int id);
    Task<CargoFullDataDto> GetByIdFullAsync(int id);
    Task<PaginatedCargosDto> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null);
    Task<CargoDto> UpdateCargoAsync(int id, UpdateCargoDto request);
    Task<bool> SoftDeleteAsync(int id);
    Task<bool> RestoreAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<List<CargoDto>> GetCargosByAlumnoAsync(int alumnoId);
    Task<List<CargoDto>> GetCargosPendientesAsync(int alumnoId);
}

public class CargoService : ICargoService
{
    private readonly AppDbContext _context;
    private readonly ILogger<CargoService> _logger;
    private readonly IValidator<CreateCargoDto> _createValidator;
    private readonly IValidator<UpdateCargoDto> _updateValidator;

    public CargoService(
        AppDbContext context,
        ILogger<CargoService> logger,
        IValidator<CreateCargoDto> createValidator,
        IValidator<UpdateCargoDto> updateValidator)
    {
        _context = context;
        _logger = logger;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>
    /// Crear un nuevo cargo con validaciones.
    /// </summary>
    public async Task<CargoDto> CreateCargoAsync(CreateCargoDto request)
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

            // Verificar que no exista un cargo duplicado para el mismo alumno, concepto, ciclo y mes
            var cargoExists = await _context.Cargos
                .AnyAsync(c => c.AlumnoId == request.AlumnoId &&
                              c.ConceptoCobroId == request.ConceptoCobroId &&
                              c.CicloEscolarId == request.CicloEscolarId &&
                              c.Mes == request.Mes &&
                              c.Activo);
            if (cargoExists)
                throw new BusinessException($"Ya existe un cargo activo para el alumno, concepto, ciclo y mes especificados.");

            // Generar folio único
            var folio = await GenerateFolioAsync();

            // Calcular subtotal y total
            var subtotal = request.Monto - request.Descuento + request.Recargo;
            var total = subtotal * (1 + request.IVA);

            // Crear cargo
            var cargo = new Cargo
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

            _context.Cargos.Add(cargo);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Cargo creado: ID={cargo.Id}, Folio={cargo.Folio}, Total={cargo.Total:C}");

            return MapToDto(cargo);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error de BD al crear cargo");
            throw new BusinessException("Error al guardar el cargo. Verifique los datos e intente nuevamente.");
        }
    }

    /// <summary>
    /// Obtener cargo por ID.
    /// </summary>
    public async Task<CargoDto> GetByIdAsync(int id)
    {
        var cargo = await _context.Cargos
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.Activo);

        if (cargo == null)
            throw new NotFoundException("Cargo", id);

        return MapToDto(cargo);
    }

    /// <summary>
    /// Obtener cargo con datos completos (relaciones incluidas).
    /// </summary>
    public async Task<CargoFullDataDto> GetByIdFullAsync(int id)
    {
        var cargo = await _context.Cargos
            .Include(c => c.Alumno)
            .Include(c => c.ConceptoCobro)
            .Include(c => c.CicloEscolar)
            .Include(c => c.Pagos.Where(p => p.Activo))
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.Activo);

        if (cargo == null)
            throw new NotFoundException("Cargo", id);

        return MapToFullDto(cargo);
    }

    /// <summary>
    /// Obtener todos los cargos con paginación y búsqueda.
    /// </summary>
    public async Task<PaginatedCargosDto> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
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

        var cargos = await query
            .OrderByDescending(c => c.FechaEmision)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return new PaginatedCargosDto
        {
            Items = cargos.Select(MapToDto).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords
        };
    }

    /// <summary>
    /// Actualizar un cargo.
    /// </summary>
    public async Task<CargoDto> UpdateCargoAsync(int id, UpdateCargoDto request)
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

            var cargo = await _context.Cargos.FindAsync(id);
            if (cargo == null || !cargo.Activo)
                throw new NotFoundException("Cargo", id);

            // Validaciones de negocio para actualización
            if (request.Estado == "Pagado" && cargo.MontoRecibido < cargo.Total)
                throw new BusinessException("No se puede marcar como pagado un cargo con monto recibido insuficiente.");

            if (request.Estado == "Cancelado" && cargo.MontoRecibido > 0)
                throw new BusinessException("No se puede cancelar un cargo que ya tiene pagos aplicados.");

            // Actualizar campos
            cargo.Estado = request.Estado;
            cargo.MontoRecibido = request.MontoRecibido;
            cargo.Observacion = request.Observacion?.Trim();

            // Si se marca como pagado y no tiene fecha de pago, asignar fecha actual
            if (request.Estado == "Pagado" && !cargo.FechaPago.HasValue)
                cargo.FechaPago = DateTime.UtcNow;
            else if (request.Estado != "Pagado")
                cargo.FechaPago = request.FechaPago;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Cargo actualizado: ID={cargo.Id}, Estado={cargo.Estado}");

            return MapToDto(cargo);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error de BD al actualizar cargo");
            throw new BusinessException("Error al actualizar el cargo. Verifique los datos e intente nuevamente.");
        }
    }

    /// <summary>
    /// Soft delete de cargo.
    /// </summary>
    public async Task<bool> SoftDeleteAsync(int id)
    {
        var cargo = await _context.Cargos.FindAsync(id);
        if (cargo == null)
            throw new NotFoundException("Cargo", id);

        if (!cargo.Activo)
            throw new BusinessException("El cargo ya está eliminado.");

        // Verificar que no tenga pagos aplicados
        var hasPagos = await _context.Pagos
            .AnyAsync(p => p.CargoId == id && p.Activo);
        if (hasPagos)
            throw new BusinessException("No se puede eliminar un cargo que tiene pagos aplicados.");

        cargo.Activo = false;
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Cargo eliminado: ID={id}");
        return true;
    }

    /// <summary>
    /// Restaurar cargo eliminado.
    /// </summary>
    public async Task<bool> RestoreAsync(int id)
    {
        var cargo = await _context.Cargos.FindAsync(id);
        if (cargo == null)
            throw new NotFoundException("Cargo", id);

        if (cargo.Activo)
            throw new BusinessException("El cargo ya está activo.");

        cargo.Activo = true;
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Cargo restaurado: ID={id}");
        return true;
    }

    /// <summary>
    /// Verificar si existe un cargo.
    /// </summary>
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Cargos.AnyAsync(c => c.Id == id && c.Activo);
    }

    /// <summary>
    /// Obtener cargos de un alumno específico.
    /// </summary>
    public async Task<List<CargoDto>> GetCargosByAlumnoAsync(int alumnoId)
    {
        // Verificar que el alumno existe
        var alumnoExists = await _context.Alumnos.AnyAsync(a => a.Id == alumnoId && a.Activo);
        if (!alumnoExists)
            throw new NotFoundException("Alumno", alumnoId);

        var cargos = await _context.Cargos
            .Include(c => c.ConceptoCobro)
            .Include(c => c.CicloEscolar)
            .Where(c => c.AlumnoId == alumnoId && c.Activo)
            .OrderByDescending(c => c.FechaEmision)
            .AsNoTracking()
            .ToListAsync();

        return cargos.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Obtener cargos pendientes de un alumno.
    /// </summary>
    public async Task<List<CargoDto>> GetCargosPendientesAsync(int alumnoId)
    {
        // Verificar que el alumno existe
        var alumnoExists = await _context.Alumnos.AnyAsync(a => a.Id == alumnoId && a.Activo);
        if (!alumnoExists)
            throw new NotFoundException("Alumno", alumnoId);

        var cargos = await _context.Cargos
            .Include(c => c.ConceptoCobro)
            .Include(c => c.CicloEscolar)
            .Where(c => c.AlumnoId == alumnoId && c.Activo &&
                       (c.Estado == "Pendiente" || c.Estado == "Parcial"))
            .OrderBy(c => c.FechaVencimiento ?? c.FechaEmision)
            .AsNoTracking()
            .ToListAsync();

        return cargos.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Generar folio único para cargo.
    /// </summary>
    private async Task<string> GenerateFolioAsync()
    {
        var date = DateTime.UtcNow;
        var baseFolio = $"CARGO-{date:yyyyMMdd}";
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
    private static CargoDto MapToDto(Cargo cargo)
    {
        return new CargoDto
        {
            Id = cargo.Id,
            AlumnoId = cargo.AlumnoId,
            ConceptoCobroId = cargo.ConceptoCobroId,
            CicloEscolarId = cargo.CicloEscolarId,
            Folio = cargo.Folio,
            Mes = cargo.Mes,
            Monto = cargo.Monto,
            Descuento = cargo.Descuento,
            Recargo = cargo.Recargo,
            Subtotal = cargo.Subtotal,
            IVA = cargo.IVA,
            Total = cargo.Total,
            Estado = cargo.Estado,
            MontoRecibido = cargo.MontoRecibido,
            FechaEmision = cargo.FechaEmision,
            FechaVencimiento = cargo.FechaVencimiento,
            FechaPago = cargo.FechaPago,
            Observacion = cargo.Observacion
        };
    }

    /// <summary>
    /// Mapear entidad Cargo a DTO completo.
    /// </summary>
    private static CargoFullDataDto MapToFullDto(Cargo cargo)
    {
        return new CargoFullDataDto
        {
            Id = cargo.Id,
            Alumno = cargo.Alumno != null ? new AlumnoDto
            {
                Id = cargo.Alumno.Id,
                NombreCompleto = $"{cargo.Alumno.Nombre} {cargo.Alumno.Apellido}".Trim(),
                Matricula = cargo.Alumno.Matricula,
                CURP = cargo.Alumno.CURP
            } : null,
            ConceptoCobro = cargo.ConceptoCobro != null ? new ConceptoCobroDto
            {
                Id = cargo.ConceptoCobro.Id,
                Nombre = cargo.ConceptoCobro.Nombre,
                Descripcion = cargo.ConceptoCobro.Descripcion,
                TipoConcepto = cargo.ConceptoCobro.TipoConcepto,
                MontoBase = cargo.ConceptoCobro.MontoBase
            } : null,
            CicloEscolar = cargo.CicloEscolar != null ? new CicloEscolarDto
            {
                Id = cargo.CicloEscolar.Id,
                Nombre = cargo.CicloEscolar.Nombre
            } : null,
            Folio = cargo.Folio,
            Mes = cargo.Mes,
            Monto = cargo.Monto,
            Descuento = cargo.Descuento,
            Recargo = cargo.Recargo,
            Subtotal = cargo.Subtotal,
            IVA = cargo.IVA,
            Total = cargo.Total,
            Estado = cargo.Estado,
            MontoRecibido = cargo.MontoRecibido,
            FechaEmision = cargo.FechaEmision,
            FechaVencimiento = cargo.FechaVencimiento,
            FechaPago = cargo.FechaPago,
            Observacion = cargo.Observacion,
            Pagos = cargo.Pagos?.Select(p => new PagoDto
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
