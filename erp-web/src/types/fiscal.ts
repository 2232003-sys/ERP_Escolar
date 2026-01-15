
import type { Cargo } from './finanzas';

/**
 * Representa los datos básicos de un CFDI (Factura).
 */
export interface CFDI {
  id: number;
  cargoId: number;
  uuid: string;
  serie: string;
  folio: string;
  rfcEmisor: string;
  rfcReceptor: string;
  nombreReceptor: string;
  subtotal: number;
  descuento: number;
  iva: number;
  total: number;
  estado: 'Borrador' | 'Timbrada' | 'Cancelada' | 'Error';
  nivelEducativo?: string;
  curpAlumno?: string;
  claveCt?: string;
  fechaTimbrado?: string;
  fechaCancelacion?: string;
  razonCancelacion?: string;
  errorTimbrado?: string;
}

/**
 * Representa una entrada en la bitácora de eventos de un CFDI.
 */
export interface BitacoraFiscal {
  id: number;
  evento: string;
  descripcion: string;
  detalleError?: string;
  usuarioId: string;
  timestamp: string;
}

/**
 * Representa un CFDI con todos sus datos relacionados (cargo, bitácora, etc.).
 */
export interface CFDIFull extends CFDI {
  colegiatura: Cargo | null;
  cadenaOriginal?: string;
  xmlConTimbrado?: string;
  bitacoras: BitacoraFiscal[];
}

/**
 * DTO para crear un nuevo CFDI.
 */
export interface CreateCFDIDto {
  cargoId: number;
  rfcReceptor: string;
  nombreReceptor: string;
  serie?: string;
  folio?: string;
  nivelEducativo?: string;
  curpAlumno?: string;
  claveCt?: string;
}

/**
 * DTO para actualizar el estado de un CFDI (ej. para cancelar).
 */
export interface UpdateCFDIDto {
  estado: 'Borrador' | 'Timbrada' | 'Cancelada' | 'Error';
  razonCancelacion?: string;
  errorTimbrado?: string;
}

/**
 * DTO para solicitar el timbrado de un CFDI.
 */
export interface TimbrarCFDIDto {
  cfdiId: number;
  forzarTimbrado?: boolean;
}

/**
 * DTO para solicitar la cancelación de un CFDI.
 */
export interface CancelarCFDIDto {
  cfdiId: number;
  razonCancelacion: string;
}

/**
 * Representa la respuesta del servicio de timbrado/cancelación.
 */
export interface TimbradoResponse {
  exitoso: boolean;
  uuid?: string;
  mensaje: string;
  fechaTimbrado?: string;
  error?: string;
}
