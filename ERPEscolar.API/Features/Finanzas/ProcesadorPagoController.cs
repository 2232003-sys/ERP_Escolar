
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using ERPEscolar.API.Data;
using Stripe;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.EntityFrameworkCore;

namespace ERPEscolar.API.Features.Finanzas
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProcesadorPagoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<ProcesadorPagoController> _logger;

        public ProcesadorPagoController(AppDbContext context, IConfiguration config, ILogger<ProcesadorPagoController> logger)
        {
            _context = context;
            _config = config;
            _logger = logger;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        public class CreatePaymentIntentRequest
        {
            public Guid CargoId { get; set; }
        }
        
        [HttpPost("create-payment-intent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
        {
            try
            {
                var cargo = await _context.Cargos
                    .Include(c => c.Alumno)
                    .FirstOrDefaultAsync(c => c.Id == request.CargoId && c.Activo);

                if (cargo == null)
                {
                    return NotFound(new { message = "El cargo especificado no fue encontrado." });
                }

                if (cargo.Estado == "Pagado")
                {
                    return BadRequest(new { message = "Este cargo ya ha sido pagado." });
                }

                var montoPendiente = (long)((cargo.Total - cargo.MontoRecibido) * 100); // Stripe usa centavos

                if (montoPendiente <= 0)
                {
                     return BadRequest(new { message = "El cargo no tiene monto pendiente de pago." });
                }

                var options = new PaymentIntentCreateOptions
                {
                    Amount = montoPendiente,
                    Currency = "mxn",
                    Description = $"Cargo escolar para {cargo.Alumno.Nombre} {cargo.Alumno.ApellidoPaterno}",
                    Metadata = new Dictionary<string, string>
                    {
                        { "cargoId", cargo.Id.ToString() },
                        { "alumnoId", cargo.AlumnoId.ToString() },
                        { "folioCargo", cargo.Folio }
                    },
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                    },
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                return Ok(new { clientSecret = paymentIntent.ClientSecret });
            }
            catch(StripeException se)
            {
                _logger.LogError(se, "Error de Stripe al crear Payment Intent para CargoId {CargoId}", request.CargoId);
                return StatusCode(500, new { message = $"Error con el proveedor de pagos: {se.Message}"});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico al crear Payment Intent para CargoId {CargoId}", request.CargoId);
                return StatusCode(500, new { message = "Ocurrió un error inesperado en el servidor." });
            }
        }
    }
}
