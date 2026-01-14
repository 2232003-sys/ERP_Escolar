# 🧪 Testing Inicial - ERP Escolar API

## Iniciar la API

```bash
cd c:\Users\israe\OneDrive\Documentos\ERP_Escolar\ERPEscolar.API
dotnet run
```

API estará disponible en: `https://localhost:5001`

---

## 📝 Testing Manual con CURL o Postman

### 1. Crear Roles (Data Seeding - Opcional)

Primero, debemos insertar roles base. Puedes hacerlo con un migration seeder o directamente:

```sql
-- Ejecutar en PostgreSQL
INSERT INTO "Roles" ("Nombre", "Descripcion", "Activo", "FechaCreacion") VALUES
  ('SuperAdmin', 'Administrador supremo', true, NOW()),
  ('Admin TI', 'Administrador de TI', true, NOW()),
  ('Dirección', 'Dirección escolar', true, NOW()),
  ('Control Escolar', 'Control escolar', true, NOW()),
  ('Docente', 'Docente', true, NOW()),
  ('Finanzas', 'Finanzas/Contabilidad', true, NOW()),
  ('Tutor', 'Padre/Tutor', true, NOW());

-- Ver roles creados
SELECT * FROM "Roles";
```

### 2. Crear Usuario de Prueba

```sql
-- Hash de "123456" con BCrypt
INSERT INTO "Users" ("Username", "Email", "PasswordHash", "EmailConfirmado", "Activo", "CambioPasswordRequerido", "FechaCreacion") VALUES
  ('admin', 'admin@erp.local', '$2a$11$xyz...', false, true, false, NOW());

-- Asignar rol SuperAdmin (RoleId = 1)
INSERT INTO "UserRoles" ("UserId", "RoleId", "FechaAsignacion") VALUES
  (1, 1, NOW());
```

**Nota**: Reemplaza el hash con un real generado por BCrypt online o código:
```csharp
BCrypt.Net.BCrypt.HashPassword("123456")
```

### 3. Testing con CURL

#### 🔐 **Login**
```bash
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "123456"
  }' \
  --insecure
```

**Respuesta esperada**:
```json
{
  "userId": 1,
  "username": "admin",
  "email": "admin@erp.local",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "a1b2c3d4e5f6...",
  "roles": ["SuperAdmin"],
  "permisos": []
}
```

#### 📋 **Guardar Token**
```bash
$TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

#### 🔍 **Validar Token**
```bash
curl -X POST "https://localhost:5001/api/auth/validate" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '"'$TOKEN'"' \
  --insecure
```

#### ♻️ **Refresh Token**
```bash
curl -X POST "https://localhost:5001/api/auth/refresh" \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "a1b2c3d4e5f6..."
  }' \
  --insecure
```

#### 👤 **Crear Usuario**
```bash
curl -X POST "https://localhost:5001/api/auth/register" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "username": "docente1",
    "email": "docente1@erp.local",
    "password": "Pass123!",
    "roleIds": [5]
  }' \
  --insecure
```

---

## 🔧 Testing con Postman

### 1. Importar colección
Crear una colección llamada "ERP Escolar"

### 2. Variables de entorno
```json
{
  "base_url": "https://localhost:5001",
  "accessToken": "",
  "refreshToken": ""
}
```

### 3. Crear requests

#### POST: Auth/Login
```
URL: {{base_url}}/api/auth/login
Method: POST
Body (JSON):
{
  "username": "admin",
  "password": "123456"
}
```

**Script (Post-request)**:
```javascript
var jsonData = pm.response.json();
pm.environment.set("accessToken", jsonData.accessToken);
pm.environment.set("refreshToken", jsonData.refreshToken);
```

#### GET: Health Check (con token)
```
URL: {{base_url}}/api/auth/validate
Method: POST
Headers:
  Authorization: Bearer {{accessToken}}
Body (JSON):
"{{accessToken}}"
```

---

## 🐳 Docker Setup (Opcional)

### Dockerfile para Backend
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ERPEscolar.API.csproj", "."]
RUN dotnet restore "ERPEscolar.API.csproj"
COPY . .
RUN dotnet build "ERPEscolar.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ERPEscolar.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ERPEscolar.API.dll"]
```

### docker-compose.yml
```yaml
version: '3.8'

services:
  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: ERPEscolarDB
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: israel
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  api:
    build: ./ERPEscolar.API
    ports:
      - "5001:5001"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=ERPEscolarDB;Username=postgres;Password=israel
    depends_on:
      - postgres

volumes:
  postgres_data:
```

**Ejecutar**:
```bash
docker-compose up -d
```

---

## 📊 Swagger/OpenAPI

Accede a la documentación interactiva:
```
https://localhost:5001/swagger
```

Desde ahí puedes:
- Ver todos los endpoints
- Probar directamente
- Ver esquemas de request/response

---

## ⚠️ Troubleshooting

### Error: "SSL connection error"
Solución (solo en desarrollo):
```bash
curl --insecure https://localhost:5001/...
```

O en Postman: Settings > SSL Certificate Verification > OFF

### Error: "Connection refused"
Verificar que la API está corriendo:
```bash
netstat -ano | findstr :5001
```

### Error: "Invalid token"
- Verificar que el token no expiró (1 hora)
- Usar refresh token para obtener nuevo
- Verificar que la clave JWT en `appsettings.json` es correcta

### Error de BD: "Connection timeout"
Verificar PostgreSQL está corriendo:
```bash
psql -h localhost -U postgres -c "SELECT 1"
```

---

## 📈 Monitoreo

### Logs en tiempo real
```bash
dotnet run --loglevel debug
```

### Queries a la BD
Habilitar en `appsettings.Development.json`:
```json
"Logging": {
  "LogLevel": {
    "Microsoft.EntityFrameworkCore": "Debug"
  }
}
```

---

## 🎯 Siguiente: Seed Data Script

Crear un script `SeedData.cs` para:
1. Crear roles base
2. Crear usuario admin
3. Crear escuela demo
4. Crear ciclo escolar actual
5. Crear 5 alumnos de prueba

Esto facilitará testing rápido.

---

## 📞 Comandos útiles

```bash
# Compilar
dotnet build

# Ejecutar
dotnet run

# Ver migraciones aplicadas
dotnet ef migrations list

# Revertir última migración
dotnet ef migrations remove

# Aplicar migraciones
dotnet ef database update

# Limpiar y recrear BD
dotnet ef database drop --force
dotnet ef database update

# Ver errores de compilación
dotnet build --no-restore
```

---

**Estado**: ✅ API lista para testing
**Proxima sesión**: Seed data + Fase 2 (Controllers Control Escolar)
