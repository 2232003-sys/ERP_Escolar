
import apiClient from '@/lib/api/client';
import type { ApiResponse } from '@/lib/api/types';
// Se elimina la definición local y se importa la centralizada
import type { CFDI } from '@/lib/api/types';

// La interfaz local 'Cfdi' ha sido eliminada.

export const cfdiService = {

  // El método ahora devuelve el tipo CFDI consistente
  async getAllCfdi(): Promise<ApiResponse<CFDI[]>> {
    const response = await apiClient.get<ApiResponse<CFDI[]>>('/cfdi');
    return response.data;
  },

  async emitirCfdi(cargoId: string): Promise<ApiResponse<CFDI>> {
    const response = await apiClient.post<ApiResponse<CFDI>>('/cfdi/emitir', { cargoId });
    return response.data;
  },

  async cancelarCfdi(uuid: string): Promise<ApiResponse<any>> {
    const response = await apiClient.post<ApiResponse<any>>(`/cfdi/cancelar/${uuid}`);
    return response.data;
  },
  
  async enviarCfdiPorEmail(uuid: string, email: string): Promise<ApiResponse<any>> {
    const response = await apiClient.post<ApiResponse<any>>('/cfdi/enviar', { uuid, email });
    return response.data;
  },
};
