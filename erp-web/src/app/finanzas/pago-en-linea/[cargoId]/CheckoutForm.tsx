
'use client';

import { useEffect, useState } from "react";
import {
  PaymentElement,
  useStripe,
  useElements
} from "@stripe/react-stripe-js";

export default function CheckoutForm() {
  const stripe = useStripe();
  const elements = useElements();

  const [message, setMessage] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (!stripe) {
      return;
    }

    const clientSecret = new URLSearchParams(window.location.search).get(
      "payment_intent_client_secret"
    );

    if (!clientSecret) {
      return;
    }

    stripe.retrievePaymentIntent(clientSecret).then(({ paymentIntent }) => {
      switch (paymentIntent?.status) {
        case "succeeded":
          setMessage("¡Pago exitoso!");
          break;
        case "processing":
          setMessage("Su pago se está procesando.");
          break;
        case "requires_payment_method":
          setMessage("Su pago no fue exitoso, por favor intente de nuevo.");
          break;
        default:
          setMessage("Algo salió mal.");
          break;
      }
    });
  }, [stripe]);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!stripe || !elements) {
      // Stripe.js no ha cargado todavía. 
      // No mostrar el formulario hasta que Stripe.js haya cargado.
      return;
    }

    setIsLoading(true);

    const { error } = await stripe.confirmPayment({
      elements,
      confirmParams: {
        // La URL a la que se redirigirá al cliente después del pago.
        return_url: `${window.location.origin}/finanzas/pago-completado`,
      },
    });

    // Este punto solo se alcanza si hay un error inmediato durante la confirmación del pago.
    // De lo contrario, el cliente es redirigido a la `return_url`.
    if (error.type === "card_error" || error.type === "validation_error") {
      setMessage(error.message || 'Ocurrió un error con su tarjeta.');
    } else {
      setMessage("Ocurrió un error inesperado.");
    }

    setIsLoading(false);
  };

  return (
    <form id="payment-form" onSubmit={handleSubmit}>
      <PaymentElement id="payment-element" />
      <button disabled={isLoading || !stripe || !elements} id="submit" className="w-full mt-6 bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 px-4 rounded-md transition-all disabled:bg-gray-400 disabled:cursor-not-allowed">
        <span id="button-text">
          {isLoading ? <div className="spinner" id="spinner"></div> : "Pagar ahora"}
        </span>
      </button>
      {/* Muestra cualquier mensaje de error o éxito */}
      {message && <div id="payment-message" className="mt-4 text-center text-sm text-gray-600">{message}</div>}
    </form>
  );
}

