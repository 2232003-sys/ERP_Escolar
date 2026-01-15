
using ERPEscolar.API.Data;
using ERPEscolar.API.DTOs.Finanzas;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ERPEscolar.API.Infrastructure.Services;

/// <summary>
/// Servicio para generar y gestionar los estados de cuenta de los alumnos.
/// </summary>
public class EstadoCuentaService
{
    private readonly AppDbContext _context;

    public EstadoCuentaService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene el estado de cuenta completo y actual de un alumno.
    /// </summary>
    /// <param name="alumnoId">El ID del alumno.</param>
    /// <returns>Un DTO con el estado de cuenta completo.</returns>
    /// <exception cref="Exception">Si el alumno no se encuentra.</exception>
    public async Task<EstadoCuentaDto> GetEstadoCuentaAsync(int alumnoId)
    {
        var alumno = await _context.Alumnos
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == alumnoId);

        if (alumno == null)
        {
            throw new Exception($"No se encontró un alumno con el ID {alumnoId}.");
        }

        var cargos = await _context.Cargos
            .AsNoTracking()
            .Where(c => c.AlumnoId == alumnoId && c.Activo)
            .Include(c => c.ConceptoCobro)
            .OrderByDescending(c => c.FechaEmision)
            .ToListAsync();

        var pagos = await _context.Pagos
            .AsNoTracking()
            .Where(p => p.AlumnoId == alumnoId && p.Activo)
            .OrderByDescending(p => p.FechaPago)
            .ToListAsync();

        var totalCargos = cargos.Sum(c => c.Total);
        var totalPagos = pagos.Where(p => p.Estado == "Verificado").Sum(p => p.Monto);

        var estadoCuenta = new EstadoCuentaDto
        {
            AlumnoId = alumno.Id,
            AlumnoNombre = $"{alumno.Nombres} {alumno.ApellidoPaterno} {alumno.ApellidoMaterno}",
            Matricula = alumno.Matricula,
            TotalCargos = totalCargos,
            TotalPagos = totalPagos,
            SaldoPendiente = totalCargos - totalPagos,
            FechaGeneracion = DateTime.UtcNow,

            Cargos = cargos.Select(c => new CargoEstadoDto
            {
                Id = c.Id,
                Folio = c.Folio,
                Mes = c.Mes,
                Monto = c.Monto,
                Descuento = c.Descuento,
                Recargo = c.Recargo,
                Subtotal = c.Monto - c.Descuento,
                IVA = c.IVA,
                Total = c.Total,
                Estado = c.Estado,
                MontoRecibido = c.MontoRecibido,
                FechaEmision = c.FechaEmision,
                FechaVencimiento = c.FechaVencimiento,
                Concepto = c.ConceptoCobro?.Nombre ?? "N/A",
            }).ToList(),

            Pagos = pagos.Select(p => new PagoEstadoDto
            {
                Id = p.Id,
                Folio = p.Folio,
                Monto = p.Monto,
                Metodo = p.Metodo,
                Estado = p.Estado,
                FechaPago = p.FechaPago,
                ReferenciaExterna = p.ReferenciaExterna
            }).ToList()
        };

        // Calcular saldo actual (puede ser diferente al pendiente si hay pagos no verificados)
        estadoCuenta.SaldoActual = estadoCuenta.SaldoPendiente; // Simplificado por ahora

        return estadoCuenta;
    }
}
