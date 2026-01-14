# AlumnoService Refactoring Documentation

## Overview

This document describes the refactoring of `AlumnoService` to follow production-ready patterns, including separation of concerns, thread-safety improvements, and proper soft-delete handling.

## Changes Made

### 1. Validation Delegation to FluentValidation ✅

**Before:**
- `AlumnoService` contained manual validation methods:
  - `ValidateCreateRequest()` - 30 lines
  - `ValidateUpdateRequest()` - 25 lines
  - `IsValidEmail()` - 12 lines

**After:**
- All validation logic delegated to dedicated validators:
  - `CreateAlumnoValidator` (9 rules) - injected via DI
  - `UpdateAlumnoValidator` (5 rules) - injected via DI
- Constructor now accepts `IValidator<CreateAlumnoDto>` and `IValidator<UpdateAlumnoDto>`
- Service methods use `await validator.ValidateAsync(request)` pattern
- Properly maps validation errors to HTTP responses

**Benefits:**
- ✅ Single Responsibility Principle - service handles business logic, validators handle rules
- ✅ Testability - validators can be tested independently
- ✅ Reusability - validators can be used in API middleware via `FluentValidationAutoValidation`
- ✅ Maintainability - validation rules in one place, not duplicated

---

### 2. Soft-Delete Consideration in Uniqueness Checks ✅

**Problem:**
If an alumno is soft-deleted (Activo=false), the CURP/Email should not block another alumno from using the same values.

**Example Scenario:**
```
1. Alumno1 (CURP: ABC123456789ABCDE, Activo: true)
   - Soft deleted → Activo: false
2. Try to create Alumno2 (CURP: ABC123456789ABCDE)
   - OLD: Would fail because CURP exists (even though inactive)
   - NEW: Succeeds because CURP check filters by Activo=true
```

**Implementation:**

**CreateAlumnoAsync:**
```csharp
// CURP validation - only check active alumnos
var curpExists = await _context.Alumnos
    .AnyAsync(a => a.CURP == request.CURP.Trim().ToUpper() 
                && a.Activo == true 
                && a.SchoolId == request.SchoolId);

// Email validation - only check active alumnos
var emailNormalized = request.Email.Trim().ToLower();
var emailExists = await _context.Alumnos
    .AnyAsync(a => a.Email == emailNormalized 
                && a.Activo == true 
                && a.SchoolId == request.SchoolId);
```

**UpdateAlumnoAsync:**
```csharp
// Email validation - only check active alumnos (excluding current)
var emailNormalized = request.Email.Trim().ToLower();
var emailExists = await _context.Alumnos
    .AnyAsync(a => a.Email == emailNormalized 
                && a.Activo == true 
                && a.SchoolId == alumno.SchoolId
                && a.Id != request.Id);
```

**Benefits:**
- ✅ Allows "resurrection" of soft-deleted records with same CURP/Email
- ✅ Respects soft-delete pattern semantics
- ✅ Maintains data integrity

---

### 3. Fixed GenerateMatricula Race Condition 🔄 IMPROVED

**Problem:**
Original implementation used `Count() + 1` which is NOT atomic:

```csharp
var nextNumber = _context.Alumnos
    .Where(a => a.SchoolId == schoolId && a.Matricula != null)
    .Count() + 1;  // ❌ RACE CONDITION
// Two concurrent requests:
// Thread 1: Count=100 → nextNumber=101
// Thread 2: Count=100 → nextNumber=101  (DUPLICATE!)
```

**Current Fix:**
```csharp
var lastMatricula = await _context.Alumnos
    .Where(a => a.SchoolId == schoolId && a.Matricula != null && a.Matricula.StartsWith(yearPrefix))
    .OrderByDescending(a => a.Matricula)
    .Select(a => a.Matricula)
    .FirstOrDefaultAsync();

int nextNumber = 1;
if (!string.IsNullOrEmpty(lastMatricula))
{
    var lastNumber = int.Parse(lastMatricula.Substring(yearPrefix.Length));
    nextNumber = lastNumber + 1;
}
```

**Improvements:**
- ✅ Filters by year prefix (ALU-2026-) to reset count each year
- ✅ Uses `OrderByDescending` to get max, more efficient than Count()
- ✅ Reduces race condition window (though not completely atomic)

**Production Solutions (Recommended for High Concurrency):**

#### Option 1: PostgreSQL SEQUENCE (RECOMMENDED) ⭐
```sql
-- Create sequence per school
CREATE SEQUENCE alumnos_matricula_seq START WITH 1 INCREMENT BY 1;

-- Use in EF Core mapping
modelBuilder.Entity<Alumno>()
    .Property(a => a.MatriculaSequenceNumber)
    .HasDefaultValueSql("nextval('alumnos_matricula_seq')");

-- In code:
var sequenceNumber = await _context.ExecuteRawAsync(
    $"SELECT nextval('alumnos_matricula_seq_school_{schoolId}')"
);
```
**Why:** Native database atomicity, no race conditions, simple to implement.

#### Option 2: Dedicated Sequencing Table with Pessimistic Locking
```sql
CREATE TABLE MatriculaSequence (
    SchoolId INT PRIMARY KEY,
    LastNumber INT NOT NULL DEFAULT 0
);

-- In transaction:
using var transaction = await _context.Database.BeginTransactionAsync();
var sequence = await _context.MatriculaSequences
    .FromSqlInterpolated($"SELECT * FROM \"MatriculaSequence\" WHERE \"SchoolId\" = {schoolId} FOR UPDATE")
    .FirstOrDefaultAsync();
sequence.LastNumber++;
await _context.SaveChangesAsync();
await transaction.CommitAsync();
```
**Why:** Works without SEQUENCE support, explicit control.

#### Option 3: Event Sourcing
```csharp
// Generate matrícula immediately, but idempotent
var matricula = GenerateTemporaryMatricula();  // e.g., UUID-based
// Store as AlumnoCreatedEvent { SchoolId, TempMatricula }
// Background job processes events and assigns final matrícula
```
**Why:** Fully idempotent, handles failures gracefully.

#### Option 4: UUID Generation + Lookup
```csharp
var matricula = Guid.NewGuid().ToString("N").Substring(0, 12);
// Later: Map to sequential number or keep UUID for API, sequential for internal
```
**Why:** No conflicts ever, simple implementation.

---

### 4. Multi-Tenancy: SchoolId Filtering ✅

**Pattern:** All queries that check uniqueness or access data now filter by `SchoolId`.

**Why:** Multiple schools may have alumnos with the same CURP/Email.

**Locations Updated:**
- ✅ `CreateAlumnoAsync` - CURP and Email uniqueness checks
- ✅ `UpdateAlumnoAsync` - Email uniqueness check and alumno retrieval
- ✅ `GenerateMatriculaAsync` - Gets last matrícula for this school only

**Example:**
```csharp
// ✅ Correct - each school has independent sequence
var emailExists = await _context.Alumnos
    .AnyAsync(a => a.Email == emailNormalized 
                && a.SchoolId == request.SchoolId);  // ← Filters by school

// ❌ Wrong - would block same email across different schools
var emailExists = await _context.Alumnos
    .AnyAsync(a => a.Email == emailNormalized);
```

---

### 5. Soft-Delete Filtering in All GET Methods ✅

**Applied to:**
- ✅ `GetByIdAsync()` - Returns null if alumno is inactive
- ✅ `GetByIdFullAsync()` - Returns null if alumno is inactive
- ✅ `GetAllAsync()` - Filters out inactive alumnos from pagination

**Code:**
```csharp
public async Task<Alumno?> GetByIdAsync(int id, int schoolId)
{
    return await _context.Alumnos
        .FirstOrDefaultAsync(a => a.Id == id && a.Activo && a.SchoolId == schoolId);
}

public async Task<PagedResult<Alumno>> GetAllAsync(int schoolId, int pageNumber = 1, int pageSize = 10)
{
    var query = _context.Alumnos.AsQueryable();
    query = query.Where(a => a.Activo && a.SchoolId == schoolId);  // ← Added
    // ... rest of pagination logic
}
```

**Benefits:**
- ✅ Consistent behavior - deleted alumnos never appear in results
- ✅ API contracts respected - clients won't see inactive data

---

## Database Index Recommendations

To optimize the refactored queries, apply these indexes to the `Alumno` table:

### Index 1: CURP Uniqueness per School (Active Only)

```csharp
modelBuilder.Entity<Alumno>()
    .HasIndex(a => new { a.SchoolId, a.CURP })
    .IsUnique()
    .HasDatabaseName("IX_Alumno_SchoolId_CURP")
    .HasFilter("\"Activo\" = true");
```

**SQL:**
```sql
CREATE UNIQUE INDEX "IX_Alumno_SchoolId_CURP" 
    ON "Alumno"("SchoolId", "CURP") 
    WHERE "Activo" = true;
```

**Used by:**
- `CreateAlumnoAsync` - checks CURP uniqueness
- `UpdateAlumnoAsync` - checks CURP existence (if added)

**Performance:** Reduces full table scan to index lookup.

---

### Index 2: Email Uniqueness per School (Active Only)

```csharp
modelBuilder.Entity<Alumno>()
    .HasIndex(a => new { a.SchoolId, a.Email })
    .IsUnique()
    .HasDatabaseName("IX_Alumno_SchoolId_Email")
    .HasFilter("\"Activo\" = true");
```

**SQL:**
```sql
CREATE UNIQUE INDEX "IX_Alumno_SchoolId_Email" 
    ON "Alumno"("SchoolId", "Email") 
    WHERE "Activo" = true;
```

**Used by:**
- `CreateAlumnoAsync` - checks email uniqueness
- `UpdateAlumnoAsync` - checks email uniqueness

**Performance:** Reduces full table scan to index lookup. Filtered index keeps size small.

---

### Index 3: SchoolId + Activo for List/Pagination

```csharp
modelBuilder.Entity<Alumno>()
    .HasIndex(a => new { a.SchoolId, a.Activo, a.Id })
    .HasDatabaseName("IX_Alumno_SchoolId_Activo_Id");
```

**SQL:**
```sql
CREATE INDEX "IX_Alumno_SchoolId_Activo_Id" 
    ON "Alumno"("SchoolId", "Activo", "Id");
```

**Used by:**
- `GetAllAsync` - filters by `SchoolId && Activo` then paginates by Id
- `ExistsAsync` - checks existence by SchoolId

**Performance:** Covering index - query engine doesn't need to look at main table.

---

### Index 4: Matrícula Lookup for Sequential Generation

```csharp
modelBuilder.Entity<Alumno>()
    .HasIndex(a => new { a.SchoolId, a.Matricula })
    .HasDatabaseName("IX_Alumno_SchoolId_Matricula");
```

**SQL:**
```sql
CREATE INDEX "IX_Alumno_SchoolId_Matricula" 
    ON "Alumno"("SchoolId", "Matricula");
```

**Used by:**
- `GenerateMatriculaAsync` - finds max matrícula for school

**Performance:** Speeds up `OrderByDescending(a => a.Matricula)` query.

---

## Dependency Injection Configuration

Update `Program.cs` to explicitly register validators:

```csharp
// Registrar validadores explícitamente para inyección en servicios
builder.Services.AddScoped<IValidator<CreateAlumnoDto>, CreateAlumnoValidator>();
builder.Services.AddScoped<IValidator<UpdateAlumnoDto>, UpdateAlumnoValidator>();
```

**Why:**
- Auto-discovery via `AddValidatorsFromAssemblyContaining<>()` works, but explicit registration ensures clarity
- Allows mock injection in unit tests
- Makes DI container explicit about service dependencies

---

## Testing Recommendations

### Unit Tests for Validation Injection

```csharp
[Fact]
public async Task CreateAlumnoAsync_ValidatorCalled()
{
    var mockValidator = new Mock<IValidator<CreateAlumnoDto>>();
    mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateAlumnoDto>(), default))
        .ReturnsAsync(new FluentValidation.Results.ValidationResult());

    var service = new AlumnoService(mockDbContext, mockLogger, mockValidator.Object, mockUpdateValidator);
    var result = await service.CreateAlumnoAsync(validRequest);

    mockValidator.Verify(v => v.ValidateAsync(validRequest, default), Times.Once);
}
```

### Integration Tests for Soft-Delete

```csharp
[Fact]
public async Task CreateAlumnoAsync_AllowsDuplicateCurpIfPreviousSoftDeleted()
{
    // Arrange: Create and soft-delete alumno1
    var alumno1 = await service.CreateAlumnoAsync(request);
    await service.SoftDeleteAsync(alumno1.Id, schoolId);

    // Act: Try to create alumno2 with same CURP
    var alumno2 = await service.CreateAlumnoAsync(request with { Nombre = "Otro" });

    // Assert: Should succeed
    Assert.NotNull(alumno2);
    Assert.Equal(request.CURP, alumno2.CURP);
}
```

### Concurrency Tests for GenerateMatricula

```csharp
[Fact]
public async Task GenerateMatriculaAsync_HandlesHighConcurrency()
{
    var tasks = Enumerable.Range(1, 100)
        .Select(_ => service.GenerateMatriculaAsync(schoolId))
        .ToList();

    var matriculas = (await Task.WhenAll(tasks)).ToList();

    // Assert: All unique (or acceptable duplicate rate for current implementation)
    var uniqueCount = matriculas.Distinct().Count();
    Assert.Equal(100, uniqueCount);  // Will fail with current implementation at high load
    // Shows why SEQUENCE is needed for production
}
```

---

## Summary of Improvements

| Aspect | Before | After | Impact |
|--------|--------|-------|--------|
| **Validation** | Manual in service | FluentValidation via DI | Separation of concerns, testable |
| **Soft-Delete Handling** | Ignored in uniqueness | Filtered in all checks | Correct semantics |
| **Multi-Tenancy** | Partial SchoolId filtering | Complete filtering | Data isolation |
| **Race Condition** | Count() + 1 | OrderByDescending + documented solutions | Reduced risk |
| **Soft-Deleted Records** | Returned in results | Filtered from results | Consistent behavior |
| **Code Duplication** | Validation rules duplicated | Single source of truth | Maintainability |

---

## Migration Steps (If Applied to Existing Database)

1. **Add indexes** (non-blocking in PostgreSQL):
   ```csharp
   // In EF Core migration:
   migrationBuilder.CreateIndex(
       name: "IX_Alumno_SchoolId_CURP",
       table: "Alumno",
       columns: new[] { "SchoolId", "CURP" },
       unique: true,
       filter: "\"Activo\" = true");
   ```

2. **Verify data integrity:**
   ```sql
   -- Check for duplicate active CURP per school (should be 0)
   SELECT "SchoolId", "CURP", COUNT(*) 
   FROM "Alumno" 
   WHERE "Activo" = true 
   GROUP BY "SchoolId", "CURP" 
   HAVING COUNT(*) > 1;
   ```

3. **Update services** (backward compatible change):
   - Old code: Validation in service + manual validation
   - New code: FluentValidation via DI
   - Both work in parallel during transition

4. **Deploy with no downtime:**
   - Service works with both implementations
   - Gradually migrate validators
   - Monitor for any validation differences

---

## References

- [FluentValidation Documentation](https://docs.fluentvalidation.net)
- [EF Core Indexes](https://learn.microsoft.com/en-us/ef/core/modeling/indexes)
- [PostgreSQL SEQUENCES](https://www.postgresql.org/docs/current/sql-createsequence.html)
- [Soft Delete Pattern](https://en.wikipedia.org/wiki/Soft_delete)
- [Multi-Tenancy in .NET](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/multi-tenancy)
