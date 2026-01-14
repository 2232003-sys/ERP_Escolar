# ERP Escolar - Sistema de Gestión Educativa

## 📋 Descripción General

Sistema integral de gestión educativa con módulos de:
- **Control Escolar**: Estudiantes, tutores, grupos, materias, horarios, asistencia
- **Académico**: Calificaciones, períodos, reportes
- **Finanzas**: Cargos, pagos, becas, descuentos
- **Fiscal**: CFDI 4.0, Complemento Educativo (IEDU), timbrado electrónico
- **Portal Familias**: Consulta de boletas, estado de cuenta, asistencia

## 🏗️ Arquitectura

### Backend (C# .NET 8)
- **Framework**: ASP.NET Core
- **BD Principal**: PostgreSQL
- **Autenticación**: JWT + RBAC
- **Patrones**: Repository Pattern, Dependency Injection, Clean Architecture

### Estructura de Carpetas
```
ERPEscolar.API/
├── Models/                 # Entidades de BD
├── Data/                   # DbContext y migraciones
├── Features/               # Módulos funcionales
│   ├── Auth/
│   ├── ControlEscolar/
│   ├── Finanzas/
│   └── Fiscal/
├── Infrastructure/
│   ├── Repositories/       # Data access layer
│   └── Services/           # Lógica de negocio
├── DTOs/                   # Data Transfer Objects
├── Validators/             # Validaciones
├── Core/
│   └── Exceptions/         # Excepciones personalizadas
└── Program.cs              # Startup
```

## 🗄️ Modelo de Datos

### Tablas Principales
- `Schools` - Instituciones educativas (multi-plantel futuro)
- `CiclosEscolares` - Ciclos escolares con períodos
- `Alumnos` - Estudiantes
- `Tutores` - Padres/Tutores
- `Docentes` - Maestros
- `Grupos` - Clases
- `Materias` - Asignaturas
- `GrupoMaterias` - Relación grupo-materia-docente
- `Inscripciones` - Matrícula de alumnos
- `Asistencias` - Registro de asistencia
- `Calificaciones` - Notas por período
- `Cargos` - Facturas/deudas
- `Pagos` - Registros de pago
- `CFDIs` - Facturas electrónicas
- `BitacorasFiscales` - Auditoría fiscal
- `Users`, `Roles`, `Permisos` - Seguridad

## 🔐 Autenticación y Autorización

### JWT Token
- **Expiration**: 1 hora
- **Refresh Token**: 7 días
- **Claims**: UserId, Username, Email, Roles, Permisos

### Roles Iniciales
- `SuperAdmin` / `Admin TI`
- `Dirección`
- `Control Escolar`
- `Docente`
- `Caja/Finanzas`
- `Padre/Tutor`
- `Alumno` (opcional, lectura)

## 🚀 Iniciando

### Requisitos
- .NET 8 SDK
- PostgreSQL 12+
- Node.js 18+ (para frontend)

### Instalación Backend

1. **Restaurar dependencias**:
```bash
cd ERPEscolar.API
dotnet restore
```

2. **Configurar conexión BD** (`appsettings.json`):
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=ERPEscolarDB;Username=postgres;Password=tu_contraseña"
}
```

3. **Crear BD y aplicar migraciones**:
```bash
dotnet ef database update
```

4. **Generar JWT Secret** (seguro):
```bash
# Usa una contraseña fuerte, mínimo 32 caracteres
```

5. **Ejecutar la API**:
```bash
dotnet run
```

API estará en: `https://localhost:5001`

### Instalación Frontend

```bash
# (Próximo paso)
```

## 📝 APIs Principales

### Autenticación
- `POST /api/auth/login` - Iniciar sesión
- `POST /api/auth/refresh` - Renovar token
- `POST /api/auth/register` - Crear usuario

### Control Escolar
- `GET /api/alumnos` - Listar alumnos
- `POST /api/alumnos` - Crear alumno
- `GET /api/grupos` - Listar grupos
- `POST /api/asistencias` - Registrar asistencia

### Finanzas
- `GET /api/cargos` - Listar facturas
- `POST /api/pagos` - Registrar pago
- `GET /api/estado-cuenta/{alumnoId}` - Estado de cuenta

### Fiscal
- `POST /api/cfdi/generar` - Generar CFDI
- `POST /api/cfdi/timbrar` - Timbrar factura
- `GET /api/cfdi/{id}` - Consultar CFDI

## 🔧 Configuración

### Variables de Entorno (appsettings.Development.json)
```json
{
  "Jwt": {
    "SecretKey": "tu_clave_super_secreta_minimo_32_caracteres",
    "Issuer": "ERPEscolar",
    "Audience": "ERPEscolarClient",
    "ExpirationHours": 1
  },
  "Fiscal": {
    "Proveedor": "FINKOK",
    "UrlTimbrado": "https://pruebafactura.finkok.com/servicios/soap",
    "Usuario": "usuario_pac",
    "Contraseña": "contraseña_pac"
  }
}
```

## 📦 Migraciones

### Crear migración
```bash
dotnet ef migrations add NombreMigracion
```

### Aplicar migraciones
```bash
dotnet ef database update
```

### Revertir migración
```bash
dotnet ef migrations remove
```

## ✅ Testing

```bash
dotnet test
```

## 📄 Licencia

Privado

## 🤝 Soporte

Para consultas o reportar bugs, contactar al equipo de desarrollo.

---

**Última actualización**: Enero 2026
**Estado**: En desarrollo - MVP
