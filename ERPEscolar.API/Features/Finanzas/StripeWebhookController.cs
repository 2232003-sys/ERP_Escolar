
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;
using System.IO;
using System.Threading.Tasks;
using ERPEscolar.API.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using System;

namespace ERPEscolar.API.Features.Finanzas
{
    [ApiController]
    [Route("api/stripe-webhook")]
    public class StripeWebhookController : ControllerBase
    {
        private readonly string _webhookSecret;
        private readonly IPagoService _pagoService;
        private readonly ILogger<StripeWebhookController> _logger;

        public StripeWebhookController(IConfiguration config, IPagoService pagoService, ILogger<StripeWebhookController> logger)
        {
            // Es CRÍTICO que este secret esté en la configuración y no en el código
            _webhookSecret = config["Stripe:WebhookSecret"];
            _pagoService = pagoService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"],
                    _webhookSecret
                );

                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    _logger.LogInformation($"Webhook de PaymentIntentSucceeded recibido para: {paymentIntent.Id}");
                    await HandlePaymentIntentSucceeded(paymentIntent);
                }
                else
                {
                    _logger.LogWarning($"Webhook de tipo {stripeEvent.Type} recibido, pero no manejado.");
                }

                return Ok();
            }
            catch (StripeException e)
            {
                _logger.LogError(e, "Error de firma en webhook de Stripe. El request podría no ser legítimo.");
                return BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error desconocido al procesar webhook de Stripe.");
                return StatusCode(500);
            }
        }

        private async Task HandlePaymentIntentSucceeded(PaymentIntent paymentIntent)
        {
            if (paymentIntent.Metadata.TryGetValue("cargoId", out var cargoIdStr) && Guid.TryParse(cargoIdStr, out var cargoId))
            {
                // La información del cargo está en los metadatos. Procedemos a registrar el pago.
                var registrarPagoRequest = new DTOs.Finanzas.RegistrarPagoRequest
                {
                    CargoId = cargoId,
                    Monto = (double)paymentIntent.AmountReceived / 100, // Convertir de centavos a la unidad principal
                    Metodo = "Tarjeta de Crédito/Débito (Online)",
                    FechaPago = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    ReferenciaExterna = paymentIntent.Id, // Usar el ID del PaymentIntent como referencia
                    Observacion = $"Pago procesado automáticamente vía Stripe."
                };

                try
                {
                    await _pagoService.RegistrarPago(registrarPagoRequest);
                    _logger.LogInformation($"Pago para CargoId {cargoId} registrado exitosamente desde el webhook de Stripe.");
                }
                catch (Exception ex)
                {\n                    // Si algo falla aquí, es crítico. El cliente pagó pero no se registró.
                    _logger.LogCritical(ex, "¡ALERTA CRÍTICA! El cliente pagó (PI: {PaymentIntentId}) pero falló el registro del pago para el CargoId {CargoId}", paymentIntent.Id, cargoId);
                    // Aquí se podrían añadir notificaciones a administradores (ej. por email)
                }
            }
            else
            {
                _logger.LogWarning("Webhook de PaymentIntentSucceeded recibido, pero no tenía el metadata 'cargoId'. PaymentIntent ID: {PaymentIntentId}", paymentIntent.Id);
            }
        }
    }
}
