
import apiClient from '@/lib/api/client';
import type { ApiResponse } from '@/lib/api/types';
import type { Cargo } from './cargoService';

export interface RegistrarPagoDto {
  cargoId: string;
  metodoPago: 'Efectivo' | 'Transferencia' | 'TarjetaDebito' | 'TarjetaCredito';
  monto: number;
  referencia?: string; 
  fechaPago: string;
}

export interface GeneracionMasivaDto {
  concepto: string;
  monto: number;
  fechaVencimiento: string;
  idsAlumnos?: number[]; 
}

export const finanzasService = {

  async registrarPagoManual(pagoData: RegistrarPagoDto): Promise<ApiResponse<Cargo>> {
    const response = await apiClient.post<ApiResponse<Cargo>>('/pagos/registrar-manual', pagoData);
    return response.data;
  },

  async generarCargosMasivos(generacionData: GeneracionMasivaDto): Promise<ApiResponse<{ message: string; cargosGenerados: number }>> {
    const response = await apiClient.post<ApiResponse<{ message: string; cargosGenerados: number }>>('/cargos/generacion-masiva', generacionData);
    return response.data;
  },
};
