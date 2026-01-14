# 📚 AlumnosController - Documentación de Endpoints

## 🎯 Base URL
```
http://localhost:5235/api/alumnos
```

## 🔐 Autenticación
Todos los endpoints requieren **JWT Bearer Token** en el header:
```
Authorization: Bearer {token}
```

Para obtener un token, usar el endpoint `/api/auth/login`:
```bash
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin123!"
}
```

Respuesta:
```json
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "eyJhbGc...",
  "expiresIn": 3600
}
```

---

## 📋 Endpoints Disponibles

### 1️⃣ GET /api/alumnos
**Obtener listado de alumnos con paginación y búsqueda**

**Autorización:** `[Authorize]` - Cualquier usuario autenticado

**Parámetros Query:**
- `pageNumber` (int, default: 1) - Número de página
- `pageSize` (int, default: 10) - Cantidad de registros por página
- `searchTerm` (string, nullable) - Buscar por nombre, apellido, email o CURP

**Ejemplo de Solicitud:**
```bash
GET /api/alumnos?pageNumber=1&pageSize=10&searchTerm=alumno
```

**Respuesta 200 OK:**
```json
{
  "items": [
    {
      "id": 1,
      "nombre": "Alumno1",
      "apellido": "Demo",
      "email": "alumno1@erp.local",
      "curp": "ALUD0000001000",
      "fechaNacimiento": "2010-01-11T00:00:00Z",
      "sexo": "F",
      "matricula": "ALU-2024-00001",
      "activo": true,
      "fechaInscripcion": "2025-01-13T12:34:56Z",
      "schoolId": 1
    }
  ],
  "totalItems": 10,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 1
}
```

---

### 2️⃣ GET /api/alumnos/{id}
**Obtener datos básicos de un alumno**

**Autorización:** `[Authorize]` - Cualquier usuario autenticado

**Parámetros:**
- `id` (int) - ID del alumno

**Ejemplo de Solicitud:**
```bash
GET /api/alumnos/1
```

**Respuesta 200 OK:**
```json
{
  "id": 1,
  "nombre": "Alumno1",
  "apellido": "Demo",
  "email": "alumno1@erp.local",
  "curp": "ALUD0000001000",
  "fechaNacimiento": "2010-01-11T00:00:00Z",
  "sexo": "F",
  "matricula": "ALU-2024-00001",
  "activo": true,
  "fechaInscripcion": "2025-01-13T12:34:56Z",
  "schoolId": 1
}
```

**Respuesta 404 Not Found:**
```json
{
  "message": "Alumno con ID 999 no encontrado"
}
```

---

### 3️⃣ GET /api/alumnos/{id}/completo
**Obtener alumno con datos completos (tutores e inscripciones)**

**Autorización:** `[Authorize]` - Cualquier usuario autenticado

**Parámetros:**
- `id` (int) - ID del alumno

**Ejemplo de Solicitud:**
```bash
GET /api/alumnos/1/completo
```

**Respuesta 200 OK:**
```json
{
  "id": 1,
  "nombre": "Alumno1",
  "apellido": "Demo",
  "email": "alumno1@erp.local",
  "curp": "ALUD0000001000",
  "fechaNacimiento": "2010-01-11T00:00:00Z",
  "sexo": "F",
  "matricula": "ALU-2024-00001",
  "activo": true,
  "fechaInscripcion": "2025-01-13T12:34:56Z",
  "schoolId": 1,
  "tutoresNombres": [
    "Tutor1 Demo",
    "Tutor2 Demo"
  ],
  "inscripciones": [
    {
      "grupoId": 1,
      "grupoNombre": "1ro A",
      "cicloEscolarId": 1,
      "cicloNombre": "2024-2025",
      "activo": true
    }
  ]
}
```

---

### 4️⃣ POST /api/alumnos
**Crear un nuevo alumno**

**Autorización:** `[Authorize(Roles = "SuperAdmin,Admin TI,Control Escolar")]`

**Request Body:**
```json
{
  "nombre": "Juan",
  "apellido": "García",
  "email": "juan.garcia@example.com",
  "curp": "GARJ000000ABC",
  "fechaNacimiento": "2010-05-15T00:00:00Z",
  "sexo": "M",
  "direccion": "Calle Principal 123",
  "telefonoContacto": "33-1234-5678",
  "schoolId": 1,
  "tutorId": null
}
```

**Respuesta 201 Created:**
```json
{
  "id": 11,
  "nombre": "Juan",
  "apellido": "García",
  "email": "juan.garcia@example.com",
  "curp": "GARJ000000ABC",
  "fechaNacimiento": "2010-05-15T00:00:00Z",
  "sexo": "M",
  "matricula": "ALU-2024-00011",
  "activo": true,
  "fechaInscripcion": "2025-01-13T12:34:56Z",
  "schoolId": 1
}
```

**Respuesta 400 Bad Request (Validación):**
```json
{
  "message": "Error de validación",
  "errors": [
    "El alumno con CURP 'GARJ000000ABC' ya existe"
  ]
}
```

**Respuesta 404 Not Found:**
```json
{
  "message": "School con ID 999 no encontrada"
}
```

---

### 5️⃣ PUT /api/alumnos/{id}
**Actualizar datos de un alumno**

**Autorización:** `[Authorize(Roles = "SuperAdmin,Admin TI,Control Escolar")]`

**Parámetros:**
- `id` (int) - ID del alumno

**Request Body (actualización parcial):**
```json
{
  "nombre": "Juan Carlos",
  "apellido": "García López",
  "email": "juan.carlos@example.com",
  "fechaNacimiento": "2010-05-15T00:00:00Z",
  "sexo": "M"
}
```

**Nota:** Solo se pueden actualizar: `Nombre`, `Apellido`, `Email`, `FechaNacimiento`, `Sexo`

No se pueden cambiar: `CURP`, `Matricula`, `Activo`, `SchoolId`, `FechaCreacion`, `FechaInscripcion`

**Respuesta 200 OK:**
```json
{
  "id": 1,
  "nombre": "Juan Carlos",
  "apellido": "García López",
  "email": "juan.carlos@example.com",
  "curp": "ALUD0000001000",
  "fechaNacimiento": "2010-05-15T00:00:00Z",
  "sexo": "M",
  "matricula": "ALU-2024-00001",
  "activo": true,
  "fechaInscripcion": "2025-01-13T12:34:56Z",
  "schoolId": 1
}
```

**Respuesta 400 Bad Request:**
```json
{
  "message": "Error de validación",
  "errors": [
    "El email ya está registrado"
  ]
}
```

**Respuesta 404 Not Found:**
```json
{
  "message": "Alumno con ID 999 no encontrado"
}
```

---

### 6️⃣ DELETE /api/alumnos/{id}
**Desactivar alumno (Soft Delete)**

**Autorización:** `[Authorize(Roles = "SuperAdmin,Admin TI,Control Escolar")]`

**Parámetros:**
- `id` (int) - ID del alumno

**Ejemplo de Solicitud:**
```bash
DELETE /api/alumnos/1
```

**Respuesta 204 No Content:**
```
(Sin body - Solo header HTTP 204)
```

**Respuesta 404 Not Found:**
```json
{
  "message": "Alumno con ID 999 no encontrado"
}
```

---

### 7️⃣ PATCH /api/alumnos/{id}/restore
**Restaurar alumno desactivado**

**Autorización:** `[Authorize(Roles = "SuperAdmin,Admin TI,Control Escolar")]`

**Parámetros:**
- `id` (int) - ID del alumno

**Ejemplo de Solicitud:**
```bash
PATCH /api/alumnos/1/restore
```

**Respuesta 204 No Content:**
```
(Sin body - Solo header HTTP 204)
```

**Respuesta 404 Not Found:**
```json
{
  "message": "Alumno con ID 999 no encontrado"
}
```

---

## 🧪 Testing con cURL

### Login (Obtener Token)
```bash
curl -X POST "http://localhost:5235/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin123!"
  }'
```

### GET Alumnos (Con Token)
```bash
curl -X GET "http://localhost:5235/api/alumnos?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer {token}"
```

### GET Alumno por ID
```bash
curl -X GET "http://localhost:5235/api/alumnos/1" \
  -H "Authorization: Bearer {token}"
```

### POST Crear Alumno
```bash
curl -X POST "http://localhost:5235/api/alumnos" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "nombre": "Nuevo",
    "apellido": "Alumno",
    "email": "nuevo@example.com",
    "curp": "ALUN000000XYZ",
    "fechaNacimiento": "2010-06-20T00:00:00Z",
    "sexo": "M",
    "schoolId": 1
  }'
```

### PUT Actualizar Alumno
```bash
curl -X PUT "http://localhost:5235/api/alumnos/1" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "nombre": "Nombre Actualizado",
    "apellido": "Apellido Actualizado",
    "email": "actualizado@example.com",
    "fechaNacimiento": "2010-05-15T00:00:00Z",
    "sexo": "M"
  }'
```

### DELETE Desactivar Alumno
```bash
curl -X DELETE "http://localhost:5235/api/alumnos/1" \
  -H "Authorization: Bearer {token}"
```

### PATCH Restaurar Alumno
```bash
curl -X PATCH "http://localhost:5235/api/alumnos/1/restore" \
  -H "Authorization: Bearer {token}"
```

---

## 📊 Códigos de Respuesta HTTP

| Código | Descripción | Casos |
|--------|-------------|-------|
| **200** | OK | GET exitoso, PUT exitoso |
| **201** | Created | POST exitoso, recurso creado |
| **204** | No Content | DELETE/PATCH exitoso |
| **400** | Bad Request | Validación fallida, datos inválidos |
| **401** | Unauthorized | Falta token JWT |
| **403** | Forbidden | Usuario sin permisos/roles |
| **404** | Not Found | Recurso no existe |
| **409** | Conflict | Error de negocio (CURP/Email duplicado) |
| **500** | Internal Server Error | Error no controlado en servidor |

---

## 🔑 Credenciales de Prueba (Seed Data)

```
✅ SuperAdmin
  Username: admin
  Password: Admin123!
  Roles: SuperAdmin

✅ Usuario Control Escolar
  Username: docente1
  Password: Docente123!
  Roles: Docente

✅ Alumno Demo
  Username: alumno1
  Password: Alumno123!
  Roles: Alumno
```

---

## 📝 Validaciones Implementadas

### CreateAlumnoDto
- ✅ Nombre: Requerido, 2-100 caracteres
- ✅ Apellido: Requerido, 2-100 caracteres
- ✅ Email: Requerido, formato válido, único
- ✅ CURP: Requerido, 18 caracteres, único
- ✅ FechaNacimiento: Requerido, mayor de 14 años
- ✅ Sexo: Requerido (M o F)
- ✅ SchoolId: Requerido, escuela debe existir

### UpdateAlumnoDto
- ✅ Nombre: 2-100 caracteres
- ✅ Apellido: 2-100 caracteres
- ✅ Email: Formato válido, único
- ✅ FechaNacimiento: Mayor de 14 años
- ✅ Sexo: M o F

---

## 🚀 Características Especiales

### Soft Delete
Los alumnos no se eliminan de la BD, se marcan como `Activo = false`

### AutoMapper
Los mapeos DTO → Model se realizan automáticamente sin código manual

### Paginación
Todos los listados soportan paginación:
```json
{
  "items": [...],
  "totalItems": 50,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 5
}
```

### Búsqueda
Busca en múltiples campos: Nombre, Apellido, Email, CURP
```
GET /api/alumnos?searchTerm=juan
```

---

## ⚙️ Configuración en Program.cs

```csharp
// Servicios
builder.Services.AddScoped<IAlumnoService, AlumnoService>();

// Validadores (Auto-discovery)
builder.Services.AddValidatorsFromAssemblyContaining<CreateAlumnoValidator>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(AlumnoProfile));

// Autorización
builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors("AllowAll", policy =>
    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
```

---

## 📞 Soporte

Para errores o preguntas, revisar:
- `Infrastructure/Services/AlumnoService.cs` - Lógica de negocio
- `Features/ControlEscolar/AlumnosController.cs` - Endpoints
- `Validators/CreateAlumnoValidator.cs` - Validaciones
- `Infrastructure/Mappings/AlumnoProfile.cs` - Mapeos AutoMapper
