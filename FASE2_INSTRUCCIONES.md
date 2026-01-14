# ▶️ Instrucciones para Continuar - Sesión 2

## Estado Actual

✅ **Fase 1 completada**: Arquitectura + BD + Autenticación JWT

La API está **funcional y corriendo** en `http://localhost:5235`

---

## 📋 Próxima sesión: FASE 2 - Control Escolar (2-3 días)

### 1️⃣ Empezar con **Alumnos** (AlumnosService + AlumnosController)

#### Crear DTOs primero
```csharp
// DTOs/ControlEscolar/AlumnoDto.cs
- AlumnoDto (respuesta GET)
- CreateAlumnoDto (POST)
- UpdateAlumnoDto (PUT)
- AlumnoFullDataDto (con tutor + inscripciones)
```

#### Crear AlumnoService
```csharp
// Infrastructure/Services/AlumnoService.cs
- CreateAlumnoAsync()
- GetAlumnoWithFullDataAsync()
- GetAlumnosWithPaginationAsync(pageNumber, pageSize, searchTerm)
- UpdateAlumnoAsync()
- ValidateMatriculaUniqueAsync()
- GenerateMatriculaAsync()
```

#### Crear AlumnoController
```csharp
// Features/ControlEscolar/AlumnosController.cs
GET    /api/alumnos
GET    /api/alumnos/{id}
GET    /api/alumnos/{id}/completo
POST   /api/alumnos
PUT    /api/alumnos/{id}
DELETE /api/alumnos/{id}
```

### 2️⃣ Luego **Grupos** (GrupoService + GrupoController)

- Crear grupo con docente tutor
- Listar alumnos por grupo
- Horarios (próxima fase)

### 3️⃣ **Inscripciones**

- Matricular alumno
- Cambiar de grupo
- Desmatricular

### 4️⃣ **Asistencias**

- Registrar por fecha y grupo
- Batch upload
- Reportes

### 5️⃣ **Calificaciones**

- Registrar calificación
- Cálculo de promedios
- Generación de boletas

---

## 🎨 Patrón a Seguir

```csharp
// 1. DTO
public class CreateAlumnoDto
{
    [Required]
    public string Nombre { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    // ...
}

// 2. Service
public class AlumnoService
{
    private readonly AppDbContext _context;
    private readonly IRepository<Alumno> _repository;
    
    public async Task<AlumnoDto> CreateAlumnoAsync(CreateAlumnoDto request)
    {
        // Validar
        // Crear
        // Mapear a DTO
        // Retornar
    }
}

// 3. Controller
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,ControlEscolar")]
public class AlumnosController : ControllerBase
{
    private readonly AlumnoService _service;
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAlumnoDto request)
    {
        var result = await _service.CreateAlumnoAsync(request);
        return Created($"api/alumnos/{result.Id}", result);
    }
}
```

---

## 🛠️ Stack para Fase 2

### Librerías a instalar
```bash
dotnet add package FluentValidation  # Validaciones
dotnet add package AutoMapper        # Mapeos DTO
dotnet add package Serilog           # Logging
```

### Mejoras de código
1. **Mappers**: Usar AutoMapper para DTO-Entity
2. **Validators**: FluentValidation para validar requests
3. **Logging**: Serilog para logs estructurados
4. **Error handling**: GlobalExceptionHandler middleware

---

## 📂 Estructura de archivos a crear

```
Features/ControlEscolar/
├── AlumnosController.cs
├── GruposController.cs
├── InscripcionesController.cs
├── AsistenciasController.cs
└── CalificacionesController.cs

DTOs/ControlEscolar/
├── AlumnoDto.cs
├── GrupoDto.cs
├── InscripcionDto.cs
├── AsistenciaDto.cs
└── CalificacionDto.cs

Infrastructure/Services/
├── AlumnoService.cs
├── GrupoService.cs
├── InscripcionService.cs
├── AsistenciaService.cs
└── CalificacionService.cs

Validators/ControlEscolar/
├── CreateAlumnoValidator.cs
├── CreateGrupoValidator.cs
└── ...
```

---

## 🧪 Testing (Antes de terminar Fase 2)

```bash
# Probar cada endpoint
curl -X GET "https://localhost:5001/api/alumnos" \
  -H "Authorization: Bearer $TOKEN" \
  --insecure

# En Postman:
1. Login para obtener token
2. Guardar en variable {{token}}
3. Testear cada endpoint
4. Verificar respuestas correctas
5. Probar casos edge (alumno duplicado, etc)
```

---

## 📈 Checklist Fase 2

- [ ] DTOs creados y documentados
- [ ] Services implementados con lógica de negocio
- [ ] Controllers con autorizaciones apropiadas
- [ ] Validaciones FluentValidation
- [ ] Mappers AutoMapper
- [ ] Logging en puntos clave
- [ ] Error handling global
- [ ] Tests manuales en Postman
- [ ] Documentación en Swagger actualizada

---

## 🚀 Comando para compilar y testear

```bash
# Terminal 1: Compilar
cd ERPEscolar.API
dotnet build

# Terminal 2: Ejecutar
dotnet run

# Terminal 3: Testear con curl o Postman
# Ir a http://localhost:5235/swagger
```

---

## 💡 Tips para Fase 2

1. **Test early**: No esperes a terminar todo para testear
2. **Use Swagger**: Ve los cambios en tiempo real
3. **Seed data**: Ya tienes 10 alumnos, úsalos para probar
4. **Paginación**: Importante para grandes datasets
5. **Búsqueda**: Implementar filtros útiles
6. **Performance**: Index en campos de búsqueda

---

## 📞 Referencia Rápida

| Archivo | Ubicación | Propósito |
|---------|-----------|----------|
| Program.cs | Root | Config servicios |
| AppDbContext.cs | Data/ | Mapeo de tablas |
| User.cs, Rol.cs | Models/ | Entidades |
| AuthService.cs | Infrastructure/Services/ | Lógica auth |
| AuthController.cs | Features/Auth/ | Endpoints auth |
| appsettings.json | Root | Config BD, JWT |

---

## 🎯 Meta para Fase 2

Tener **5 nuevos controllers funcionales** con:
- ✅ CRUD básico
- ✅ Validaciones
- ✅ Autorización por rol
- ✅ Logging
- ✅ Error handling
- ✅ Documentación Swagger

---

## 📚 Recursos

- [FluentValidation Docs](https://docs.fluentvalidation.net/)
- [AutoMapper](https://automapper.org/)
- [Serilog](https://serilog.net/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

---

## 🎬 Empezar Ahora

```bash
# 1. Continúa desde aquí
cd c:\Users\israe\OneDrive\Documentos\ERP_Escolar\ERPEscolar.API

# 2. Crea la rama feature para Fase 2
git checkout -b feature/fase-2-control-escolar

# 3. Crea primero los DTOs
# (ver template arriba)

# 4. Crea el servicio
# (ver template arriba)

# 5. Crea el controller
# (ver template arriba)

# 6. Test!
dotnet run
# Ir a http://localhost:5235/swagger
```

---

**¡Éxito en la Fase 2!** 🚀

Recuerda: Quality over speed. Mejor tener 5 endpoints bien hechos que 20 mal hechos.
