
export type MetodoPago = "Efectivo" | "Transferencia" | "TarjetaDebito" | "TarjetaCredito";

/**
 * Representa la estructura de un Cargo (deuda) en el estado de cuenta.
 */
export interface Cargo {
  id: number;
  folio: string;
  mes: string;
  monto: number;
  descuento: number;
  recargo: number;
  subtotal: number;
  iva: number;
  total: number;
  estado: 'Pendiente' | 'Pagado' | 'Parcial' | 'Cancelado';
  montoRecibido: number;
  fechaEmision: string; 
  fechaVencimiento?: string;
  concepto: string;
}

/**
 * Representa la estructura de un Pago en el estado de cuenta.
 */
export interface Pago {
  id: number;
  folio: string;
  monto: number;
  metodo: string;
  estado: 'Verificado' | 'Pendiente' | 'Rechazado';
  fechaPago: string;
  referenciaExterna?: string;
}

/**
 * Representa el DTO completo del Estado de Cuenta de un Alumno.
 */
export interface EstadoCuenta {
  alumnoId: number;
  alumnoNombre: string;
  matricula: string;
  saldoActual: number;
  totalCargos: number;
  totalPagos: number;
  saldoPendiente: number;
  fechaGeneracion: string;
  cargos: Cargo[];
  pagos: Pago[];
}

/**
 * DTO para el registro de un nuevo pago.
 */
export interface RegistrarPagoDto {
  alumnoId: number;
  cargoId: string;
  monto: number;
  metodoPago: MetodoPago; // Usa el tipo específico
  fechaPago: string;
  referenciaExterna?: string;
  observacion?: string;
}
