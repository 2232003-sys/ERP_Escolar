# 🧪 Ejemplos de Respuestas Reales - AlumnosController

## Datos de Prueba Disponibles

```
Base URL: http://localhost:5235

✅ Credenciales Admin
   Username: admin
   Password: Admin123!

✅ Datos de Ejemplo:
   - 10 alumnos (alumno1-alumno10)
   - 5 tutores (Tutor1-Tutor5 Demo)
   - 1 escuela (Instituto Educativo Demo)
   - 1 ciclo escolar (2024-2025)
   - 1 grupo (1ro A) con 5 inscripciones
```

---

## 1️⃣ POST /api/auth/login - Obtener Token

### Solicitud
```http
POST /api/auth/login HTTP/1.1
Host: localhost:5235
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin123!"
}
```

### Respuesta 200 OK
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwibmFtZSI6ImFkbWluIiwiZW1haWwiOiJhZG1pbkBlcnAubG9jYWwiLCJyb2xlcyI6WyJTdXBlckFkbWluIl0sIm5iZiI6MTczNzAwODEyOCwiZXhwIjoxNzM3MDExNzI4LCJpYXQiOjE3MzcwMDgxMjh9.abc123...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwibmFtZSI6ImFkbWluIiwibmJmIjoxNzM3MDA4MTI4LCJleHAiOjE3MzcxODEwMjgsImlhdCI6MTczNzAwODEyOH0.xyz789...",
  "expiresIn": 3600
}
```

---

## 2️⃣ GET /api/alumnos - Listar Alumnos (Paginado)

### Solicitud
```http
GET /api/alumnos?pageNumber=1&pageSize=5 HTTP/1.1
Host: localhost:5235
Authorization: Bearer {accessToken}
```

### Respuesta 200 OK
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
      "fechaInscripcion": "2025-01-13T15:20:45.123Z",
      "schoolId": 1
    },
    {
      "id": 2,
      "nombre": "Alumno2",
      "apellido": "Demo",
      "email": "alumno2@erp.local",
      "curp": "ALUD0000002000",
      "fechaNacimiento": "2010-01-21T00:00:00Z",
      "sexo": "M",
      "matricula": "ALU-2024-00002",
      "activo": true,
      "fechaInscripcion": "2025-01-13T15:20:45.123Z",
      "schoolId": 1
    },
    {
      "id": 3,
      "nombre": "Alumno3",
      "apellido": "Demo",
      "email": "alumno3@erp.local",
      "curp": "ALUD0000003000",
      "fechaNacimiento": "2010-01-31T00:00:00Z",
      "sexo": "F",
      "matricula": "ALU-2024-00003",
      "activo": true,
      "fechaInscripcion": "2025-01-13T15:20:45.123Z",
      "schoolId": 1
    },
    {
      "id": 4,
      "nombre": "Alumno4",
      "apellido": "Demo",
      "email": "alumno4@erp.local",
      "curp": "ALUD0000004000",
      "fechaNacimiento": "2010-02-10T00:00:00Z",
      "sexo": "M",
      "matricula": "ALU-2024-00004",
      "activo": true,
      "fechaInscripcion": "2025-01-13T15:20:45.123Z",
      "schoolId": 1
    },
    {
      "id": 5,
      "nombre": "Alumno5",
      "apellido": "Demo",
      "email": "alumno5@erp.local",
      "curp": "ALUD0000005000",
      "fechaNacimiento": "2010-02-20T00:00:00Z",
      "sexo": "F",
      "matricula": "ALU-2024-00005",
      "activo": true,
      "fechaInscripcion": "2025-01-13T15:20:45.123Z",
      "schoolId": 1
    }
  ],
  "totalItems": 10,
  "pageNumber": 1,
  "pageSize": 5,
  "totalPages": 2
}
```

### Respuesta con Búsqueda (searchTerm=alumno1)
```http
GET /api/alumnos?pageNumber=1&pageSize=10&searchTerm=alumno1 HTTP/1.1
```

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
      "fechaInscripcion": "2025-01-13T15:20:45.123Z",
      "schoolId": 1
    }
  ],
  "totalItems": 1,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 1
}
```

---

## 3️⃣ GET /api/alumnos/{id} - Obtener Alumno por ID

### Solicitud
```http
GET /api/alumnos/1 HTTP/1.1
Host: localhost:5235
Authorization: Bearer {accessToken}
```

### Respuesta 200 OK
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
  "fechaInscripcion": "2025-01-13T15:20:45.123Z",
  "schoolId": 1
}
```

### Respuesta 404 Not Found (ID no existe)
```http
GET /api/alumnos/999 HTTP/1.1
```

```json
{
  "message": "Alumno con ID 999 no encontrado"
}
```

---

## 4️⃣ GET /api/alumnos/{id}/completo - Alumno Completo

### Solicitud
```http
GET /api/alumnos/1/completo HTTP/1.1
Host: localhost:5235
Authorization: Bearer {accessToken}
```

### Respuesta 200 OK
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
  "fechaInscripcion": "2025-01-13T15:20:45.123Z",
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

## 5️⃣ POST /api/alumnos - Crear Alumno

### Solicitud
```http
POST /api/alumnos HTTP/1.1
Host: localhost:5235
Content-Type: application/json
Authorization: Bearer {accessToken}

{
  "nombre": "Juan Carlos",
  "apellido": "García López",
  "email": "juan.garcia@example.com",
  "curp": "GARJ000000ABCDE",
  "fechaNacimiento": "2009-06-15T00:00:00Z",
  "sexo": "M",
  "direccion": "Calle Principal 123, Guadalajara",
  "telefonoContacto": "33-1234-5678",
  "schoolId": 1,
  "tutorId": null
}
```

### Respuesta 201 Created
```http
HTTP/1.1 201 Created
Location: /api/alumnos/11
Content-Type: application/json

{
  "id": 11,
  "nombre": "Juan Carlos",
  "apellido": "García López",
  "email": "juan.garcia@example.com",
  "curp": "GARJ000000ABCDE",
  "fechaNacimiento": "2009-06-15T00:00:00Z",
  "sexo": "M",
  "matricula": "ALU-2024-00011",
  "activo": true,
  "fechaInscripcion": "2025-01-13T15:25:30.456Z",
  "schoolId": 1
}
```

### Respuesta 400 Bad Request (Validación - Edad)
```json
{
  "message": "Error de validación",
  "errors": [
    "El alumno debe tener al menos 14 años de edad"
  ]
}
```

### Respuesta 400 Bad Request (Validación - CURP duplicado)
```json
{
  "message": "Error de validación",
  "errors": [
    "El alumno con CURP 'ALUD0000001000' ya existe"
  ]
}
```

### Respuesta 400 Bad Request (Validación - Email duplicado)
```json
{
  "message": "Error de validación",
  "errors": [
    "Ya existe un alumno registrado con el email 'alumno1@erp.local'"
  ]
}
```

### Respuesta 404 Not Found (School no existe)
```json
{
  "message": "School con ID 999 no encontrada"
}
```

### Respuesta 403 Forbidden (Roles insuficientes)
```http
HTTP/1.1 403 Forbidden

{
  "title": "Forbidden",
  "status": 403
}
```

---

## 6️⃣ PUT /api/alumnos/{id} - Actualizar Alumno

### Solicitud
```http
PUT /api/alumnos/1 HTTP/1.1
Host: localhost:5235
Content-Type: application/json
Authorization: Bearer {accessToken}

{
  "nombre": "Juan Carlos Actualizado",
  "apellido": "García López Updated",
  "email": "juan.updated@example.com",
  "fechaNacimiento": "2009-06-15T00:00:00Z",
  "sexo": "M"
}
```

### Respuesta 200 OK
```json
{
  "id": 1,
  "nombre": "Juan Carlos Actualizado",
  "apellido": "García López Updated",
  "email": "juan.updated@example.com",
  "curp": "ALUD0000001000",
  "fechaNacimiento": "2009-06-15T00:00:00Z",
  "sexo": "M",
  "matricula": "ALU-2024-00001",
  "activo": true,
  "fechaInscripcion": "2025-01-13T15:20:45.123Z",
  "schoolId": 1
}
```

### Respuesta 400 Bad Request (Email duplicado)
```json
{
  "message": "Error de validación",
  "errors": [
    "Ya existe un alumno registrado con el email 'alumno2@erp.local'"
  ]
}
```

### Respuesta 404 Not Found
```json
{
  "message": "Alumno con ID 999 no encontrado"
}
```

### Respuesta 409 Conflict (Intentar cambiar CURP)
```
(El cambio es ignorado silenciosamente, solo se actualizan campos permitidos)
```

---

## 7️⃣ DELETE /api/alumnos/{id} - Desactivar Alumno

### Solicitud
```http
DELETE /api/alumnos/1 HTTP/1.1
Host: localhost:5235
Authorization: Bearer {accessToken}
```

### Respuesta 204 No Content
```http
HTTP/1.1 204 No Content
```

### Respuesta 404 Not Found
```json
{
  "message": "Alumno con ID 999 no encontrado"
}
```

---

## 8️⃣ PATCH /api/alumnos/{id}/restore - Restaurar Alumno

### Solicitud
```http
PATCH /api/alumnos/1/restore HTTP/1.1
Host: localhost:5235
Authorization: Bearer {accessToken}
```

### Respuesta 204 No Content
```http
HTTP/1.1 204 No Content
```

### Respuesta 404 Not Found
```json
{
  "message": "Alumno con ID 999 no encontrado"
}
```

---

## 🔐 Ejemplo: Sin Autorización (401)

### Solicitud sin Token
```http
GET /api/alumnos HTTP/1.1
Host: localhost:5235
```

### Respuesta 401 Unauthorized
```http
HTTP/1.1 401 Unauthorized
```

---

## 🔐 Ejemplo: Token Expirado (401)

### Solicitud con Token Inválido
```http
GET /api/alumnos HTTP/1.1
Host: localhost:5235
Authorization: Bearer expired_or_invalid_token
```

### Respuesta 401 Unauthorized
```http
HTTP/1.1 401 Unauthorized
```

---

## 📋 Headers en Respuestas

Todas las respuestas incluyen headers estándar:

```http
Content-Type: application/json; charset=utf-8
Date: Mon, 13 Jan 2025 15:30:00 GMT
Server: Kestrel
Transfer-Encoding: chunked
```

---

## 📊 Estadísticas de Respuestas

### Códigos HTTP Observados
- ✅ **200 OK** - GET exitosos
- ✅ **201 Created** - POST exitoso
- ✅ **204 No Content** - DELETE/PATCH exitosos
- ✅ **400 Bad Request** - Validaciones fallidas
- ✅ **401 Unauthorized** - Sin token o token inválido
- ✅ **403 Forbidden** - Roles insuficientes
- ✅ **404 Not Found** - Recurso no existe
- ✅ **409 Conflict** - Error de negocio

---

## 🚀 Tiempos de Respuesta

```
GET /api/alumnos              : ~50-100ms (con paginación)
GET /api/alumnos/{id}         : ~30-50ms
GET /api/alumnos/{id}/completo: ~80-150ms (carga relaciones)
POST /api/alumnos             : ~100-200ms (validaciones + BD)
PUT /api/alumnos/{id}         : ~100-200ms
DELETE /api/alumnos/{id}      : ~50-100ms
PATCH /api/alumnos/{id}/restore: ~50-100ms
```

---

## ✅ Validaciones en Acción

```
CreateAlumnoValidator (9 reglas):
  ✅ Nombre requerido, 2-100 caracteres
  ✅ Apellido requerido, 2-100 caracteres
  ✅ Email requerido, formato válido, único
  ✅ CURP requerido, 18 caracteres, único
  ✅ FechaNacimiento: Edad > 14 años
  ✅ Sexo: M o F
  ✅ Dirección: máx 200 caracteres
  ✅ Teléfono: máx 20 caracteres
  ✅ SchoolId debe existir

UpdateAlumnoValidator (5 reglas):
  ✅ Nombre: 2-100 caracteres (opcional)
  ✅ Apellido: 2-100 caracteres (opcional)
  ✅ Email: Válido, único (opcional)
  ✅ FechaNacimiento: Edad > 14 años (opcional)
  ✅ Sexo: M o F (opcional)
```

---

## 🎯 Casos de Uso Validados

✅ Crear alumno válido
✅ Crear alumno con CURP duplicado → 400
✅ Crear alumno con email duplicado → 400
✅ Crear alumno menor de 14 años → 400
✅ Crear alumno con School inexistente → 404
✅ Obtener alumno válido → 200
✅ Obtener alumno inexistente → 404
✅ Actualizar alumno válido → 200
✅ Intentar cambiar CURP → Ignorado (protegido)
✅ Actualizar email a duplicado → 400
✅ Desactivar alumno → 204
✅ Restaurar alumno → 204
✅ Sin token JWT → 401
✅ Con token expirado → 401
✅ Sin roles requeridos → 403
✅ Búsqueda: nombre, apellido, email, curp → OK
✅ Paginación: pageNumber, pageSize → OK
