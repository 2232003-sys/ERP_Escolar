# 🚀 Roadmap de Desarrollo - ERP Escolar

## Fase Actual: ✅ COMPLETA - Arquitectura Base

### Lo que hicimos (Sesión 1):
1. ✅ Diseño de arquitectura limpia (Clean Architecture)
2. ✅ 49 tablas de BD con relaciones complejas
3. ✅ Modelo RBAC completo
4. ✅ Autenticación JWT + BCrypt
5. ✅ Migraciones automáticas
6. ✅ AuthService y AuthController base
7. ✅ Repository Pattern

---

## 📋 FASE 2: Servicios de Control Escolar (2-3 días)

### Controllers a crear:

#### 1. **AlumnosController** (`/api/control-escolar/alumnos`)
```
GET    /                          - Listar con paginación, filtros
GET    /{id}                      - Obtener alumno específico
GET    /{id}/estado-completo      - Alumno + tutor + inscripciones
POST   /                          - Crear alumno
PUT    /{id}                      - Editar alumno
DELETE /{id}                      - Marcar inactivo (soft delete)
GET    /{id}/expediente          - Descargar expediente (PDF)
POST   /{id}/asignar-tutor       - Vincular tutor
```

#### 2. **GruposController** (`/api/control-escolar/grupos`)
```
GET    /                          - Listar grupos
GET    /{id}                      - Grupo con alumnos inscritos
GET    /{id}/lista-clase          - Attendance list
POST   /                          - Crear grupo
PUT    /{id}                      - Editar grupo
GET    /{id}/horario             - Horario del grupo
```

#### 3. **InscripcionesController** (`/api/control-escolar/inscripciones`)
```
POST   /                          - Matricular alumno
DELETE /{id}                      - Desmatricular
GET    /alumno/{alumnoId}         - Inscripciones de alumno
PUT    /{id}                      - Cambiar de grupo
```

#### 4. **AsistenciasController** (`/api/control-escolar/asistencias`)
```
POST   /                          - Registrar asistencia (docente)
POST   /batch                     - Registrar múltiples (lista)
GET    /grupo/{grupoMateriaId}   - Asistencia de grupo por fecha
GET    /alumno/{alumnoId}        - Historial de alumno
GET    /reportes                 - Reportes por período
PUT    /{id}                     - Editar registro
```

#### 5. **CalificacionesController** (`/api/control-escolar/calificaciones`)
```
POST   /                          - Registrar calificación
PUT    /{id}                      - Actualizar
GET    /grupo/{grupoMateriaId}   - Calificaciones del grupo
GET    /alumno/{alumnoId}        - Expediente académico
GET    /alumno/{alumnoId}/boleta - Boleta por período
POST   /reportes/cierre          - Cierre de período
```

### Servicios a crear:

```csharp
// AlumnoService
- CreateAlumnoAsync()
- GetAlumnoWithFullDataAsync()
- GetAlumnosWithPaginationAsync()
- ValidateMatriculaUniqueAsync()
- GenerateMatriculaAsync()

// GrupoService
- CreateGrupoAsync()
- GetGrupoWithAlumnosAsync()

// AsistenciaService
- RegisterAsistenciaAsync()
- GetReportAsistenciaAsync()
- CalculateAsistenciaPercentageAsync()

// CalificacionesService
- RegisterCalificacionAsync()
- CalculatePromedioAsync()
- CalculatePromedioFinalAsync()
- GenerateBolетaAsync()
- ValidarCierrePeriodoAsync()
```

### DTOs a crear:
```
AlumnoDto
CreateAlumnoDto
UpdateAlumnoDto
AlumnoFullDataDto
InscripcionDto
AsistenciaDto
CalificacionDto
BolетaDto
```

---

## 💰 FASE 3: Servicios de Finanzas (2-3 días)

### Controllers:

#### 1. **CargosController** (`/api/finanzas/cargos`)
```
GET    /                          - Listar con filtros (alumno, estado, mes)
GET    /{id}                      - Detalle cargo
POST   /                          - Crear cargo manual
POST   /batch                     - Crear múltiples (import CSV)
PUT    /{id}                      - Editar
DELETE /{id}                      - Cancelar cargo
GET    /generar-mensuales         - Generar cargos automáticos
GET    /pendientes                - Resumen de cobranza
```

#### 2. **PagosController** (`/api/finanzas/pagos`)
```
GET    /                          - Listar pagos
GET    /{id}                      - Detalle pago
POST   /                          - Registrar pago
POST   /transferencia             - Pago por transferencia (Oxxo, Paypal, etc)
PUT    /{id}                      - Editar (si no verificado)
GET    /pendientes-conciliacion   - Para auditor
GET    /exportar-excel            - Exportar período
```

#### 3. **EstadoCuentaController** (`/api/finanzas/estado-cuenta`)
```
GET    /{alumnoId}                - Estado actual
GET    /{alumnoId}/historial      - Histórico con detalles
GET    /{alumnoId}/pdf            - Descargar estado de cuenta
POST   /{alumnoId}/enviar-email   - Enviar por email
```

#### 4. **BecasController** (`/api/finanzas/becas`)
```
GET    /                          - Listar becas
GET    /{id}                      - Detalle beca
POST   /                          - Crear beca
PUT    /{id}                      - Editar
DELETE /{id}                      - Cancelar beca
GET    /calcular-descuento        - Simular aplicación de beca
```

### Servicios:

```csharp
// CargoService
- CreateCargoAsync()
- GenerateMensualCargesAsync() // Tarea programada
- UpdateCargoEstateAsync()
- GetEstadisticasCobranzaAsync()

// PagoService
- RegisterPagoAsync()
- ProcessPagoAsync() // Validar y aplicar descuentos
- VerifyPagoAsync()
- ExportarPagosAsync()

// EstadoCuentaService
- GetEstadoCuentaAsync()
- CalculateDeudaAsync()
- CalculateInteresesByDaysAsync() // Recargos por mora

// BecaService
- CreateBecaAsync()
- ApplyBecaAsync()
- CalculateBecaImpactAsync()
```

### DTOs:
```
CargoDto
CreateCargoDto
PagoDto
RegisterPagoDto
EstadoCuentaDto
TransactionSummaryDto
BecaDto
```

---

## 🧾 FASE 4: Motor Fiscal CFDI (3-4 días)

### Controllers:

#### **CFDIController** (`/api/fiscal/cfdi`)
```
GET    /                          - Listar CFDIs
GET    /{id}                      - Detalle CFDI
GET    /{id}/xml                  - Descargar XML
GET    /{id}/pdf                  - Descargar PDF
POST   /generar                   - Generar CFDI desde cargo
POST   /timbrar                   - Enviar a SAT
POST   /{id}/retimbrar           - Reintentar si falló
DELETE /{id}/cancelar             - Cancelar factura (con estatus SAT)
GET    /reportes/diarios          - Timbrados del día
GET    /reportes/bitacora         - Auditoría fiscal
POST   /validar-xml               - Validar XML antes de timbrar
```

### Servicios:

```csharp
// CFDIService
- GenerateCFDIAsync()           // XML builder
- TimbreCFDIAsync()             // Llamar PAC (FINKOK)
- CancelCFDIAsync()
- ValidateCFDIAsync()
- GetCFDIStatusAsync()          // Consultar SAT

// ComplementoEducativoService
- GenerateComplementoIEDUAsync()
- ValidateComplementoAsync()

// BitacoraFiscalService
- LogOperationAsync()           // Auditoría
- GetBitacoraAsync()
- GetReporteFiscalAsync()
```

### Librerías necesarias:
```
System.Security.Cryptography    (firma digital)
X509Certificate2                 (certificado .cer/.key)
HttpClient                       (llamadas PAC)
System.Xml.Linq                 (manipulación XML)
SelectPdf (o iTextSharp)        (generación PDF)
```

---

## 🎨 FASE 5: Frontend React (4-5 días)

### Setup inicial:
```bash
# Crear proyecto
npx create-react-app erp-escolar-web
cd erp-escolar-web

# Instalar dependencias
npm install @reduxjs/toolkit react-redux
npm install axios
npm install react-router-dom
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p

# UI components (opcional pero recomendado)
npm install @headlessui/react @heroicons/react
```

### Estructura:
```
src/
├── components/              # Componentes reutilizables
│   ├── Header.tsx
│   ├── Sidebar.tsx
│   ├── Modal.tsx
│   └── ...
├── pages/                   # Páginas (una por ruta)
│   ├── LoginPage.tsx
│   ├── DashboardPage.tsx
│   ├── AlumnosPage.tsx
│   └── ...
├── features/                # Redux slices
│   ├── auth/
│   ├── alumnos/
│   ├── finanzas/
│   └── ...
├── services/                # API client
│   ├── api.ts
│   ├── authService.ts
│   ├── alumnosService.ts
│   └── ...
├── hooks/                   # Custom hooks
│   ├── useAuth.ts
│   ├── useFetch.ts
│   └── ...
├── store.ts                 # Redux store
├── App.tsx
└── index.tsx
```

### Páginas principales:

**1. Login** (`/login`)
- Form con email + password
- Guardar JWT en localStorage
- Redirigir a dashboard

**2. Dashboard** (`/dashboard`)
- Stats: Alumnos, Deuda, Cargos del mes
- Últimas transacciones
- Alertas (períodos a cerrar, facturas pendientes)

**3. Control Escolar**
- Listado de alumnos con tabla
- Create/Edit alumno modal
- Búsqueda y filtros
- Inscripciones (grupo, ciclo)
- Asistencia (registrar, ver reportes)
- Calificaciones (captura, boletas)

**4. Finanzas**
- Listado de cargos
- Estado de cuenta (por alumno)
- Registrar pago
- Becas
- Reportes

**5. Portal Familias** (sub-app separada)
- Login tutor
- Ver boleta
- Ver asistencia
- Estado de cuenta
- Descargar comprobantes

---

## ✅ Checklist de Implementación

### Backend
- [ ] Fase 2: Controllers de Control Escolar
- [ ] Fase 2: Servicios de Academia
- [ ] Fase 3: Controllers de Finanzas
- [ ] Fase 3: Servicios de Finanzas
- [ ] Fase 4: CFDI Generator
- [ ] Fase 4: Integración PAC
- [ ] Validaciones (FluentValidation)
- [ ] Error handling global
- [ ] Logging centralizado
- [ ] Unit tests
- [ ] Integration tests

### Frontend
- [ ] Fase 5: Setup React
- [ ] Auth y protección de rutas
- [ ] Componentes base
- [ ] Páginas de módulos
- [ ] Estado global (Redux)
- [ ] Llamadas API
- [ ] Manejo de errores
- [ ] Responsive design
- [ ] Testing

### DevOps
- [ ] Docker setup (Backend + DB)
- [ ] CI/CD (GitHub Actions)
- [ ] Deploy a AWS/Azure
- [ ] Ambiente de staging
- [ ] Backup strategy

---

## 🎯 Estimación de tiempo

| Fase | Duración | Complejidad |
|------|----------|-------------|
| 1 (Completada) | 1 día | Alta |
| 2 (Control Escolar) | 2-3 días | Media |
| 3 (Finanzas) | 2-3 días | Media |
| 4 (Fiscal CFDI) | 3-4 días | Alta |
| 5 (Frontend) | 4-5 días | Media |
| **TOTAL MVP** | **12-16 días** | **-** |

---

## 🔑 Decisiones Clave

1. **CFDI**: Usar PAC establecido (FINKOK es estable)
2. **Frontend**: React > Angular por curva de aprendizaje
3. **BD**: PostgreSQL > SQLServer por open source
4. **Auth**: JWT > Cookie por API REST
5. **Testing**: Primero integración (API), luego unit

---

## 📞 Contacto

Para preguntas sobre la arquitectura, revisar:
- `ARQUITECTURA.md` - Detalle técnico
- `README.md` - Guía de setup
- Logs de desarrollo en `/var/log/erp/`

---

**Versión**: 1.0
**Última actualización**: 14 Enero 2026
**Siguiente revisión**: Después de completar Fase 2
