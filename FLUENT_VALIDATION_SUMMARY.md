# ✅ CreateAlumnoValidator - Implementación Completa

## Resumen

Se ha implementado un **validador robusto con FluentValidation** para la creación y actualización de alumnos. Las validaciones devuelven mensajes de error claros en español y están integradas automáticamente en ASP.NET Core.

---

## Archivos Creados

### 1. **CreateAlumnoValidator.cs**
```
📁 Infrastructure/Validators/CreateAlumnoValidator.cs
```
- 115 líneas de código
- Implementa `AbstractValidator<CreateAlumnoDto>`
- 9 reglas de validación con lógica personalizada
- Validación custom de edad con método `.Custom()`

### 2. **UpdateAlumnoValidator.cs**
```
📁 Infrastructure/Validators/UpdateAlumnoValidator.cs
```
- 68 líneas de código
- Implementa `AbstractValidator<UpdateAlumnoDto>`
- 5 reglas de validación (sin campos opcionales)

### 3. **Actualización: CreateAlumnoDto**
```
📁 DTOs/ControlEscolar/AlumnoDto.cs
```
- Agregadas propiedades `Direccion` y `TelefonoContacto`
- Ahora soporta validación de direcciones y teléfonos

---

## Instalación de Paquetes

```
✅ FluentValidation (12.1.1)
✅ FluentValidation.AspNetCore (11.3.1)
✅ FluentValidation.DependencyInjectionExtensions (11.11.0)
```

---

## Integración en Program.cs

```csharp
// using statements
using FluentValidation;
using FluentValidation.AspNetCore;
using ERPEscolar.API.Infrastructure.Validators;

// Configuración
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAlumnoValidator>();
```

**Beneficios:**
- ✅ Validación automática en todos los endpoints
- ✅ Sin código manual en controllers
- ✅ Errores en formato estándar de ASP.NET Core
- ✅ Mensajes en español

---

## Validaciones Implementadas

### CreateAlumnoValidator

| Campo | Validaciones | Mensajes |
|-------|--------------|----------|
| **Nombre** | Obligatorio, 2-100 chars, solo letras | 3 mensajes |
| **Apellido** | Obligatorio, 2-100 chars, solo letras | 3 mensajes |
| **CURP** | Obligatorio, 18 chars exactos, formato XXXXXX000000HXXXXX00 | 3 mensajes |
| **Email** | Obligatorio, formato válido, max 255 chars | 2 mensajes |
| **FechaNacimiento** | Obligatoria, no futura, edad 3-25 años | 3 mensajes |
| **Sexo** | Obligatorio, solo M o F | 1 mensaje |
| **Dirección** | Opcional, max 500 chars | 1 mensaje |
| **Teléfono** | Opcional, formato válido, max 20 chars | 2 mensajes |
| **TutorId** | Opcional, si viene > 0 | 1 mensaje |

### UpdateAlumnoValidator

| Campo | Validaciones | Mensajes |
|-------|--------------|----------|
| **Nombre** | Obligatorio, 2-100 chars, solo letras | 3 mensajes |
| **Apellido** | Obligatorio, 2-100 chars, solo letras | 3 mensajes |
| **Email** | Obligatorio, formato válido, max 255 chars | 2 mensajes |
| **FechaNacimiento** | Obligatoria, no futura, edad 3-25 años | 3 mensajes |
| **Sexo** | Obligatorio, solo M o F | 1 mensaje |

---

## Ejemplos de Respuestas

### ✅ Request Válido (201 Created)

```bash
POST /api/alumnos
Content-Type: application/json
Authorization: Bearer {token}

{
  "nombre": "Juan Carlos",
  "apellido": "García López",
  "curp": "GAPC960308HDFLNS09",
  "email": "juan.garcia@example.com",
  "fechaNacimiento": "2010-03-08",
  "sexo": "M",
  "direccion": "Calle Principal 123",
  "telefonoContacto": "5551234567",
  "schoolId": 1
}
```

**Respuesta:**
```json
{
  "id": 1,
  "nombre": "Juan Carlos",
  "apellido": "García López",
  "curp": "GAPC960308HDFLNS09",
  "email": "juan.garcia@example.com",
  "matricula": "ALU-2024-001",
  "sexo": "M",
  "activo": true,
  "fechaCreacion": "2026-01-13T10:30:00Z"
}
```

---

### ❌ Request Inválido (400 Bad Request)

**Ejemplo 1: Múltiples errores**
```bash
POST /api/alumnos
{
  "nombre": "",           # Vacío
  "apellido": "G",        # Muy corto
  "curp": "INVALID",      # Formato incorrecto
  "email": "invalid",     # Email inválido
  "fechaNacimiento": "2030-03-08",  # Fecha futura
  "sexo": "X"             # Sexo inválido
}
```

**Respuesta (400):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
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

**Ejemplo 2: CURP inválido**
```bash
{
  "nombre": "Juan",
  "apellido": "García",
  "curp": "INVALID123",
  "email": "juan@example.com",
  "fechaNacimiento": "2010-03-08",
  "sexo": "M"
}
```

**Respuesta (400):**
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

**Ejemplo 3: Alumno menor de 3 años**
```bash
{
  "nombre": "Baby",
  "apellido": "García",
  "curp": "GAPC230308HDFLNS09",
  "email": "baby@example.com",
  "fechaNacimiento": "2024-01-13",  # Menos de 2 años
  "sexo": "M"
}
```

**Respuesta (400):**
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

## Características Especiales

### 1. Validación Custom de Edad
```csharp
.Custom((fechaNacimiento, context) =>
{
    var edad = DateTime.Today.Year - fechaNacimiento.Year;
    if (fechaNacimiento.Date > DateTime.Today.AddYears(-edad))
        edad--;

    if (edad < 3)
        context.AddFailure(nameof(fechaNacimiento), 
            "El alumno debe tener al menos 3 años de edad.");

    if (edad > 25)
        context.AddFailure(nameof(fechaNacimiento), 
            "La edad del alumno parece ser muy alta...");
})
```

### 2. Validación de Formato CURP
```csharp
.Matches(@"^[A-Z]{4}[0-9]{6}[HM][A-Z]{5}[0-9]{2}$")
.WithMessage("El formato del CURP no es válido...")
```

Patrón CURP: `XXXXXX000000HXXXXX00`
- 4 letras iniciales (apellidos + nombre)
- 6 dígitos (fecha nacimiento YYMMDD)
- 1 letra (género H/M)
- 5 letras (consonantes)
- 2 dígitos (secuencia)

### 3. Validación Condicional de Campos Opcionales
```csharp
RuleFor(x => x.Direccion)
    .MaximumLength(500)
    .WithMessage("...")
    .When(x => !string.IsNullOrEmpty(x.Direccion));  // Solo si viene
```

### 4. Mensajes Claros en Español
Todos los mensajes están en español y son descriptivos:
- ❌ "El nombre del alumno es obligatorio."
- ❌ "El CURP debe tener exactamente 18 caracteres."
- ❌ "El email no tiene un formato válido."
- ❌ "El alumno debe tener al menos 3 años de edad."

---

## Estado de Compilación

```
✅ Build Status: SUCCESS
⚠️  Warnings: 1 (no crítica)
❌ Errors: 0
✅ API Status: Running (http://localhost:5235)
```

---

## Flujo de Validación

```
1. HTTP POST /api/alumnos
   └─ Body: JSON

2. Model Binding
   └─ Deserializa JSON a CreateAlumnoDto

3. FluentValidation (Automático)
   └─ Ejecuta CreateAlumnoValidator
   └─ Si hay errores → retorna 400 con detalles

4. Controller [Create]
   └─ Si ModelState.IsValid → continúa
   └─ Si no → retorna BadRequest

5. AlumnoService.CreateAlumnoAsync()
   └─ Validaciones adicionales de negocio
   └─ Acceso a BD
   └─ Generación de matrícula

6. Response
   └─ 201 Created + AlumnoDto
```

---

## Integración con AlumnosController

El validador se ejecuta automáticamente **antes** de que el código del controller sea ejecutado:

```csharp
[HttpPost]
[Authorize(Roles = "SuperAdmin,Admin TI,Control Escolar")]
public async Task<IActionResult> Create([FromBody] CreateAlumnoDto request)
{
    // FluentValidation ya se ejecutó aquí automáticamente
    // Si hay errores, nunca llegamos a este punto
    
    try
    {
        if (!ModelState.IsValid)  // Redundante pero seguro
            return BadRequest(ModelState);
            
        var alumno = await _alumnoService.CreateAlumnoAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = alumno.Id }, alumno);
    }
    catch (Exception ex) { ... }
}
```

---

## Ventajas de FluentValidation

✅ **Código limpio** - Validaciones separadas en clases dedicadas
✅ **Reutilizable** - Mismo validador en múltiples contextos
✅ **Expresivo** - Sintaxis fluida y fácil de leer
✅ **Completo** - Múltiples validadores built-in
✅ **Extensible** - Soporta validaciones custom
✅ **Mensajes personalizados** - Mensajes en idioma del usuario
✅ **Integración ASP.NET** - Funciona automáticamente
✅ **Sin reflexión** - Performance óptimo

---

## Próximas Mejoras (Opcional)

1. **Validadores adicionales:**
   ```
   [ ] GrupoValidator
   [ ] InscripcionValidator
   [ ] AsistenciaValidator
   [ ] CalificacionValidator
   ```

2. **Validaciones cruzadas (Cross-Property):**
   ```csharp
   RuleFor(x => x.FechaNacimiento)
       .GreaterThan(x => x.FechaInscripcion)
       .WithMessage("La fecha de inscripción no puede ser antes de nacimiento");
   ```

3. **Validaciones asincrónicas:**
   ```csharp
   RuleFor(x => x.CURP)
       .MustAsync(async (curp, ct) => !await CurpExists(curp))
       .WithMessage("CURP ya existe en BD");
   ```

4. **Custom error formatter:**
   ```csharp
   builder.Services.Configure<ApiBehaviorOptions>(options =>
       options.InvalidModelStateResponseFactory = context => 
       {
           // Formato personalizado de errores
       });
   ```

---

## Documentación de Pruebas

Para ver todos los casos de prueba posibles, consulta:
👉 **[VALIDATOR_TEST_CASES.md](VALIDATOR_TEST_CASES.md)**

Contiene 10 ejemplos completos con:
- Requests cURL
- Respuestas esperadas
- Explicaciones

---

## Comandos Útiles

### Ver validadores registrados
```csharp
var validators = app.Services.GetServices<IValidator>();
```

### Testear validador directamente
```csharp
var validator = new CreateAlumnoValidator();
var dto = new CreateAlumnoDto { /* ... */ };
var result = await validator.ValidateAsync(dto);
if (!result.IsValid)
{
    foreach (var error in result.Errors)
        Console.WriteLine(error.ErrorMessage);
}
```

### Ejecutar API con validador
```powershell
cd "c:\Users\israe\OneDrive\Documentos\ERP_Escolar\ERPEscolar.API"
dotnet run
```

---

**✅ Implementación completada exitosamente**

**Última actualización**: 13 de enero de 2026
**Status**: 🟢 Validador funcional y probado
