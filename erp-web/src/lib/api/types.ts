// Tipos para la API del ERP Escolar

export interface Alumno {
  id: number
  nombre: string
  apellidoPaterno: string
  apellidoMaterno: string
  rfc: string
  curp: string
  fechaNacimiento: string
  email: string
  telefono: string
  direccion: string
  matricula: string
  schoolId: number
  activo: boolean
}

export interface Cargo {
  id: number
  alumnoId: number
  concepto: string
  monto: number
  subtotal: number
  descuento: number
  iva: number
  total: number
  fechaVencimiento: string
  estado: 'Pendiente' | 'Pagado' | 'Vencido'
  activo: boolean
}

export interface CFDI {
  id: number
  cargoId: number
  uuid: string
  serie: string
  folio: string
  rfcEmisor: string
  rfcReceptor: string
  nombreReceptor: string
  subtotal: number
  descuento: number
  iva: number
  total: number
  estado: 'Borrador' | 'Timbrada' | 'Cancelada'
  fechaEmision: string
  fechaTimbrado: string
  nivelEducativo: string
  curpAlumno: string
  claveCT: string
  activo: boolean
}

export interface AuthResponse {
  token: string
  refreshToken: string
  user: {
    id: number
    username: string
    email: string
    roles: string[]
  }
}

export interface ApiResponse<T> {
  data: T
  message: string
  success: boolean
}

export interface PaginatedResponse<T> {
  data: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
}