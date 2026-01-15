
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ERPEscolar.API.Data;
using CsvHelper;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ERPEscolar.API.Infrastructure.Services
{
    // DTOs para el servicio de conciliación
    public class TransaccionCsv
    {
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string Referencia { get; set; } = string.Empty;
    }

    public class ResultadoConciliacion
    {
        public int TotalTransacciones { get; set; }
        public int PagosReconciliados { get; set; }
        public int Errores { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public List<string> DetallesErrores { get; set; } = new List<string>();
    }

    // Interfaz del servicio
    public interface IConciliacionService
    {
        Task<ResultadoConciliacion> ProcesarCsvAsync(Stream stream);
    }

    // Implementación del servicio
    public class ConciliacionService : IConciliacionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ConciliacionService> _logger;
        private readonly IPagoService _pagoService; // Para registrar el pago

        public ConciliacionService(AppDbContext context, ILogger<ConciliacionService> logger, IPagoService pagoService)
        {
            _context = context;
            _logger = logger;
            _pagoService = pagoService;
        }

        public async Task<ResultadoConciliacion> ProcesarCsvAsync(Stream stream)
        {
            var resultado = new ResultadoConciliacion();
            var errores = new List<string>();
            int lineNumber = 1;

            try
            {
                using (var reader = new StreamReader(stream))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    // Configuración básica del CSV, esto debería ser parametrizable en el futuro
                    // por ahora asumimos columnas: Fecha, Descripcion, Monto
                    csv.Context.RegisterClassMap<TransaccionMap>();
                    var records = csv.GetRecords<TransaccionCsv>().ToList();
                    resultado.TotalTransacciones = records.Count;

                    foreach (var record in records)
                    {
                        lineNumber++;
                        // 1. Extraer la referencia de la descripción
                        // Esta lógica es CRUCIAL y depende del formato del banco
                        // Asumiremos que la referencia está en la descripción, ej "Pago REF:1058-0324"
                        var referencia = ExtraerReferencia(record.Descripcion);
                        if (string.IsNullOrEmpty(referencia))
                        {
                            errores.Add($"Línea {lineNumber}: No se encontró una referencia de pago válida en la descripción: '{record.Descripcion}'.");
                            continue;
                        }

                        // 2. Buscar el cargo en la BD por la referencia (folio)
                        var cargo = await _context.Cargos
                            .FirstOrDefaultAsync(c => c.Folio == referencia && c.Activo);

                        if (cargo == null)
                        {
                            errores.Add($"Línea {lineNumber}: No se encontró un cargo pendiente para la referencia '{referencia}'.");
                            continue;
                        }

                        if (cargo.Estado == "Pagado")
                        {
                             errores.Add($"Línea {lineNumber}: El cargo con referencia '{referencia}' ya se encuentra pagado.");
                             continue;
                        }

                        // 3. Validar el monto
                        var montoPendiente = cargo.Total - cargo.MontoRecibido;
                        if (record.Monto != montoPendiente)
                        {
                            errores.Add($"Línea {lineNumber}: El monto del depósito ${record.Monto} no coincide con el monto pendiente ${montoPendiente} para la referencia '{referencia}'.");
                            continue;
                        }

                        // 4. Registrar el pago (¡la magia!)
                        try
                        {
                            var requestPago = new DTOs.Finanzas.RegistrarPagoRequest
                            {
                                AlumnoId = cargo.AlumnoId,
                                CargoId = cargo.Id,
                                Monto = (double)record.Monto,
                                Metodo = "Transferencia Bancaria",
                                FechaPago = record.Fecha.ToString("yyyy-MM-dd"),
                                ReferenciaExterna = record.Descripcion, // Guardar la descripción completa
                                Observacion = $"Conciliado automáticamente desde CSV."
                            };

                            await _pagoService.RegistrarPago(requestPago);
                            resultado.PagosReconciliados++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error al registrar el pago para la referencia {referencia}", referencia);
                            errores.Add($"Línea {lineNumber}: Error interno al registrar el pago para la referencia '{referencia}'.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el archivo CSV.");
                resultado.Mensaje = "Ocurrió un error crítico al leer el archivo. Verifique el formato.";
                resultado.Errores = 1;
                resultado.DetallesErrores.Add(ex.Message);
                return resultado;
            }

            resultado.Errores = errores.Count;
            resultado.DetallesErrores = errores;
            resultado.Mensaje = "Proceso de conciliación completado.";
            
            return resultado;
        }

        private string ExtraerReferencia(string descripcion)
        {
            // Lógica de ejemplo, a mejorar con Regex y hacerlo configurable
            // Busca un patrón como "1234-5678"
            var match = System.Text.RegularExpressions.Regex.Match(descripcion, @"\b(\d{4}-\d{4})\b");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return string.Empty;
        }
    }

    // Mapeo para CsvHelper para que coincida con nombres de columnas esperados
    public sealed class TransaccionMap : CsvHelper.Configuration.ClassMap<TransaccionCsv>
    {
        public TransaccionMap()
        {   
            // Los nombres aquí DEBEN coincidir con las cabeceras del CSV del banco
            Map(m => m.Fecha).Name("Fecha");
            Map(m => m.Descripcion).Name("Descripción"); // Ojo con acentos
            Map(m => m.Monto).Name("Monto");
        }
    }
}
