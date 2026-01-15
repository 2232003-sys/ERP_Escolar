
import { apiClient } from '@/lib/api/client';
import type { Alumno, CreateAlumnoDto, UpdateAlumnoDto } from '@/types/alumno';

/**
 * Servicio para gestionar las operaciones CRUD de los alumnos.
 */
export const alumnoService = {
  /**
   * Obtiene la lista completa de todos los alumnos.
   */
  async getAllAlumnos(): Promise<Alumno[]> {
    try {
      const response = await apiClient.get<Alumno[]>('/alumnos');
      return response.data;
    } catch (error) {
      console.error('Error al obtener los alumnos:', error);
      throw error;
    }
  },

  /**
   * Obtiene un único alumno por su ID.
   */
  async getAlumnoById(id: number): Promise<Alumno> {
    try {
      const response = await apiClient.get<Alumno>(`/alumnos/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Error al obtener el alumno con ID ${id}:`, error);
      throw error;
    }
  },

  /**
   * Crea un nuevo alumno en el sistema.
   */
  async createAlumno(data: CreateAlumnoDto): Promise<Alumno> {
    try {
      const response = await apiClient.post<Alumno>('/alumnos', data);
      return response.data;
    } catch (error) {
      console.error('Error al crear el alumno:', error);
      throw error;
    }
  },

  /**
   * Actualiza un alumno existente en el sistema.
   */
  async updateAlumno(id: number, data: UpdateAlumnoDto): Promise<Alumno> {
    try {
      const response = await apiClient.put<Alumno>(`/alumnos/${id}`, data);
      return response.data;
    } catch (error) {
      console.error(`Error al actualizar el alumno con ID ${id}:`, error);
      throw error;
    }
  },

  /**
   * Elimina un alumno del sistema.
   */
  async deleteAlumno(id: number): Promise<void> {
    try {
      await apiClient.delete(`/alumnos/${id}`);
    } catch (error) {
      console.error(`Error al eliminar el alumno con ID ${id}:`, error);
      throw error;
    }
  },
};
