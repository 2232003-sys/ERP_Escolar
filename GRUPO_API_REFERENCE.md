# GrupoService API Reference

## Base URL
```
https://localhost:5235/api/grupos
```

## Authentication
All endpoints require JWT Bearer token:
```
Authorization: Bearer {access_token}
```

---

## Endpoints

### 1. List Grupos (Paginated)

**Request**
```http
GET /api/grupos?pageNumber=1&pageSize=10&searchTerm=1ro
Authorization: Bearer {token}
```

**Query Parameters**
- `pageNumber` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Items per page (default: 10, max: 100)
- `searchTerm` (string, optional): Search by Nombre, Grado, or Seccion

**Response** - 200 OK
```json
{
  "items": [
    {
      "id": 1,
      "schoolId": 1,
      "cicloEscolarId": 1,
      "nombre": "1ro A",
      "grado": "1ro",
      "seccion": "A",
      "docenteTutorId": 5,
      "capacidadMaxima": 35,
      "activo": true,
      "fechaCreacion": "2026-01-13T22:30:00Z"
    }
  ],
  "totalItems": 1,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 1
}
```

**Error Response** - 500 Internal Server Error
```json
{
  "message": "Error al obtener grupos."
}
```

---

### 2. Get Grupo by ID

**Request**
```http
GET /api/grupos/1
Authorization: Bearer {token}
```

**Response** - 200 OK
```json
{
  "id": 1,
  "schoolId": 1,
  "cicloEscolarId": 1,
  "nombre": "1ro A",
  "grado": "1ro",
  "seccion": "A",
  "docenteTutorId": 5,
  "capacidadMaxima": 35,
  "activo": true,
  "fechaCreacion": "2026-01-13T22:30:00Z"
}
```

**Error Response** - 404 Not Found
```json
{
  "message": "Grupo with Id 999 not found"
}
```

---

### 3. Get Grupo with Full Data

**Request**
```http
GET /api/grupos/1/completo
Authorization: Bearer {token}
```

**Response** - 200 OK
```json
{
  "id": 1,
  "schoolId": 1,
  "cicloEscolarId": 1,
  "nombre": "1ro A",
  "grado": "1ro",
  "seccion": "A",
  "docenteTutorId": 5,
  "capacidadMaxima": 35,
  "activo": true,
  "fechaCreacion": "2026-01-13T22:30:00Z",
  "cicloNombre": "Ciclo 2025-2026",
  "docenteTutorNombre": "Juan Pérez García",
  "inscripcionesActivas": 28
}
```

**Error Response** - 404 Not Found
```json
{
  "message": "Grupo with Id 999 not found"
}
```

---

### 4. Create Grupo

**Request**
```http
POST /api/grupos
Content-Type: application/json
Authorization: Bearer {token}
X-Required-Roles: SuperAdmin, Admin TI, Control Escolar

{
  "schoolId": 1,
  "cicloEscolarId": 1,
  "nombre": "1ro A",
  "grado": "1ro",
  "seccion": "A",
  "docenteTutorId": 5,
  "capacidadMaxima": 35
}
```

**Request Body**
- `schoolId` (int, required): School identifier
- `cicloEscolarId` (int, required): Academic cycle identifier
- `nombre` (string, required): Group name (1-100 characters)
- `grado` (string, required): Grade level (1-50 characters)
- `seccion` (string, required): Section identifier (1-50 characters)
- `docenteTutorId` (int, optional): Tutor teacher ID
- `capacidadMaxima` (int, required): Maximum capacity (1-200)

**Response** - 201 Created
```json
{
  "id": 1,
  "schoolId": 1,
  "cicloEscolarId": 1,
  "nombre": "1ro A",
  "grado": "1ro",
  "seccion": "A",
  "docenteTutorId": 5,
  "capacidadMaxima": 35,
  "activo": true,
  "fechaCreacion": "2026-01-13T22:30:00Z"
}
```

**Error Response** - 400 Bad Request (Validation Error)
```json
{
  "message": "One or more validation errors occurred.",
  "errors": {
    "nombre": [
      "El nombre del grupo es obligatorio."
    ],
    "capacidadMaxima": [
      "La capacidad máxima debe ser al menos 1."
    ]
  }
}
```

**Error Response** - 409 Conflict (Business Logic)
```json
{
  "message": "Ya existe un grupo activo con Grado '1ro' y Sección 'A' en este ciclo escolar."
}
```

**Error Response** - 404 Not Found (School or CicloEscolar)
```json
{
  "message": "School with Id 999 not found"
}
```

---

### 5. Update Grupo

**Request**
```http
PUT /api/grupos/1
Content-Type: application/json
Authorization: Bearer {token}
X-Required-Roles: SuperAdmin, Admin TI, Control Escolar

{
  "nombre": "1ro A Actualizado",
  "grado": "1ro",
  "seccion": "A",
  "docenteTutorId": 6,
  "capacidadMaxima": 40
}
```

**Request Body**
- `nombre` (string, required): Group name (1-100 characters)
- `grado` (string, required): Grade level (1-50 characters)
- `seccion` (string, required): Section identifier (1-50 characters)
- `docenteTutorId` (int, optional): Tutor teacher ID
- `capacidadMaxima` (int, required): Maximum capacity (1-200)

**Response** - 200 OK
```json
{
  "id": 1,
  "schoolId": 1,
  "cicloEscolarId": 1,
  "nombre": "1ro A Actualizado",
  "grado": "1ro",
  "seccion": "A",
  "docenteTutorId": 6,
  "capacidadMaxima": 40,
  "activo": true,
  "fechaCreacion": "2026-01-13T22:30:00Z"
}
```

**Error Response** - 400 Bad Request
```json
{
  "message": "One or more validation errors occurred.",
  "errors": {
    "capacidadMaxima": [
      "La capacidad máxima debe ser al menos 1."
    ]
  }
}
```

**Error Response** - 409 Conflict
```json
{
  "message": "Ya existe otro grupo activo con Grado '1ro' y Sección 'A' en este ciclo escolar."
}
```

**Error Response** - 404 Not Found
```json
{
  "message": "Grupo with Id 999 not found"
}
```

---

### 6. Soft Delete Grupo

**Request**
```http
DELETE /api/grupos/1
Authorization: Bearer {token}
X-Required-Roles: SuperAdmin, Admin TI, Control Escolar
```

**Response** - 204 No Content
(No response body)

**Error Response** - 404 Not Found
```json
{
  "message": "Grupo with Id 999 not found"
}
```

---

### 7. Restore Grupo

**Request**
```http
PATCH /api/grupos/1/restore
Authorization: Bearer {token}
X-Required-Roles: SuperAdmin, Admin TI, Control Escolar
```

**Response** - 204 No Content
(No response body)

**Error Response** - 404 Not Found
```json
{
  "message": "Grupo with Id 999 not found"
}
```

---

## Validation Rules

### CreateGrupoDto / UpdateGrupoDto

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| schoolId | int | ✓ | > 0 |
| cicloEscolarId | int | ✓ | > 0 |
| nombre | string | ✓ | 1-100 chars |
| grado | string | ✓ | 1-50 chars |
| seccion | string | ✓ | 1-50 chars |
| docenteTutorId | int? | ✗ | > 0 if provided |
| capacidadMaxima | int | ✓ | 1-200 |

### Uniqueness Constraints

1. **By Grade and Section**
   - Only one grupo per (SchoolId, CicloEscolarId, Grado, Seccion)
   - Only considers active grupos (Activo = true)

2. **By Name**
   - Only one grupo per (SchoolId, CicloEscolarId, Nombre)
   - Only considers active grupos (Activo = true)

### DocenteTutor Validation

- Must exist in system
- Must belong to same school (SchoolId match)
- Must be active (Activo = true)

---

## Error Codes

| Code | Status | Meaning |
|------|--------|---------|
| 200 | OK | Success |
| 201 | Created | Resource created |
| 204 | No Content | Success (no body) |
| 400 | Bad Request | Validation error |
| 401 | Unauthorized | Missing/invalid token |
| 403 | Forbidden | Insufficient permissions |
| 404 | Not Found | Resource not found |
| 409 | Conflict | Business logic violation |
| 500 | Internal Error | Server error |

---

## Authorization

### Required Roles for Write Operations
- `SuperAdmin`
- `Admin TI`
- `Control Escolar`

### All Authenticated Users Can
- List grupos
- View individual grupos
- View detailed data

---

## Example cURL Commands

### List Grupos
```bash
curl -X GET "https://localhost:5235/api/grupos?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json"
```

### Get Single Grupo
```bash
curl -X GET "https://localhost:5235/api/grupos/1" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json"
```

### Create Grupo
```bash
curl -X POST "https://localhost:5235/api/grupos" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "schoolId": 1,
    "cicloEscolarId": 1,
    "nombre": "1ro A",
    "grado": "1ro",
    "seccion": "A",
    "docenteTutorId": 5,
    "capacidadMaxima": 35
  }'
```

### Update Grupo
```bash
curl -X PUT "https://localhost:5235/api/grupos/1" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "1ro A Actualizado",
    "grado": "1ro",
    "seccion": "A",
    "docenteTutorId": 6,
    "capacidadMaxima": 40
  }'
```

### Delete Grupo
```bash
curl -X DELETE "https://localhost:5235/api/grupos/1" \
  -H "Authorization: Bearer {token}"
```

### Restore Grupo
```bash
curl -X PATCH "https://localhost:5235/api/grupos/1/restore" \
  -H "Authorization: Bearer {token}"
```

---

## Response Headers

All responses include standard HTTP headers:
- `Content-Type: application/json`
- `Date: {timestamp}`
- `Server: Kestrel`

---

## Notes

- All timestamps are in UTC format (ISO 8601)
- Pagination uses 1-based indexing
- Soft-deleted grupos cannot be retrieved (Activo = false)
- Updating SchoolId or CicloEscolarId is not allowed
- All error messages are in Spanish

---

**Last Updated:** January 13, 2026
**Version:** 1.0
**Status:** Production Ready
