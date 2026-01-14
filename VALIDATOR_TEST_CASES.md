# Pruebas del CreateAlumnoValidator

## Descripción
El `CreateAlumnoValidator` está integrado con ASP.NET Core y se ejecuta automáticamente cuando se envía una request POST a `/api/alumnos`.

Las validaciones devuelven mensajes de error claros en español.

---

## Casos de Prueba

### 1. ✅ Crear Alumno - VÁLIDO

```bash
curl -X POST http://localhost:5235/api/alumnos \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "nombre": "Juan Carlos",
    "apellido": "García López",
    "curp": "GAPC960308HDFLNS09",
    "email": "juan.garcia@example.com",
    "fechaNacimiento": "2010-03-08",
    "sexo": "M",
    "direccion": "Calle Principal 123",
    "telefonoContacto": "5551234567",
    "schoolId": 1,
    "tutorId": null
  }'
```

**Respuesta (201 Created):**
```json
{
  "id": 1,
  "nombre": "Juan Carlos",
  "apellido": "García López",
  "curp": "GAPC960308HDFLNS09",
  "email": "juan.garcia@example.com",
  "matricula": "ALU-2024-001",
  "fechaNacimiento": "2010-03-08",
  "sexo": "M",
  "activo": true,
  "fechaCreacion": "2026-01-13T10:30:00Z"
}
```

---

### 2. ❌ Nombre vacío

```bash
curl -X POST http://localhost:5235/api/alumnos \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "nombre": "",
    "apellido": "García",
    "curp": "GAPC960308HDFLNS09",
    "email": "juan@example.com",
    "fechaNacimiento": "2010-03-08",
    "sexo": "M",
    "schoolId": 1
  }'
```

**Respuesta (400 Bad Request):**
```json
{
  "errors": {
    "Nombre": [
      "El nombre del alumno es obligatorio."
    ]
  },
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "..."
}
```

---

### 3. ❌ Nombre muy corto (menos de 2 caracteres)

```bash
curl -X POST http://localhost:5235/api/alumnos \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "nombre": "J",
    "apellido": "García",
    "curp": "GAPC960308HDFLNS09",
    "email": "juan@example.com",
    "fechaNacimiento": "2010-03-08",
    "sexo": "M",
    "schoolId": 1
  }'
```

**Respuesta (400 Bad Request):**
```json
{
  "errors": {
    "Nombre": [
      "El nombre debe tener al menos 2 caracteres."
    ]
  }
}
```

---

### 4. ❌ CURP inválido (formato incorrecto)

```bash
curl -X POST http://localhost:5235/api/alumnos \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "nombre": "Juan",
    "apellido": "García",
    "curp": "INVALID123",
    "email": "juan@example.com",
    "fechaNacimiento": "2010-03-08",
    "sexo": "M",
    "schoolId": 1
  }'
```

**Respuesta (400 Bad Request):**
```json
{
  "errors": {
    "CURP": [
      "El CURP debe tener exactamente 18 caracteres.",
      "El formato del CURP no es válido. Debe seguir el patrón: XXXXXX000000HXXXXX00"
    ]
  }
}
```

---

### 5. ❌ Email inválido

```bash
curl -X POST http://localhost:5235/api/alumnos \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "nombre": "Juan",
    "apellido": "García",
    "curp": "GAPC960308HDFLNS09",
    "email": "email_invalido",
    "fechaNacimiento": "2010-03-08",
    "sexo": "M",
    "schoolId": 1
  }'
```

**Respuesta (400 Bad Request):**
```json
{
  "errors": {
    "Email": [
      "El email no tiene un formato válido."
    ]
  }
}
```

---

### 6. ❌ Fecha de nacimiento en el futuro

```bash
curl -X POST http://localhost:5235/api/alumnos \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "nombre": "Juan",
    "apellido": "García",
    "curp": "GAPC960308HDFLNS09",
    "email": "juan@example.com",
    "fechaNacimiento": "2030-03-08",
    "sexo": "M",
    "schoolId": 1
  }'
```

**Respuesta (400 Bad Request):**
```json
{
  "errors": {
    "FechaNacimiento": [
      "La fecha de nacimiento no puede ser en el futuro."
    ]
  }
}
```

---

### 7. ❌ Alumno muy joven (menor de 3 años)

```bash
curl -X POST http://localhost:5235/api/alumnos \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "nombre": "Baby",
    "apellido": "García",
    "curp": "GAPC230308HDFLNS09",
    "email": "baby@example.com",
    "fechaNacimiento": "2023-08-08",
    "sexo": "M",
    "schoolId": 1
  }'
```

**Respuesta (400 Bad Request):**
```json
{
  "errors": {
    "FechaNacimiento": [
      "El alumno debe tener al menos 3 años de edad."
    ]
  }
}
```

---

### 8. ❌ Sexo inválido

```bash
curl -X POST http://localhost:5235/api/alumnos \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "nombre": "Juan",
    "apellido": "García",
    "curp": "GAPC960308HDFLNS09",
    "email": "juan@example.com",
    "fechaNacimiento": "2010-03-08",
    "sexo": "X",
    "schoolId": 1
  }'
```

**Respuesta (400 Bad Request):**
```json
{
  "errors": {
    "Sexo": [
      "El sexo debe ser 'M' (Masculino) o 'F' (Femenino)."
    ]
  }
}
```

---

### 9. ❌ Teléfono con caracteres inválidos

```bash
curl -X POST http://localhost:5235/api/alumnos \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "nombre": "Juan",
    "apellido": "García",
    "curp": "GAPC960308HDFLNS09",
    "email": "juan@example.com",
    "fechaNacimiento": "2010-03-08",
    "sexo": "M",
    "telefonoContacto": "555@@@!!!",
    "schoolId": 1
  }'
```

**Respuesta (400 Bad Request):**
```json
{
  "errors": {
    "TelefonoContacto": [
      "El formato del teléfono no es válido. Solo se permiten números, +, -, espacios y paréntesis."
    ]
  }
}
```

---

### 10. ❌ Múltiples errores de validación

```bash
curl -X POST http://localhost:5235/api/alumnos \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "nombre": "",
    "apellido": "G",
    "curp": "INVALID",
    "email": "email_invalido",
    "fechaNacimiento": "2030-03-08",
    "sexo": "X",
    "schoolId": 1
  }'
```

**Respuesta (400 Bad Request):**
```json
{
  "errors": {
    "Nombre": [
      "El nombre del alumno es obligatorio."
    ],
    "Apellido": [
      "El apellido debe tener al menos 2 caracteres."
    ],
    "CURP": [
      "El CURP debe tener exactamente 18 caracteres.",
      "El formato del CURP no es válido. Debe seguir el patrón: XXXXXX000000HXXXXX00"
    ],
    "Email": [
      "El email no tiene un formato válido."
    ],
    "FechaNacimiento": [
      "La fecha de nacimiento no puede ser en el futuro."
    ],
    "Sexo": [
      "El sexo debe ser 'M' (Masculino) o 'F' (Femenino)."
    ]
  }
}
```

---

## Características del Validador

✅ **Mensajes claros en español** - Todos los mensajes están en español y son descriptivos
✅ **Validaciones automáticas** - Se ejecutan sin código adicional en el controller
✅ **Reglas de negocio** - Validaciones específicas del dominio (edad mínima, CURP, etc.)
✅ **Campos opcionales** - Soporta campos opcionales como dirección y teléfono
✅ **Expresiones regulares** - Validación de formato para CURP, email y teléfono
✅ **Validaciones personalizadas** - Cálculo de edad con validación custom

---

## Reglas Implementadas

| Campo | Reglas | Mensajes |
|-------|--------|----------|
| **Nombre** | Obligatorio, 2-100 caracteres, solo letras | 3 mensajes diferentes |
| **Apellido** | Obligatorio, 2-100 caracteres, solo letras | 3 mensajes diferentes |
| **CURP** | Obligatorio, exactamente 18 caracteres, formato XXXXXX000000HXXXXX00 | 3 mensajes diferentes |
| **Email** | Obligatorio, formato válido, máximo 255 caracteres | 2 mensajes diferentes |
| **FechaNacimiento** | Obligatoria, no futura, edad 3-25 años | 3 mensajes diferentes |
| **Sexo** | Obligatorio, solo M o F | 1 mensaje |
| **Dirección** | Opcional, máximo 500 caracteres | 1 mensaje |
| **Teléfono** | Opcional, formato válido, máximo 20 caracteres | 2 mensajes diferentes |
| **TutorId** | Opcional, si viene debe ser > 0 | 1 mensaje |

---

**Última actualización**: [Sesión actual]
**Status**: ✅ Validador completamente funcional
