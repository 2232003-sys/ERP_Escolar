
import apiClient from '@/lib/api/client';
import type { ApiResponse } from '@/lib/api/types';
import type { Alumno, CreateAlumnoDto, UpdateAlumnoDto } from '@/types/alumno';
import type { EstadoCuenta } from '@/types/finanzas';


export const alumnoService = {

  async getAllAlumnos(): Promise<Alumno[]> {
    const response = await apiClient.get<ApiResponse<Alumno[]>>('/alumnos');
    return response.data.data;
  },

  async getAlumnoById(id: number): Promise<Alumno> {
    const response = await apiClient.get<ApiResponse<Alumno>>(`/alumnos/${id}`);
    return response.data.data;
  },

  async createAlumno(alumnoData: CreateAlumnoDto): Promise<Alumno> {
    const response = await apiClient.post<ApiResponse<Alumno>>('/alumnos', alumnoData);
    return response.data.data;
  },

  async updateAlumno(id: number, alumnoData: UpdateAlumnoDto): Promise<Alumno> {
    const response = await apiClient.put<ApiResponse<Alumno>>(`/alumnos/${id}`, alumnoData);
    return response.data.data;
  },

  async deleteAlumno(id: number): Promise<void> {
    await apiClient.delete(`/alumnos/${id}`);
  },

  async getEstadoCuenta(alumnoId: number): Promise<ApiResponse<EstadoCuenta>> {
    try {
      const response = await apiClient.get<ApiResponse<EstadoCuenta>>(
        `/estado-cuenta/${alumnoId}`
      );
      return response.data;
    } catch (error) {
      throw error;
    }
  },
};
