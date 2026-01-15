using ERPEscolar.API.Core.Exceptions;
using ERPEscolar.API.Data;
using ERPEscolar.API.DTOs.Finanzas;
using ERPEscolar.API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ERPEscolar.API.Infrastructure.Services;

public interface IEstadoCuentaService
{
    Task<EstadoCuentaDto> GetEstadoCuentaAsync(int alumnoId);
    Task<EstadoCuentaHistorialDto> GetHistorialEstadosCuentaAsync(int alumnoId, DateTime? desde = null, DateTime? hasta = null);
    Task<byte[]> GenerarEstadoCuentaPdfAsync(int alumnoId);
    Task EnviarEstadoCuentaPorEmailAsync(int alumnoId, EnviarEstadoCuentaEmailDto dto);
}

public class EstadoCuentaService : IEstadoCuentaService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public EstadoCuentaService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<EstadoCuentaDto> GetEstadoCuentaAsync(int alumnoId)
    {
        var alumno = await _context.Alumnos
            .Include(a => a.Cargos.Where(c => c.Activo))
                .ThenInclude(c => c.ConceptoCobro)
            .Include(a => a.Cargos.Where(c => c.Activo))
                .ThenInclude(c => c.Pagos.Where(p => p.Activo))
            .FirstOrDefaultAsync(a => a.Id == alumnoId && a.Activo);

        if (alumno == null)
        {
            throw new NotFoundException("Alumno no encontrado");
        }

        var estadoCuenta = new EstadoCuentaDto
        {
            AlumnoId = alumno.Id,
            AlumnoNombre = $"{alumno.Nombre} {alumno.Apellido}",
            Matricula = alumno.Matricula ?? "Sin matrícula",
            FechaGeneracion = DateTime.UtcNow
        };

        // Calcular totales
        foreach (var cargo in alumno.Cargos)
        {
            estadoCuenta.TotalCargos += cargo.Total;
            estadoCuenta.TotalPagos += cargo.MontoRecibido;

            var cargoEstado = new CargoEstadoDto
            {
                Id = cargo.Id,
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
                Concepto = cargo.ConceptoCobro?.Nombre ?? "Sin concepto"
            };

            estadoCuenta.Cargos.Add(cargoEstado);

            // Agregar pagos del cargo
            foreach (var pago in cargo.Pagos)
            {
                var pagoEstado = new PagoEstadoDto
                {
                    Id = pago.Id,
                    Folio = pago.Folio,
                    Monto = pago.Monto,
                    Metodo = pago.Metodo,
                    Estado = pago.Estado,
                    FechaPago = pago.FechaPago,
                    ReferenciaExterna = pago.ReferenciaExterna
                };

                estadoCuenta.Pagos.Add(pagoEstado);
            }
        }

        estadoCuenta.SaldoActual = estadoCuenta.TotalCargos - estadoCuenta.TotalPagos;
        estadoCuenta.SaldoPendiente = estadoCuenta.Cargos
            .Where(c => c.Estado != "Pagado")
            .Sum(c => c.Total - c.MontoRecibido);

        return estadoCuenta;
    }

    public async Task<EstadoCuentaHistorialDto> GetHistorialEstadosCuentaAsync(int alumnoId, DateTime? desde = null, DateTime? hasta = null)
    {
        // For now, return current state. In a real implementation, you might store historical snapshots
        var estadoActual = await GetEstadoCuentaAsync(alumnoId);

        return new EstadoCuentaHistorialDto
        {
            EstadosCuenta = new[] { estadoActual },
            TotalCount = 1
        };
    }

    public async Task<byte[]> GenerarEstadoCuentaPdfAsync(int alumnoId)
    {
        var estadoCuenta = await GetEstadoCuentaAsync(alumnoId);

        // TODO: Implement PDF generation using a library like iTextSharp or similar
        // For now, return a simple byte array placeholder
        var pdfContent = $"Estado de Cuenta - {estadoCuenta.AlumnoNombre}\n" +
                        $"Saldo Actual: {estadoCuenta.SaldoActual:C}\n" +
                        $"Saldo Pendiente: {estadoCuenta.SaldoPendiente:C}\n" +
                        $"Fecha: {estadoCuenta.FechaGeneracion}";

        return System.Text.Encoding.UTF8.GetBytes(pdfContent);
    }

    public async Task EnviarEstadoCuentaPorEmailAsync(int alumnoId, EnviarEstadoCuentaEmailDto dto)
    {
        var estadoCuenta = await GetEstadoCuentaAsync(alumnoId);
        var pdfBytes = await GenerarEstadoCuentaPdfAsync(alumnoId);

        // TODO: Implement email sending using a service like SendGrid, SMTP, etc.
        // For now, just log the action
        Console.WriteLine($"Enviando estado de cuenta por email a {dto.Email} para alumno {estadoCuenta.AlumnoNombre}");

        // Simulate email sending
        await Task.CompletedTask;
    }
}