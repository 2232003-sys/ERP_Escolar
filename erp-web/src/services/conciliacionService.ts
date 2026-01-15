
import apiClient from '@/lib/api/client';
import type { ApiResponse } from '@/lib/api/types';

/**
 * Define la estructura del resumen del resultado de la conciliación,
 * que es lo que la API devuelve tras procesar el archivo.
 */
export interface ConciliacionSummary {
  totalTransacciones: number;
  pagosReconciliados: number;
  errores: number;
  detallesErrores: string[];
}

/**
 * Servicio para el proceso de conciliación bancaria.
 */
export const conciliacionService = {

  /**
   * Sube y procesa un archivo de estado de cuenta bancario para conciliar los pagos.
   * @param file - El archivo (usualmente CSV o TXT) del estado de cuenta.
   * @returns Una promesa que se resuelve con un resumen de la operación.
   */
  async conciliarPagos(file: File): Promise<ApiResponse<ConciliacionSummary>> {
    const formData = new FormData();
    formData.append('file', file);

    const response = await apiClient.post<ApiResponse<ConciliacionSummary>>(
      '/conciliacion/procesar-estado-cuenta',
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    );
    return response.data;
  },
};
