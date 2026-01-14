# Arquitectura - ERP Escolar API

## Estructura del Proyecto

```
ERPEscolar.API/
│
├── Models/                          # Entidades de base de datos
│   ├── School.cs
│   ├── CicloEscolar.cs
│   ├── PeriodoCalificacion.cs
│   ├── Alumno.cs                    # Entidad principal de Control Escolar
│   ├── Tutor.cs
│   ├── Docente.cs
│   ├── Materia.cs
│   ├── Grupo.cs
│   ├── GrupoMateria.cs
│   ├── Inscripcion.cs
│   ├── Asistencia.cs
│   ├── Calificacion.cs
│   ├── User.cs
│   ├── Role.cs
│   ├── Permiso.cs
│   ├── RolePermiso.cs
│   ├── UserRole.cs
│   ├── RefreshToken.cs
│   ├── ConceptoCobro.cs
│   ├── Cargo.cs
│   ├── Pago.cs
│   ├── Beca.cs
│   ├── ConfiguracionFiscal.cs
│   ├── CFDI.cs
│   ├── ComplementoEducativo.cs
│   ├── BitacoraFiscal.cs
│   └── ConfiguracionCFDI.cs
│
├── Core/
│   ├── Exceptions/
│   │   └── CustomExceptions.cs      # NotFoundException, BusinessException, ValidationException
│   └── Interfaces/
│       └── IRepository.cs           # Interface genérico del patrón Repository
│
├── Data/
│   ├── AppDbContext.cs              # Context de EF Core
│   └── Repository.cs                # Implementación genérica del Repository Pattern
│
├── DTOs/                            # Data Transfer Objects
│   ├── Auth/
│   │   ├── LoginRequestDto.cs
│   │   ├── LoginResponseDto.cs
│   │   ├── CreateUserDto.cs
│   │   ├── UserDto.cs
│   │   └── RefreshTokenRequestDto.cs
│   │
│   └── ControlEscolar/
│       ├── AlumnoDto.cs             # GET response básico
│       ├── CreateAlumnoDto.cs       # POST request
│       ├── UpdateAlumnoDto.cs       # PUT request
│       ├── AlumnoFullDataDto.cs     # GET completo con relaciones
│       ├── GrupoInscripcionDto.cs   # Nested DTO
│       └── PaginatedAlumnosDto.cs   # Wrapper de paginación
│
├── Infrastructure/
│   └── Services/
│       ├── IAuthService.cs          # Interface de autenticación
│       ├── AuthService.cs           # Implementación de autenticación
│       ├── SeedDataService.cs       # Generador de datos de prueba
│       ├── IAlumnoService.cs        # Interface de alumnos ✨ NUEVO
│       └── AlumnoService.cs         # Implementación de alumnos ✨ NUEVO
│
├── Features/
│   └── ControlEscolar/
│       ├── AlumnosController.cs     # ✨ NUEVO - 7 endpoints
│       └── [Futuros Controllers]
│           ├── GruposController.cs
│           ├── InscripcionesController.cs
│           ├── AsistenciasController.cs
│           └── CalificacionesController.cs
│
├── Properties/
│   └── launchSettings.json
│
├── Program.cs                       # Configuración de startup
├── appsettings.json                # Configuración base
├── appsettings.Development.json     # Configuración desarrollo
└── ERPEscolar.API.csproj
```

---

## Capas de la Arquitectura

### 1. **Capa de Presentación (API)**
**Ubicación:** `Features/ControlEscolar/AlumnosController.cs`

**Responsabilidades:**
- Recibir requests HTTP
- Validar datos de entrada (ModelState)
- Mapear DTOs a/desde servicios
- Retornar respuestas HTTP apropiadas
- Manejo de autorizaciones

**Ejemplo:**
```csharp
[HttpPost]
[Authorize(Roles = "SuperAdmin,Admin TI,Control Escolar")]
public async Task<IActionResult> Create([FromBody] CreateAlumnoDto request)
{
    // Validación, call al servicio, respuesta
}
```

---

### 2. **Capa de Aplicación (Services)**
**Ubicación:** `Infrastructure/Services/AlumnoService.cs`

**Responsabilidades:**
- Implementar lógica de negocio
- Validaciones complejas
- Orquestación de operaciones
- Manejo de excepciones
- Logging

**Ejemplo de Servicio:**
```csharp
public class AlumnoService : IAlumnoService
{
    private readonly IRepository<Alumno> _repository;
    private readonly ILogger<AlumnoService> _logger;
    
    public async Task<AlumnoDto> CreateAlumnoAsync(CreateAlumnoDto dto)
    {
        // 1. Validar CURP único
        // 2. Validar Email único
        // 3. Validar edad
        // 4. Auto-generar matrícula
        // 5. Crear entidad
        // 6. Guardar en BD
        // 7. Mapear a DTO
        // 8. Retornar
    }
}
```

---

### 3. **Capa de Acceso a Datos (Repository Pattern)**
**Ubicación:** `Data/Repository.cs`

**Responsabilidades:**
- Abstracción de acceso a BD
- Operaciones CRUD genéricas
- Queries complejas
- DbContext management

**Interfaz Genérica:**
```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task SaveChangesAsync();
}
```

---

### 4. **Capa de Datos (Entity Framework Core)**
**Ubicación:** `Data/AppDbContext.cs`

**Responsabilidades:**
- Mapeo de entidades a tablas
- Definir relaciones
- Configuración de índices
- Seed data

**Ejemplo:**
```csharp
public DbSet<Alumno> Alumnos { get; set; }
public DbSet<Tutor> Tutores { get; set; }

modelBuilder.Entity<Alumno>(entity =>
{
    entity.HasKey(a => a.Id);
    entity.Property(a => a.Nombre).IsRequired().HasMaxLength(100);
    entity.HasIndex(a => a.CURP).IsUnique();
    entity.HasIndex(a => a.Email).IsUnique();
    entity.HasMany(a => a.Inscripciones)
          .WithOne(i => i.Alumno)
          .HasForeignKey(i => i.AlumnoId);
});
```

---

## Patrón de Implementación: AlumnoService

Este patrón debe ser seguido para todos los servicios de Control Escolar.

### 1. Interfaz (IAlumnoService)
```csharp
public interface IAlumnoService
{
    Task<AlumnoDto> CreateAlumnoAsync(CreateAlumnoDto request);
    Task<AlumnoDto> GetByIdAsync(int id);
    Task<AlumnoFullDataDto> GetByIdFullAsync(int id);
    Task<PaginatedAlumnosDto> GetAllAsync(int pageNumber, int pageSize, string? searchTerm);
    Task<AlumnoDto> UpdateAlumnoAsync(int id, UpdateAlumnoDto request);
    Task SoftDeleteAsync(int id);
    Task RestoreAsync(int id);
    Task<bool> ExistsAsync(int id);
}
```

### 2. Implementación (AlumnoService)
```csharp
public class AlumnoService : IAlumnoService
{
    private readonly IRepository<Alumno> _repository;
    private readonly ILogger<AlumnoService> _logger;

    public AlumnoService(
        IRepository<Alumno> repository,
        ILogger<AlumnoService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<AlumnoDto> CreateAlumnoAsync(CreateAlumnoDto request)
    {
        // Validaciones privadas
        ValidateCreateAlumnoDto(request);
        
        // Verificar CURP único
        var curpExists = await _repository
            .GetAllAsync()
            .ContinueWith(t => t.Result.Any(a => a.CURP == request.CURP));
        
        if (curpExists)
            throw new BusinessException("Ya existe un alumno con este CURP");

        // Auto-generar matrícula
        var matricula = GenerateMatricula();

        // Mapear a entidad
        var alumno = new Alumno
        {
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            CURP = request.CURP,
            Email = request.Email,
            Matricula = matricula,
            FechaNacimiento = request.FechaNacimiento,
            Genero = request.Genero,
            Direccion = request.Direccion,
            TelefonoContacto = request.TelefonoContacto,
            TutorId = request.TutorId,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        await _repository.AddAsync(alumno);
        await _repository.SaveChangesAsync();

        return MapToDto(alumno);
    }

    // Métodos privados de validación y mapeo
    private void ValidateCreateAlumnoDto(CreateAlumnoDto dto)
    {
        // Validaciones
    }

    private string GenerateMatricula()
    {
        // Genera formato ALU-{año}-{secuencia}
    }

    private AlumnoDto MapToDto(Alumno alumno)
    {
        return new AlumnoDto { /* ... */ };
    }
}
```

### 3. Controller (AlumnosController)
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlumnosController : ControllerBase
{
    private readonly IAlumnoService _alumnoService;
    private readonly ILogger<AlumnosController> _logger;

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin TI,Control Escolar")]
    public async Task<IActionResult> Create([FromBody] CreateAlumnoDto request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var alumno = await _alumnoService.CreateAlumnoAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = alumno.Id }, alumno);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message, errors = ex.Errors });
        }
        catch (BusinessException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear alumno");
            return StatusCode(500, new { message = "Error al crear alumno." });
        }
    }
}
```

### 4. Registro en DI Container (Program.cs)
```csharp
builder.Services.AddScoped<IAlumnoService, AlumnoService>();
builder.Services.AddScoped<IRepository<Alumno>, Repository<Alumno>>();
```

---

## Flujo de una Request

```
1. HTTP Request (POST /api/alumnos)
   ↓
2. AlumnosController.Create() recibe JSON
   ↓
3. ModelState.IsValid valida estructura
   ↓
4. IAlumnoService.CreateAlumnoAsync(dto) ejecuta lógica
   ↓
5. Validaciones internas:
   - CURP único
   - Email único
   - Edad mínima
   - Campos requeridos
   ↓
6. IRepository<Alumno>.AddAsync(entity) inserta en BD
   ↓
7. SaveChangesAsync() confirma cambios
   ↓
8. MapToDto(alumno) transforma entidad a DTO
   ↓
9. Return CreatedAtAction() (201) con DTO
   ↓
10. Response HTTP con JSON
```

---

## Seguridad

### 1. Autenticación
- JWT Bearer tokens
- Refresh tokens para renovación
- Expiración de 1 hora

### 2. Autorización
- [Authorize] en nivel de Controller
- Roles específicos en endpoints sensibles

```csharp
[Authorize]                                    // Todos autenticados
[Authorize(Roles = "Control Escolar")]        // Solo Control Escolar
[Authorize(Roles = "SuperAdmin,Admin TI")]    // SuperAdmin O Admin TI
```

### 3. Validación de Entrada
- ModelState en Controller
- Validaciones complejas en Service
- Custom exceptions para errores de negocio

### 4. Hashing de Datos Sensibles
- Contraseñas con BCrypt
- Tokens con HS256

---

## Patrones Utilizados

### 1. Repository Pattern
Abstrae acceso a datos con IRepository<T> genérico.

### 2. Dependency Injection
Inyección de dependencias en constructores.

### 3. Async/Await
Operaciones no bloqueantes con Task.

### 4. DTOs (Data Transfer Objects)
Separación entre modelos de BD y API.

### 5. Custom Exceptions
Jerarquía de excepciones para diferentes errores:
- `NotFoundException`
- `BusinessException`
- `ValidationException`

### 6. SOLID Principles
- **S**: Responsabilidad única (cada clase una responsabilidad)
- **O**: Abierto/Cerrado (abierto a extensión, cerrado a modificación)
- **L**: Liskov Substitution (interfaces genéricas)
- **I**: Segregación de interfaces (IAlumnoService pequeño y específico)
- **D**: Inyección de dependencias

---

## Próximas Entidades (Mismo Patrón)

### GrupoService
- Validar capacidad máxima
- Relaciones con Materias
- Turno y Grado

### InscripcionService
- Validar alumno activo
- Validar grupo activo
- Prevenir inscripciones duplicadas

### AsistenciaService
- Validar fechas válidas
- Cálculo de porcentaje
- Reportes

### CalificacionService
- Validar rango 0-100
- Cálculo de promedio
- Control por período

---

**Última actualización**: [Sesión actual]
**Estado**: AlumnoService completamente documentado ✅
