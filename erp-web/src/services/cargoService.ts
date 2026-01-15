
import apiClient from '@/lib/api/client';
import type { ApiResponse } from '@/lib/api/types';

/**
 * Define la estructura de un objeto de Cargo, que representa un cobro pendiente o pagado.
 */
export interface Cargo {
  id: string; // O Guid
  alumnoId: number;
  concepto: string;
  folio: string;
  total: number;
  montoRecibido: number;
  fechaVencimiento: string; // ISO 8601
  estado: 'Pendiente' | 'Pagado' | 'Vencido';
  activo: boolean;
}

/**
 * Servicio para gestionar los cargos financieros de los alumnos.
 */
export const cargoService = {

  /**
   * Obtiene todos los cargos asociados a un ID de alumno específico.
   * @param alumnoId - El ID del alumno.
   * @returns Una promesa que se resuelve con la lista de cargos del alumno.
   */
  async getCargosByAlumnoId(alumnoId: number): Promise<ApiResponse<Cargo[]>> {
    try {
      const response = await apiClient.get<ApiResponse<Cargo[]>>(`/cargos/alumno/${alumnoId}`);
      return response.data;
    } catch (error) {
      console.error(`Error al obtener los cargos para el alumno ${alumnoId}:`, error);
      throw error;
    }
  },
};
