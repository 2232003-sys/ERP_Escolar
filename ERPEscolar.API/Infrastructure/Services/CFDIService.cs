using ERPEscolar.API.Data;
using ERPEscolar.API.Models;
using ERPEscolar.API.DTOs.Fiscal;
using ERPEscolar.API.DTOs.Finanzas;
using ERPEscolar.API.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace ERPEscolar.API.Infrastructure.Services;

/// <summary>
/// Servicio de gestión de CFDI con validaciones fiscales.
/// </summary>
public interface ICFDIService
{
    Task<CFDIDto> CreateCFDIAsync(CreateCFDIDto request);
    Task<CFDIDto> GetByIdAsync(int id);
    Task<CFDIFullDataDto> GetByIdFullAsync(int id);
    Task<PaginatedCFDISDto> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null);
    Task<CFDIDto> UpdateCFDIAsync(int id, UpdateCFDIDto request);
    Task<TimbradoResponseDto> TimbrarCFDIAsync(TimbrarCFDIDto request);
    Task<TimbradoResponseDto> CancelarCFDIAsync(CancelarCFDIDto request);
    Task<bool> ExistsAsync(int id);
    Task<List<CFDIDto>> GetCFDISByCargoAsync(int cargoId);
    Task<List<CFDIDto>> GetCFDISByEstadoAsync(string estado);
}

public class CFDIService : ICFDIService
{
    private readonly AppDbContext _context;
    private readonly ILogger<CFDIService> _logger;
    private readonly IValidator<CreateCFDIDto> _createValidator;
    private readonly IValidator<UpdateCFDIDto> _updateValidator;
    private readonly IValidator<TimbrarCFDIDto> _timbrarValidator;
    private readonly IValidator<CancelarCFDIDto> _cancelarValidator;

    public CFDIService(
        AppDbContext context,
        ILogger<CFDIService> logger,
        IValidator<CreateCFDIDto> createValidator,
        IValidator<UpdateCFDIDto> updateValidator,
        IValidator<TimbrarCFDIDto> timbrarValidator,
        IValidator<CancelarCFDIDto> cancelarValidator)
    {
        _context = context;
        _logger = logger;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _timbrarValidator = timbrarValidator;
        _cancelarValidator = cancelarValidator;
    }

    /// <summary>
    /// Crear un nuevo CFDI con validaciones fiscales.
    /// </summary>
    public async Task<CFDIDto> CreateCFDIAsync(CreateCFDIDto request)
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

            // Verificar que el cargo existe y está activo
            var cargo = await _context.Cargos
                .Include(c => c.Alumno)
                .Include(c => c.ConceptoCobro)
                .Include(c => c.CicloEscolar)
                .FirstOrDefaultAsync(c => c.Id == request.CargoId && c.Activo);
            if (cargo == null)
                throw new NotFoundException("Cargo", request.CargoId);

            // Verificar que el cargo no tenga ya un CFDI timbrado
            var cfdiExistente = await _context.CFDIs
                .AnyAsync(c => c.CargoId == request.CargoId && c.Estado == "Timbrada");
            if (cfdiExistente)
                throw new BusinessException("Ya existe un CFDI timbrado para este cargo.");

            // Obtener configuración fiscal de la escuela
            var configuracionFiscal = await _context.ConfiguracionesCFDI
                .FirstOrDefaultAsync(c => c.SchoolId == cargo.Alumno!.SchoolId && c.Activa);
            if (configuracionFiscal == null)
                throw new BusinessException("No hay configuración fiscal activa para la escuela del alumno.");

            // Obtener RFC de la escuela
            var school = await _context.Schools.FindAsync(cargo.Alumno.SchoolId);
            if (school == null)
                throw new BusinessException("No se encontró la escuela del alumno.");

            // Generar folio único
            var folio = await GenerateFolioAsync(request.Serie ?? "A");

            // Crear CFDI
            var cfdi = new CFDI
            {
                CargoId = request.CargoId,
                UUID = string.Empty, // Se asigna en timbrado
                Serie = request.Serie ?? "A",
                Folio = request.Folio ?? folio,
                RFC_Emisor = school.RFC,
                RFC_Receptor = request.RFC_Receptor,
                NombreReceptor = request.NombreReceptor,
                Subtotal = cargo.Subtotal,
                Descuento = cargo.Descuento,
                IVA = cargo.IVA,
                Total = cargo.Total,
                Estado = "Borrador",
                NivelEducativo = request.NivelEducativo,
                CURP_Alumno = request.CURP_Alumno,
                ClaveCT = request.ClaveCT,
                FechaTimbrado = DateTime.UtcNow,
                ReintentosTimbrado = 0
            };

            _context.CFDIs.Add(cfdi);

            // Crear entrada en bitácora
            var bitacora = new BitacoraFiscal
            {
                CFDIId = cfdi.Id, // Se asignará después de guardar
                CargoId = request.CargoId,
                Evento = "Creación",
                Descripcion = $"CFDI creado para cargo {request.CargoId}",
                UsuarioId = "Sistema", // TODO: Obtener del contexto de usuario
                Timestamp = DateTime.UtcNow
            };

            _context.BitacorasFiscales.Add(bitacora);
            await _context.SaveChangesAsync();

            // Actualizar CFDIId en bitácora
            bitacora.CFDIId = cfdi.Id;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"CFDI creado: ID={cfdi.Id}, Folio={cfdi.Serie}-{cfdi.Folio}, Total={cfdi.Total:C}");

            return MapToDto(cfdi);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error de BD al crear CFDI");
            throw new BusinessException("Error al guardar el CFDI. Verifique los datos e intente nuevamente.");
        }
    }

    /// <summary>
    /// Obtener CFDI por ID.
    /// </summary>
    public async Task<CFDIDto> GetByIdAsync(int id)
    {
        var cfdi = await _context.CFDIs
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cfdi == null)
            throw new NotFoundException("CFDI", id);

        return MapToDto(cfdi);
    }

    /// <summary>
    /// Obtener CFDI con datos completos (relaciones incluidas).
    /// </summary>
    public async Task<CFDIFullDataDto> GetByIdFullAsync(int id)
    {
        var cfdi = await _context.CFDIs
            .Include(c => c.Cargo)
                .ThenInclude(cargo => cargo!.Alumno)
            .Include(c => c.Cargo)
                .ThenInclude(cargo => cargo!.ConceptoCobro)
            .Include(c => c.Cargo)
                .ThenInclude(cargo => cargo!.CicloEscolar)
            .Include(c => c.Bitacoras.OrderByDescending(b => b.Timestamp))
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cfdi == null)
            throw new NotFoundException("CFDI", id);

        return MapToFullDto(cfdi);
    }

    /// <summary>
    /// Obtener todos los CFDI con paginación y búsqueda.
    /// </summary>
    public async Task<PaginatedCFDISDto> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
    {
        var query = _context.CFDIs.AsQueryable();

        // Aplicar búsqueda si se proporciona
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim().ToLower();
            query = query.Where(c =>
                c.Folio.ToLower().Contains(search) ||
                c.Serie.ToLower().Contains(search) ||
                c.UUID.ToLower().Contains(search) ||
                c.RFC_Receptor.ToLower().Contains(search) ||
                c.NombreReceptor.ToLower().Contains(search) ||
                c.Estado.ToLower().Contains(search));
        }

        var totalRecords = await query.CountAsync();

        var cfdis = await query
            .OrderByDescending(c => c.FechaTimbrado)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return new PaginatedCFDISDto
        {
            Items = cfdis.Select(MapToDto).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords
        };
    }

    /// <summary>
    /// Actualizar un CFDI.
    /// </summary>
    public async Task<CFDIDto> UpdateCFDIAsync(int id, UpdateCFDIDto request)
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

            var cfdi = await _context.CFDIs.FindAsync(id);
            if (cfdi == null)
                throw new NotFoundException("CFDI", id);

            // Validaciones de negocio
            if (cfdi.Estado == "Timbrada" && request.Estado != "Cancelada")
                throw new BusinessException("No se puede modificar un CFDI timbrado, solo cancelar.");

            if (request.Estado == "Cancelada" && cfdi.Estado != "Timbrada")
                throw new BusinessException("Solo se pueden cancelar CFDI timbrados.");

            // Actualizar campos
            cfdi.Estado = request.Estado;
            cfdi.RazonCancelacion = request.RazonCancelacion;
            cfdi.ErrorTimbrado = request.ErrorTimbrado;

            if (request.Estado == "Cancelada" && !cfdi.FechaCancelacion.HasValue)
                cfdi.FechaCancelacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Registrar en bitácora
            await RegistrarBitacoraAsync(cfdi.Id, null, "Actualización",
                $"CFDI actualizado a estado: {request.Estado}");

            _logger.LogInformation($"CFDI actualizado: ID={cfdi.Id}, Estado={cfdi.Estado}");

            return MapToDto(cfdi);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error de BD al actualizar CFDI");
            throw new BusinessException("Error al actualizar el CFDI. Verifique los datos e intente nuevamente.");
        }
    }

    /// <summary>
    /// Timbrar CFDI (simulación - en producción requeriría PAC real).
    /// </summary>
    public async Task<TimbradoResponseDto> TimbrarCFDIAsync(TimbrarCFDIDto request)
    {
        try
        {
            // Validar con FluentValidation
            var validationResult = await _timbrarValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                throw new Core.Exceptions.ValidationException(errors);
            }

            var cfdi = await _context.CFDIs
                .Include(c => c.Cargo)
                .FirstOrDefaultAsync(c => c.Id == request.CFDIId);
            if (cfdi == null)
                throw new NotFoundException("CFDI", request.CFDIId);

            // Validaciones previas al timbrado
            if (cfdi.Estado == "Timbrada")
                throw new BusinessException("El CFDI ya está timbrado.");

            if (cfdi.Estado == "Cancelada")
                throw new BusinessException("No se puede timbrar un CFDI cancelado.");

            if (!request.ForzarTimbrado && cfdi.ReintentosTimbrado >= 3)
                throw new BusinessException("Se ha alcanzado el límite máximo de reintentos de timbrado.");

            // Simular timbrado (en producción aquí iría la llamada al PAC)
            var uuid = Guid.NewGuid().ToString().ToUpper();
            var fechaTimbrado = DateTime.UtcNow;

            // Actualizar CFDI con datos de timbrado
            cfdi.UUID = uuid;
            cfdi.Estado = "Timbrada";
            cfdi.FechaTimbrado = fechaTimbrado;
            cfdi.ReintentosTimbrado++;
            cfdi.CadenaOriginal = GenerateCadenaOriginal(cfdi);
            cfdi.XMLConTimbrado = GenerateXMLSimulado(cfdi);

            await _context.SaveChangesAsync();

            // Registrar en bitácora
            await RegistrarBitacoraAsync(cfdi.Id, cfdi.CargoId, "Timbrado_Exitoso",
                $"CFDI timbrado exitosamente con UUID: {uuid}");

            _logger.LogInformation($"CFDI timbrado: ID={cfdi.Id}, UUID={uuid}");

            return new TimbradoResponseDto
            {
                Exitoso = true,
                UUID = uuid,
                Mensaje = "CFDI timbrado exitosamente",
                FechaTimbrado = fechaTimbrado
            };
        }
        catch (Exception ex)
        {
            // Registrar error en bitácora
            await RegistrarBitacoraAsync(request.CFDIId, null, "Error_Timbrado",
                $"Error al timbrar CFDI: {ex.Message}");

            _logger.LogError(ex, "Error al timbrar CFDI {CFDIId}", request.CFDIId);

            return new TimbradoResponseDto
            {
                Exitoso = false,
                Mensaje = "Error al timbrar CFDI",
                Error = ex.Message
            };
        }
    }

    /// <summary>
    /// Cancelar CFDI (simulación - en producción requeriría PAC real).
    /// </summary>
    public async Task<TimbradoResponseDto> CancelarCFDIAsync(CancelarCFDIDto request)
    {
        try
        {
            // Validar con FluentValidation
            var validationResult = await _cancelarValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                throw new Core.Exceptions.ValidationException(errors);
            }

            var cfdi = await _context.CFDIs.FindAsync(request.CFDIId);
            if (cfdi == null)
                throw new NotFoundException("CFDI", request.CFDIId);

            // Validaciones para cancelación
            if (cfdi.Estado != "Timbrada")
                throw new BusinessException("Solo se pueden cancelar CFDI timbrados.");

            // Simular cancelación (en producción aquí iría la llamada al PAC)
            cfdi.Estado = "Cancelada";
            cfdi.FechaCancelacion = DateTime.UtcNow;
            cfdi.RazonCancelacion = request.RazonCancelacion;

            await _context.SaveChangesAsync();

            // Registrar en bitácora
            await RegistrarBitacoraAsync(cfdi.Id, cfdi.CargoId, "Cancelación",
                $"CFDI cancelado: {request.RazonCancelacion}");

            _logger.LogInformation($"CFDI cancelado: ID={cfdi.Id}, Razón={request.RazonCancelacion}");

            return new TimbradoResponseDto
            {
                Exitoso = true,
                Mensaje = "CFDI cancelado exitosamente",
                FechaTimbrado = cfdi.FechaCancelacion
            };
        }
        catch (Exception ex)
        {
            // Registrar error en bitácora
            await RegistrarBitacoraAsync(request.CFDIId, null, "Error_Cancelación",
                $"Error al cancelar CFDI: {ex.Message}");

            _logger.LogError(ex, "Error al cancelar CFDI {CFDIId}", request.CFDIId);

            return new TimbradoResponseDto
            {
                Exitoso = false,
                Mensaje = "Error al cancelar CFDI",
                Error = ex.Message
            };
        }
    }

    /// <summary>
    /// Verificar si existe un CFDI.
    /// </summary>
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.CFDIs.AnyAsync(c => c.Id == id);
    }

    /// <summary>
    /// Obtener CFDI de un cargo específico.
    /// </summary>
    public async Task<List<CFDIDto>> GetCFDISByCargoAsync(int cargoId)
    {
        // Verificar que el cargo existe
        var cargoExists = await _context.Cargos.AnyAsync(c => c.Id == cargoId && c.Activo);
        if (!cargoExists)
            throw new NotFoundException("Cargo", cargoId);

        var cfdis = await _context.CFDIs
            .Where(c => c.CargoId == cargoId)
            .OrderByDescending(c => c.FechaTimbrado)
            .AsNoTracking()
            .ToListAsync();

        return cfdis.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Obtener CFDI por estado.
    /// </summary>
    public async Task<List<CFDIDto>> GetCFDISByEstadoAsync(string estado)
    {
        var cfdis = await _context.CFDIs
            .Where(c => c.Estado == estado)
            .OrderByDescending(c => c.FechaTimbrado)
            .AsNoTracking()
            .ToListAsync();

        return cfdis.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Generar folio único para CFDI.
    /// </summary>
    private async Task<string> GenerateFolioAsync(string serie)
    {
        var date = DateTime.UtcNow;
        var baseFolio = $"{date:yyyyMMdd}";
        var counter = 1;
        var folio = $"{baseFolio}{counter:D4}";

        while (await _context.CFDIs.AnyAsync(c => c.Serie == serie && c.Folio == folio))
        {
            counter++;
            folio = $"{baseFolio}{counter:D4}";
        }

        return folio;
    }

    /// <summary>
    /// Registrar entrada en bitácora fiscal.
    /// </summary>
    private async Task RegistrarBitacoraAsync(int? cfdiId, int? cargoId, string evento, string descripcion)
    {
        var bitacora = new BitacoraFiscal
        {
            CFDIId = cfdiId,
            CargoId = cargoId,
            Evento = evento,
            Descripcion = descripcion,
            UsuarioId = "Sistema", // TODO: Obtener del contexto de usuario
            Timestamp = DateTime.UtcNow
        };

        _context.BitacorasFiscales.Add(bitacora);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Generar cadena original simulada (en producción sería calculada por SAT).
    /// </summary>
    private string GenerateCadenaOriginal(CFDI cfdi)
    {
        return $"||{cfdi.Id}|{cfdi.RFC_Emisor}|{cfdi.RFC_Receptor}|{cfdi.Total}|{cfdi.FechaTimbrado:o}||";
    }

    /// <summary>
    /// Generar XML simulado (en producción sería el XML real del CFDI).
    /// </summary>
    private string GenerateXMLSimulado(CFDI cfdi)
    {
        return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<cfdi:Comprobante xmlns:cfdi=""http://www.sat.gob.mx/cfd/4"" Version=""4.0""
    Serie=""{cfdi.Serie}"" Folio=""{cfdi.Folio}"" Fecha=""{cfdi.FechaTimbrado:o}""
    SubTotal=""{cfdi.Subtotal}"" Total=""{cfdi.Total}"">
    <!-- XML simulado para demostración -->
</cfdi:Comprobante>";
    }

    /// <summary>
    /// Mapear entidad CFDI a DTO básico.
    /// </summary>
    private static CFDIDto MapToDto(CFDI cfdi)
    {
        return new CFDIDto
        {
            Id = cfdi.Id,
            CargoId = cfdi.CargoId,
            UUID = cfdi.UUID,
            Serie = cfdi.Serie,
            Folio = cfdi.Folio,
            RFC_Emisor = cfdi.RFC_Emisor,
            RFC_Receptor = cfdi.RFC_Receptor,
            NombreReceptor = cfdi.NombreReceptor,
            Subtotal = cfdi.Subtotal,
            Descuento = cfdi.Descuento,
            IVA = cfdi.IVA,
            Total = cfdi.Total,
            Estado = cfdi.Estado,
            NivelEducativo = cfdi.NivelEducativo,
            CURP_Alumno = cfdi.CURP_Alumno,
            ClaveCT = cfdi.ClaveCT,
            FechaTimbrado = cfdi.FechaTimbrado,
            FechaCancelacion = cfdi.FechaCancelacion,
            RazonCancelacion = cfdi.RazonCancelacion,
            ErrorTimbrado = cfdi.ErrorTimbrado
        };
    }

    /// <summary>
    /// Mapear entidad CFDI a DTO completo.
    /// </summary>
    private static CFDIFullDataDto MapToFullDto(CFDI cfdi)
    {
        return new CFDIFullDataDto
        {
            Id = cfdi.Id,
            Cargo = cfdi.Cargo != null ? new CargoDto
            {
                Id = cfdi.Cargo.Id,
                AlumnoId = cfdi.Cargo.AlumnoId,
                ConceptoCobroId = cfdi.Cargo.ConceptoCobroId,
                CicloEscolarId = cfdi.Cargo.CicloEscolarId,
                Folio = cfdi.Cargo.Folio,
                Mes = cfdi.Cargo.Mes,
                Monto = cfdi.Cargo.Monto,
                Descuento = cfdi.Cargo.Descuento,
                Recargo = cfdi.Cargo.Recargo,
                Subtotal = cfdi.Cargo.Subtotal,
                IVA = cfdi.Cargo.IVA,
                Total = cfdi.Cargo.Total,
                Estado = cfdi.Cargo.Estado,
                MontoRecibido = cfdi.Cargo.MontoRecibido,
                FechaEmision = cfdi.Cargo.FechaEmision,
                FechaVencimiento = cfdi.Cargo.FechaVencimiento,
                FechaPago = cfdi.Cargo.FechaPago,
                Observacion = cfdi.Cargo.Observacion
            } : null,
            UUID = cfdi.UUID,
            Serie = cfdi.Serie,
            Folio = cfdi.Folio,
            RFC_Emisor = cfdi.RFC_Emisor,
            RFC_Receptor = cfdi.RFC_Receptor,
            NombreReceptor = cfdi.NombreReceptor,
            Subtotal = cfdi.Subtotal,
            Descuento = cfdi.Descuento,
            IVA = cfdi.IVA,
            Total = cfdi.Total,
            Estado = cfdi.Estado,
            NivelEducativo = cfdi.NivelEducativo,
            CURP_Alumno = cfdi.CURP_Alumno,
            ClaveCT = cfdi.ClaveCT,
            FechaTimbrado = cfdi.FechaTimbrado,
            FechaCancelacion = cfdi.FechaCancelacion,
            RazonCancelacion = cfdi.RazonCancelacion,
            ErrorTimbrado = cfdi.ErrorTimbrado,
            CadenaOriginal = cfdi.CadenaOriginal,
            XMLConTimbrado = cfdi.XMLConTimbrado,
            Bitacoras = cfdi.Bitacoras?.Select(b => new BitacoraFiscalDto
            {
                Id = b.Id,
                CFDIId = b.CFDIId,
                CargoId = b.CargoId,
                Evento = b.Evento,
                Descripcion = b.Descripcion,
                DetalleError = b.DetalleError,
                UsuarioId = b.UsuarioId,
                IPOrigen = b.IPOrigen,
                Timestamp = b.Timestamp
            }).ToList() ?? []
        };
    }
}
