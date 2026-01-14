# Ejemplos de Uso - API Alumnos

## 1. Autenticación (Prerequisito)

### Login
```http
POST http://localhost:5235/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "password"
}
```

**Respuesta (201):**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresIn": 3600
  }
}
```

**Para los siguientes requests, usar el `accessToken` en el header:**
```
Authorization: Bearer {accessToken}
```

---

## 2. Operaciones CRUD de Alumnos

### 2.1 Crear Alumno (POST)

```http
POST http://localhost:5235/api/alumnos
Content-Type: application/json
Authorization: Bearer {token}

{
  "nombre": "Juan",
  "apellido": "García",
  "curp": "GAPC960308HDFLNS09",
  "email": "juan.garcia@example.com",
  "fechaNacimiento": "1996-03-08",
  "genero": "M",
  "direccion": "Calle Principal 123",
  "telefonoContacto": "5551234567",
  "tutorId": null
}
```

**Respuesta (201 Created):**
```json
{
  "id": 1,
  "nombre": "Juan",
  "apellido": "García",
  "curp": "GAPC960308HDFLNS09",
  "email": "juan.garcia@example.com",
  "matricula": "ALU-2024-001",
  "fechaNacimiento": "1996-03-08",
  "genero": "M",
  "direccion": "Calle Principal 123",
  "telefonoContacto": "5551234567",
  "activo": true,
  "fechaCreacion": "2024-01-15T10:30:00Z",
  "tutorId": null
}
```

**Errores posibles:**
- **400 Bad Request**: Validación fallida (campos faltantes)
  ```json
  {
    "message": "CURP debe tener exactamente 18 caracteres",
    "errors": ["CURP validation failed"]
  }
  ```

- **409 Conflict**: CURP o Email ya existen
  ```json
  {
    "message": "Ya existe un alumno con este CURP"
  }
  ```

---

### 2.2 Obtener un Alumno (GET)

```http
GET http://localhost:5235/api/alumnos/1
Authorization: Bearer {token}
```

**Respuesta (200 OK):**
```json
{
  "id": 1,
  "nombre": "Juan",
  "apellido": "García",
  "curp": "GAPC960308HDFLNS09",
  "email": "juan.garcia@example.com",
  "matricula": "ALU-2024-001",
  "fechaNacimiento": "1996-03-08",
  "genero": "M",
  "direccion": "Calle Principal 123",
  "telefonoContacto": "5551234567",
  "activo": true,
  "fechaCreacion": "2024-01-15T10:30:00Z",
  "tutorId": null
}
```

**Error (404 Not Found):**
```json
{
  "message": "Alumno no encontrado"
}
```

---

### 2.3 Obtener Alumno Completo (GET)

Incluye tutores e inscripciones relacionadas:

```http
GET http://localhost:5235/api/alumnos/1/completo
Authorization: Bearer {token}
```

**Respuesta (200 OK):**
```json
{
  "id": 1,
  "nombre": "Juan",
  "apellido": "García",
  "curp": "GAPC960308HDFLNS09",
  "email": "juan.garcia@example.com",
  "matricula": "ALU-2024-001",
  "fechaNacimiento": "1996-03-08",
  "genero": "M",
  "direccion": "Calle Principal 123",
  "telefonoContacto": "5551234567",
  "activo": true,
  "fechaCreacion": "2024-01-15T10:30:00Z",
  "tutorId": null,
  "tutores": [
    {
      "id": 5,
      "nombre": "María",
      "apellido": "García",
      "email": "maria.garcia@example.com",
      "telefono": "5559876543",
      "parentesco": "Madre"
    }
  ],
  "inscripciones": [
    {
      "id": 10,
      "grupoId": 2,
      "grupoNombre": "1ro A",
      "matricula": "ALU-2024-001",
      "fechaInscripcion": "2024-01-15T10:30:00Z",
      "activo": true
    }
  ]
}
```

---

### 2.4 Listar Alumnos (GET con Paginación)

```http
GET http://localhost:5235/api/alumnos?pageNumber=1&pageSize=10&searchTerm=juan
Authorization: Bearer {token}
```

**Parámetros:**
- `pageNumber`: Número de página (default: 1)
- `pageSize`: Registros por página (default: 10, máximo: 100)
- `searchTerm`: Busca en nombre, apellido, email, CURP, matrícula (opcional)

**Respuesta (200 OK):**
```json
{
  "data": [
    {
      "id": 1,
      "nombre": "Juan",
      "apellido": "García",
      "curp": "GAPC960308HDFLNS09",
      "email": "juan.garcia@example.com",
      "matricula": "ALU-2024-001",
      "fechaNacimiento": "1996-03-08",
      "genero": "M",
      "direccion": "Calle Principal 123",
      "telefonoContacto": "5551234567",
      "activo": true,
      "fechaCreacion": "2024-01-15T10:30:00Z",
      "tutorId": null
    }
  ],
  "totalRecords": 1,
  "totalPages": 1,
  "currentPage": 1,
  "pageSize": 10
}
```

---

### 2.5 Actualizar Alumno (PUT)

```http
PUT http://localhost:5235/api/alumnos/1
Content-Type: application/json
Authorization: Bearer {token}

{
  "nombre": "Juan Carlos",
  "apellido": "García López",
  "email": "juancarlos.garcia@example.com",
  "fechaNacimiento": "1996-03-08",
  "genero": "M",
  "direccion": "Calle Nueva 456",
  "telefonoContacto": "5559999888",
  "tutorId": null
}
```

**Nota:** CURP y Matrícula no se pueden actualizar

**Respuesta (200 OK):**
```json
{
  "id": 1,
  "nombre": "Juan Carlos",
  "apellido": "García López",
  "curp": "GAPC960308HDFLNS09",
  "email": "juancarlos.garcia@example.com",
  "matricula": "ALU-2024-001",
  "fechaNacimiento": "1996-03-08",
  "genero": "M",
  "direccion": "Calle Nueva 456",
  "telefonoContacto": "5559999888",
  "activo": true,
  "fechaCreacion": "2024-01-15T10:30:00Z",
  "fechaActualizacion": "2024-01-15T11:45:00Z",
  "tutorId": null
}
```

**Errores:**
- **400 Bad Request**: Validación fallida
- **404 Not Found**: Alumno no existe
- **409 Conflict**: Email duplicado en otro alumno

---

### 2.6 Desactivar Alumno (DELETE)

```http
DELETE http://localhost:5235/api/alumnos/1
Authorization: Bearer {token}
```

**Respuesta (204 No Content)** - Sin body

**Nota:** Soft delete - El alumno sigue en BD pero con `activo = false`

---

### 2.7 Reactivar Alumno (PATCH)

```http
PATCH http://localhost:5235/api/alumnos/1/restore
Authorization: Bearer {token}
```

**Respuesta (204 No Content)** - Sin body

**Nota:** Reactiva un alumno desactivado

---

## 3. Ejemplos con cURL

### Crear Alumno
```bash
curl -X POST http://localhost:5235/api/alumnos \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "nombre": "Pedro",
    "apellido": "López",
    "curp": "LOPP850620HDFRPD04",
    "email": "pedro.lopez@example.com",
    "fechaNacimiento": "1985-06-20",
    "genero": "M",
    "direccion": "Calle B 789",
    "telefonoContacto": "5554321098",
    "tutorId": null
  }'
```

### Listar Alumnos con búsqueda
```bash
curl -X GET "http://localhost:5235/api/alumnos?pageNumber=1&pageSize=5&searchTerm=pere" \
  -H "Authorization: Bearer {token}"
```

### Obtener Alumno específico
```bash
curl -X GET http://localhost:5235/api/alumnos/1 \
  -H "Authorization: Bearer {token}"
```

### Actualizar
```bash
curl -X PUT http://localhost:5235/api/alumnos/1 \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "nombre": "Pedro Updated",
    "apellido": "López",
    "email": "pedro.updated@example.com",
    "fechaNacimiento": "1985-06-20",
    "genero": "M",
    "direccion": "Nueva Dirección",
    "telefonoContacto": "5559999777",
    "tutorId": null
  }'
```

### Desactivar
```bash
curl -X DELETE http://localhost:5235/api/alumnos/1 \
  -H "Authorization: Bearer {token}"
```

### Reactivar
```bash
curl -X PATCH http://localhost:5235/api/alumnos/1/restore \
  -H "Authorization: Bearer {token}"
```

---

## 4. Notas Importantes

### Autorizaciones
- **Lectura**: Cualquier usuario autenticado
- **Creación/Actualización/Eliminación**: SuperAdmin, Admin TI, Control Escolar

### Validaciones
| Campo | Regla |
|-------|-------|
| Nombre | Requerido |
| Apellido | Requerido |
| CURP | 18 caracteres, único |
| Email | Formato válido, único |
| FechaNacimiento | Edad mínima 3 años |
| Género | M o F |

### Matrícula
- Auto-generada: `ALU-{año}-{secuencia}`
- Ejemplo: `ALU-2024-001`
- No puede modificarse

### Paginación
- **pageNumber**: 1-indexed (1, 2, 3...)
- **pageSize**: 1-100 registros
- **searchTerm**: Case-insensitive, busca 5 campos

### Soft Delete
- No se eliminan registros
- Se marca `activo = false`
- Pueden restaurarse con PATCH /restore

---

**Última actualización**: [Sesión actual]
