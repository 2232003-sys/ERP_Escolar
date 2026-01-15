
import apiClient from '@/lib/api/client';
import type { ApiResponse } from '@/lib/api/types';

/**
 * Define la respuesta esperada del endpoint que crea un Payment Intent.
 */
export interface CreatePaymentIntentResponse {
  clientSecret: string;
}

/**
 * Servicio para interactuar con el procesador de pagos del backend.
 */
export const procesadorPagoService = {

  /**
   * Crea un "Payment Intent" en el backend para un cargo específico.
   * Esto inicia el proceso de pago y devuelve un "client secret" que el frontend usará para completar el pago con Stripe.
   * @param cargoId - El ID del cargo que se desea pagar.
   * @returns Una promesa que se resuelve con el client secret.
   */
  async createPaymentIntent(cargoId: string): Promise<ApiResponse<CreatePaymentIntentResponse>> {
    try {
      const response = await apiClient.post<ApiResponse<CreatePaymentIntentResponse>>('/procesadorpago/create-payment-intent', { cargoId });
      return response.data;
    } catch (error) {
      console.error('Error al crear el Payment Intent:', error);
      throw error;
    }
  },
};
