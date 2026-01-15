# ERP Escolar - Interfaz Web

Interfaz web para el sistema ERP Escolar construida con Next.js, TypeScript y Tailwind CSS.

## Características

- Dashboard con métricas principales
- Gestión de CFDIs (Comprobantes Fiscales Digitales)
- Gestión de pagos y cargos
- Interfaz responsiva y moderna
- Integración con API REST (.NET 8)

## Requisitos Previos

- Node.js (versión 18 o superior)
- .NET 8 SDK
- PostgreSQL (para la base de datos)

## Instalación y Ejecución

### 1. API Backend (.NET)

```bash
# Navegar al directorio de la API
cd ERPEscolar.API

# Instalar dependencias
dotnet restore

# Ejecutar migraciones (si es necesario)
dotnet ef database update

# Ejecutar la API
dotnet run
```

La API estará disponible en: http://localhost:5235
Swagger UI: http://localhost:5235/swagger

### 2. Interfaz Web (Next.js)

```bash
# Navegar al directorio web
cd erp-web

# Instalar dependencias
npm install

# Ejecutar el servidor de desarrollo
npm run dev
```

La interfaz web estará disponible en: http://localhost:3000

## Estructura del Proyecto

```
src/
├── app/                 # Páginas y layouts de Next.js
├── components/          # Componentes reutilizables
│   └── ui/             # Componentes de UI básicos
└── lib/                # Utilidades y configuración API
    ├── api/            # Cliente API y tipos
    └── utils.ts        # Utilidades generales
```

## Tecnologías Utilizadas

- **Frontend:**
  - Next.js 14 (App Router)
  - React 18
  - TypeScript
  - Tailwind CSS
  - Axios (llamadas API)
  - Lucide React (iconos)

- **Backend:**
  - .NET 8
  - ASP.NET Core Web API
  - Entity Framework Core
  - PostgreSQL
  - JWT Authentication
  - FluentValidation

## Funcionalidades Implementadas

### Dashboard
- Métricas principales (alumnos, ingresos, CFDIs, pagos pendientes)
- Operaciones recientes
- Accesos rápidos a funciones principales

### Gestión Financiera
- Visualización de cargos y pagos
- Estados de pago (pendiente, pagado, vencido)

### Gestión Fiscal
- CFDIs emitidos y su estado
- Integración con timbrado electrónico
- Complemento educativo (IEDU)

## Próximos Pasos

- Implementar autenticación en la interfaz web
- Crear formularios para gestión de alumnos
- Desarrollar módulo completo de CFDIs
- Agregar reportes y estadísticas avanzadas
- Implementar notificaciones en tiempo real