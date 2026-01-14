# Próximos Pasos - Guía de Continuación

## Estado Actual ✅
- **AlumnoService**: Completamente implementado
- **Compilation**: Success
- **API Running**: http://localhost:5235
- **Swagger**: Disponible en /swagger/index.html

---

## Fase 2 - Control Escolar (Continuación)

Crear los siguientes servicios **en este orden**, usando el **AlumnoService como template**:

### 1. **GrupoService** (Siguiente)

#### Archivos a crear:
- `DTOs/ControlEscolar/GrupoDto.cs`
- `DTOs/ControlEscolar/CreateGrupoDto.cs`
- `DTOs/ControlEscolar/UpdateGrupoDto.cs`
- `Infrastructure/Services/IGrupoService.cs`
- `Infrastructure/Services/GrupoService.cs`
- `Features/ControlEscolar/GruposController.cs`

#### Validaciones para Grupo:
```csharp
// En GrupoService
- Nombre no vacío
- Grado válido (1-12 para primaria/secundaria/preparatoria)
- Turno válido (Matutino, Vespertino, Nocturno)
- Capacidad máxima > 0
- Descripción opcional
- CicloEscolarId válido
```

#### Endpoints:
```
GET    /api/grupos                    // Listar con paginación
GET    /api/grupos/{id}               // Obtener uno
GET    /api/grupos/{id}/completo      // Con materias e inscripciones
POST   /api/grupos                    // Crear
PUT    /api/grupos/{id}               // Actualizar
DELETE /api/grupos/{id}               // Desactivar
PATCH  /api/grupos/{id}/restore       // Reactivar
```

#### Métodos de Servicio:
```csharp
Task<GrupoDto> CreateGrupoAsync(CreateGrupoDto request)
Task<GrupoDto> GetByIdAsync(int id)
Task<GrupoCompleteDto> GetByIdCompleteAsync(int id)  // Con materias e inscripciones
Task<PaginatedGruposDto> GetAllAsync(int pageNumber, int pageSize, string? search)
Task<GrupoDto> UpdateGrupoAsync(int id, UpdateGrupoDto request)
Task SoftDeleteAsync(int id)
Task RestoreAsync(int id)
```

---

### 2. **InscripcionService**

#### Validaciones:
```csharp
// En InscripcionService
- AlumnoId existe
- GrupoId existe
- Alumno está activo
- Grupo está activo
- No inscribir mismo alumno en mismo grupo
- FechaInscripcion no puede ser futura
```

#### Endpoints:
```
GET    /api/inscripciones                 // Listar con filtros
GET    /api/inscripciones/{id}            // Obtener una
GET    /api/alumnos/{alumnoId}/inscripciones
GET    /api/grupos/{grupoId}/inscripciones
POST   /api/inscripciones                 // Crear
PUT    /api/inscripciones/{id}            // Actualizar
DELETE /api/inscripciones/{id}            // Desactivar
```

---

### 3. **AsistenciaService**

#### Validaciones:
```csharp
// En AsistenciaService
- InscripcionId existe
- Fecha no es futura
- Solo una asistencia por alumno/fecha
- Justificante opcional
- Tipo: Presente, Ausente, Justificado, Retraso
```

#### Endpoints:
```
GET    /api/asistencias                   // Listar
GET    /api/asistencias/{id}              // Obtener
GET    /api/inscripciones/{inscripcionId}/asistencias
GET    /api/asistencias/reporte/grupo/{grupoId}
POST   /api/asistencias                   // Registrar
PUT    /api/asistencias/{id}              // Editar
DELETE /api/asistencias/{id}              // Eliminar (soft)
```

#### Métodos especiales:
```csharp
Task<double> GetAsistenciaPercentageAsync(int inscripcionId)
Task<ReporteAsistenciaDto> GetGrupoReporteAsync(int grupoId, DateTime desde, DateTime hasta)
```

---

### 4. **CalificacionService**

#### Validaciones:
```csharp
// En CalificacionService
- InscripcionId existe
- Materia existe
- Calificación entre 0-100
- Solo una calificación por alumno/materia/período
- Período está activo
- Tipo: Parcial 1, Parcial 2, Parcial 3, Final
```

#### Endpoints:
```
GET    /api/calificaciones                        // Listar
GET    /api/calificaciones/{id}                   // Obtener
GET    /api/inscripciones/{inscripcionId}/calificaciones
GET    /api/calificaciones/promedio/alumno/{alumnoId}
GET    /api/calificaciones/reporte/grupo/{grupoId}/periodo/{periodoId}
POST   /api/calificaciones                        // Registrar
PUT    /api/calificaciones/{id}                   // Editar
DELETE /api/calificaciones/{id}                   // Eliminar (soft)
```

#### Métodos especiales:
```csharp
Task<double> CalculatePromedioAsync(int alumnoId, int materiaId)
Task<BoletaDto> GetBoletaAsync(int alumnoId, int cicloEscolarId)
Task<ReporteCalificacionesDto> GetGrupoReporteAsync(int grupoId, int periodoId)
```

---

## Fase 3 - Finanzas (Después de Control Escolar)

### 1. **CargosService** - Crear cargos/cobros por alumno
### 2. **PagosService** - Registrar pagos de alumnos
### 3. **EstadoCuentaService** - Generar estado de cuenta por alumno

---

## Fase 4 - Fiscal CFDI (Después de Finanzas)

### 1. **CFDIService** - Timbrado de documentos
### 2. **ComplementoEducativoService** - Complemento educativo

---

## Fase 5 - Frontend React (Paralelo a Backend)

### 1. Login UI
### 2. Dashboard (por rol)
### 3. Módulo Control Escolar
   - CRUD Alumnos
   - CRUD Grupos
   - Inscripciones
   - Asistencias
   - Calificaciones
### 4. Módulo Finanzas (después)
### 5. Módulo Fiscal (después)

---

## Checklist para Cada Nuevo Servicio

Cuando crees un nuevo servicio como GrupoService, asegúrate de:

### DTOs
- [ ] DTO para GET (respuesta básica)
- [ ] DTO para GET completo (con relaciones)
- [ ] DTO para POST (crear)
- [ ] DTO para PUT (actualizar)
- [ ] DTO paginado (wrapper)

### Service
- [ ] Interface IXxxService con todos los métodos
- [ ] Implementación XxxService
- [ ] Constructor con DI de IRepository<T> e ILogger<T>
- [ ] CreateAsync con validaciones
- [ ] GetByIdAsync
- [ ] GetByIdCompleteAsync (si tiene relaciones)
- [ ] GetAllAsync con paginación y búsqueda
- [ ] UpdateAsync con validaciones
- [ ] SoftDeleteAsync
- [ ] RestoreAsync (opcional)
- [ ] ExistsAsync (útil)
- [ ] Métodos privados: ValidateDto, MapToDto, MapFromEntity

### Controller
- [ ] [ApiController] y [Route("api/[controller]")]
- [ ] [Authorize] en clase y/o métodos
- [ ] GET endpoint para listar
- [ ] GET {id} para obtener uno
- [ ] GET {id}/completo para datos completos (si aplica)
- [ ] POST para crear
- [ ] PUT {id} para actualizar
- [ ] DELETE {id} para desactivar
- [ ] PATCH {id}/restore para reactivar (si aplica)
- [ ] Try-catch para cada endpoint
- [ ] Logging con ILogger
- [ ] Return types correctos (200, 201, 204, 400, 404, 409)

### Program.cs
- [ ] Registrar en DI:
  ```csharp
  builder.Services.AddScoped<IXxxService, XxxService>();
  builder.Services.AddScoped<IRepository<Xxx>, Repository<Xxx>>();
  ```

### Testing (opcional para MVP, pero recomendado)
- [ ] Unit tests del Service
- [ ] Integration tests del Controller
- [ ] Pruebas en Swagger

---

## Comandos Útiles

### Compilar
```powershell
cd "c:\Users\israe\OneDrive\Documentos\ERP_Escolar\ERPEscolar.API"
dotnet build
```

### Ejecutar API
```powershell
cd "c:\Users\israe\OneDrive\Documentos\ERP_Escolar\ERPEscolar.API"
dotnet run
```

### Crear Nueva Migración (si cambias modelo)
```powershell
dotnet ef migrations add NombreMigracion
dotnet ef database update
```

### Ver logs de compilación detallados
```powershell
dotnet build -v detailed 2>&1 | Select-String "error|warning"
```

---

## Notas Importantes

### 1. Mantener Consistencia
Todos los servicios deben seguir el **mismo patrón** que AlumnoService:
- Misma estructura de DTOs
- Misma interfaz de servicio
- Mismo manejo de excepciones
- Mismo logging
- Misma validación

### 2. Búsqueda y Paginación
Implementar en todos los GetAllAsync:
```csharp
public async Task<PaginatedXxxDto> GetAllAsync(int pageNumber, int pageSize, string? searchTerm)
{
    pageNumber = Math.Max(pageNumber, 1);
    pageSize = Math.Min(pageSize, 100);
    
    var query = (await _repository.GetAllAsync()).AsQueryable();
    
    if (!string.IsNullOrEmpty(searchTerm))
    {
        var normalizedTerm = searchTerm.ToLower();
        query = query.Where(x => 
            x.Nombre.ToLower().Contains(normalizedTerm) ||
            x.Descripcion.ToLower().Contains(normalizedTerm)
        );
    }
    
    var totalRecords = query.Count();
    var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    
    return new PaginatedXxxDto 
    { 
        Data = items.Select(MapToDto).ToList(),
        TotalRecords = totalRecords,
        TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
        CurrentPage = pageNumber,
        PageSize = pageSize
    };
}
```

### 3. Soft Delete
Todos los servicios deben usar soft delete (no eliminar físicamente):
```csharp
public async Task SoftDeleteAsync(int id)
{
    var entity = await _repository.GetByIdAsync(id);
    if (entity == null)
        throw new NotFoundException($"Entidad no encontrada");
    
    entity.Activo = false;
    entity.FechaActualizacion = DateTime.UtcNow;
    
    await _repository.UpdateAsync(entity);
    await _repository.SaveChangesAsync();
}
```

### 4. Autorización
```csharp
// Lectura: Cualquier autenticado
[Authorize]
public async Task<IActionResult> GetAll() { }

// Escritura: Solo roles específicos
[Authorize(Roles = "SuperAdmin,Admin TI,Control Escolar")]
public async Task<IActionResult> Create() { }
```

### 5. Logging
```csharp
_logger.LogInformation("Creando nuevo grupo: {nombre}", request.Nombre);
_logger.LogWarning("CURP duplicado intentado: {curp}", request.CURP);
_logger.LogError(ex, "Error al actualizar grupo {id}", id);
```

---

## Orden de Prioridad

Para MVP (Minimum Viable Product) completar en este orden:
1. ✅ **AlumnoService** - Completado
2. 🚀 **GrupoService** - Próximo
3. 📝 **InscripcionService** - Después
4. ⏰ **AsistenciaService** - Después
5. 📊 **CalificacionService** - Después
6. 💰 **CargosService** - Después (Finanzas)
7. 💳 **PagosService** - Después (Finanzas)

Una vez Control Escolar (1-5) esté completo:
- Iniciar Frontend React
- Paralelo: Iniciar Finanzas (6-7)
- Finalmente: Fiscal CFDI

---

## Línea de Tiempo Estimada

- **Hoy**: AlumnoService ✅
- **Hoy/Mañana**: GrupoService (2-3 horas)
- **Mañana**: InscripcionService (2-3 horas)
- **Mañana**: AsistenciaService (2-3 horas)
- **Pasado mañana**: CalificacionService (3-4 horas)
- **Resto de semana**: Finanzas + Frontend

---

**¿Estás listo para continuar? Dime cuándo comenzar con GrupoService 🚀**

**Última actualización**: [Sesión actual]
