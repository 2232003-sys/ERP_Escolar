
// Definición de la interfaz para un Alumno, basada en el AlumnoDto del backend.
export interface Alumno {
  id: number;
  nombre: string;
  apellido: string;
  email: string;
  curp: string;
  fechaNacimiento: string; // Se maneja como string para simplificar
  sexo: string;
  matricula?: string;
  activo: boolean;
  fechaInscripcion: string;
  schoolId: number;
}

/**
 * DTO para la creación de un nuevo alumno.
 * Basado en el `CreateAlumnoDto` del backend.
 */
export interface CreateAlumnoDto {
  nombre: string;
  apellido: string;
  email: string;
  curp: string;
  fechaNacimiento: string;
  sexo: string; // 'M' o 'F'
  direccion?: string;
  telefonoContacto?: string;
  schoolId: number;
  tutorId?: number;
}

/**
 * DTO para la actualización de un alumno existente.
 * Basado en el `UpdateAlumnoDto` del backend.
 */
export interface UpdateAlumnoDto {
  nombre: string;
  apellido: string;
  email: string;
  fechaNacimiento: string;
  sexo: string; // 'M' o 'F'
}
