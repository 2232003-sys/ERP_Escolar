
using ERPEscolar.API.Data;
using ERPEscolar.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ERPEscolar.API.Infrastructure.Services;

/// <summary>
/// DTO para la solicitud de registro de un nuevo pago.
/// </summary>
public class RegistrarPagoRequest
{
    public int AlumnoId { get; set; }
    public int CargoId { get; set; }
    public decimal Monto { get; set; }
    public string Metodo { get; set; } = null!; // "Transferencia", "Efectivo", etc.
    public DateTime FechaPago { get; set; }
    public string? ReferenciaExterna { get; set; }
    public string? Observacion { get; set; }
}


/// <summary>
/// Servicio central para orquestar la lógica de negocio financiera.
/// </summary>
public class FinanzasService
{
    private readonly AppDbContext _context;

    public FinanzasService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Registra un nuevo pago y lo aplica a un cargo existente, actualizando su estado.
    /// </summary>
    /// <param name="request">Los datos del pago a registrar.</param>
    /// <returns>El objeto Pago recién creado.</returns>
    /// <exception cref="Exception">Lanza excepciones si el cargo o el alumno no existen, o si el cargo ya está pagado.</exception>
    public async Task<Pago> RegistrarPagoAsync(RegistrarPagoRequest request)
    {
        // 1. Validar que el cargo exista y no esté cancelado.
        var cargo = await _context.Cargos
            .FirstOrDefaultAsync(c => c.Id == request.CargoId && c.AlumnoId == request.AlumnoId && c.Activo);

        if (cargo == null)
        {
            throw new Exception("El cargo especificado no existe, no pertenece al alumno o ha sido cancelado.");
        }

        // 2. Validar que el cargo no esté ya completamente pagado.
        if (cargo.Estado == "Pagado")
        {
            throw new Exception($"El cargo con folio {cargo.Folio} ya se encuentra totalmente pagado.");
        }

        // 3. Crear y guardar la nueva entidad de Pago.
        var nuevoPago = new Pago
        {
            AlumnoId = request.AlumnoId,
            CargoId = request.CargoId,
            Folio = $"PAGO-{request.CargoId}-{DateTime.UtcNow.Ticks}", // Folio único
            Monto = request.Monto,
            Metodo = request.Metodo,
            Estado = "Verificado", // Se asume que el registro manual ya está verificado
            ReferenciaExterna = request.ReferenciaExterna,
            FechaPago = request.FechaPago,
            Observacion = request.Observacion,
            Activo = true
        };

        await _context.Pagos.AddAsync(nuevoPago);
        
        // 4. Actualizar el estado y el monto recibido del cargo.
        cargo.MontoRecibido += request.Monto;
        cargo.FechaActualizacion = DateTime.UtcNow;

        if (cargo.MontoRecibido >= cargo.Total)
        {
            cargo.Estado = "Pagado";
            cargo.FechaPago = request.FechaPago; 
        }
        else
        {
            cargo.Estado = "Parcial";
        }

        // 5. Guardar todos los cambios en la base de datos en una única transacción.
        await _context.SaveChangesAsync();

        return nuevoPago;
    }

    /// <summary>
    /// Genera los cargos de colegiatura para todos los alumnos activos para un mes específico.
    /// Este método es idempotente. Si ya existe un cargo para un alumno en el mes especificado, no se creará uno nuevo.
    /// </summary>
    public async Task<string> GenerarCargosMensualesAsync(int year, int month)
    {
        var mesFormato = $"{year}-{month:D2}";

        var conceptoColegiatura = await _context.ConceptosCobro
            .FirstOrDefaultAsync(c => c.Clave == "COLEGIATURA_MENSUAL" && c.Activo);

        if (conceptoColegiatura == null)
        {
            return "Error: No se encontró un concepto de cobro activo con la clave 'COLEGIATURA_MENSUAL'.";
        }

        var alumnosActivos = await _context.Alumnos
            .Where(a => a.Activo)
            .Include(a => a.Becas.Where(b => b.Activa))
            .ToListAsync();

        var cargosCreados = 0;
        var cargosOmitidos = 0;

        foreach (var alumno in alumnosActivos)
        {
            var cargoExistente = await _context.Cargos
                .AnyAsync(c => c.AlumnoId == alumno.Id && c.Mes == mesFormato && c.ConceptoCobroId == conceptoColegiatura.Id);

            if (cargoExistente)
            {
                cargosOmitidos++;
                continue;
            }
            
            decimal montoFinal = conceptoColegiatura.MontoBase;
            decimal descuentoTotal = 0;
            
            var becaPrincipal = alumno.Becas.FirstOrDefault();
            if (becaPrincipal != null)
            {
                descuentoTotal = becaPrincipal.Tipo == "Porcentaje"
                    ? montoFinal * (becaPrincipal.Valor / 100)
                    : becaPrincipal.Valor;
            }
            
            var nuevoCargo = new Cargo
            {
                AlumnoId = alumno.Id,
                ConceptoCobroId = conceptoColegiatura.Id,
                CicloEscolarId = 1, // TODO: Obtener el ciclo escolar actual dinámicamente
                Folio = $"CG-{alumno.Id}-{mesFormato}",
                Mes = mesFormato,
                Monto = montoFinal,
                Descuento = descuentoTotal,
                Estado = "Pendiente",
                FechaEmision = DateTime.UtcNow,
                FechaVencimiento = new DateTime(year, month, 10),
                Activo = true
            };

            await _context.Cargos.AddAsync(nuevoCargo);
            cargosCreados++;
        }

        await _context.SaveChangesAsync();

        return $"Proceso completado. Cargos creados: {cargosCreados}. Cargos omitidos (ya existentes): {cargosOmitidos}.";
    }
}
