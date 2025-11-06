# Comandos de Migración - Entity Framework Core

## 📋 Requisitos Previos

### Instalar EF Core Tools

```bash
# Instalar globalmente
dotnet tool install --global dotnet-ef

# O actualizar si ya está instalado
dotnet tool update --global dotnet-ef

# Verificar instalación
dotnet ef --version
```

## 🚀 Crear Migración InitialCreate

### Opción 1: Usando PowerShell Script (Recomendado)

```powershell
# Desde la raíz del proyecto
.\crear-migracion.ps1
```

### Opción 2: Comando Manual

```bash
# Desde la raíz del proyecto
dotnet ef migrations add InitialCreate \
    --project SalesSuite.Infrastructure \
    --startup-project SalesSuite.Web \
    --context ForumDbContext \
    --output-dir Data/Migrations
```

### Opción 3: Package Manager Console (Visual Studio)

```powershell
Add-Migration InitialCreate `
    -Project SalesSuite.Infrastructure `
    -StartupProject SalesSuite.Web `
    -Context ForumDbContext `
    -OutputDir Data/Migrations
```

## 📊 Aplicar Migraciones

### Opción 1: Automático al Iniciar la Aplicación

La aplicación está configurada para aplicar migraciones automáticamente en `Program.cs`:

```bash
dotnet run --project SalesSuite.Web
```

### Opción 2: Usando PowerShell Script

```powershell
.\aplicar-migracion.ps1
```

### Opción 3: Comando Manual

```bash
dotnet ef database update \
    --project SalesSuite.Infrastructure \
    --startup-project SalesSuite.Web \
    --context ForumDbContext
```

### Opción 4: Package Manager Console (Visual Studio)

```powershell
Update-Database `
    -Project SalesSuite.Infrastructure `
    -StartupProject SalesSuite.Web `
    -Context ForumDbContext
```

## 🔍 Comandos Útiles

### Ver Migraciones Pendientes

```bash
dotnet ef migrations list \
    --project SalesSuite.Infrastructure \
    --startup-project SalesSuite.Web
```

### Ver Script SQL de la Migración

```bash
dotnet ef migrations script \
    --project SalesSuite.Infrastructure \
    --startup-project SalesSuite.Web \
    --output migration.sql
```

### Revertir a una Migración Anterior

```bash
# Revertir a la migración anterior
dotnet ef database update NombreMigracionAnterior \
    --project SalesSuite.Infrastructure \
    --startup-project SalesSuite.Web
```

### Eliminar la Última Migración

```bash
# Solo si NO ha sido aplicada a la BD
dotnet ef migrations remove \
    --project SalesSuite.Infrastructure \
    --startup-project SalesSuite.Web
```

### Eliminar la Base de Datos

```bash
dotnet ef database drop \
    --project SalesSuite.Infrastructure \
    --startup-project SalesSuite.Web \
    --force
```

## 📝 Estructura de Archivos Generados

Después de crear la migración, se generarán estos archivos:

```
SalesSuite.Infrastructure/
└── Data/
    └── Migrations/
        ├── 20241105XXXXXX_InitialCreate.cs          # Migración Up/Down
        ├── 20241105XXXXXX_InitialCreate.Designer.cs # Metadata
        └── ForumDbContextModelSnapshot.cs           # Snapshot del modelo
```

## ⚙️ Configuración de Cadena de Conexión

Asegúrate de tener configurada la cadena de conexión en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ForumDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

## 🐛 Solución de Problemas

### Error: "No se puede encontrar el proyecto"

```bash
# Asegúrate de estar en la raíz del proyecto donde está SalesSuite.sln
cd c:\Users\PC\Downloads\Examen
```

### Error: "Build failed"

```bash
# Limpia y compila la solución
dotnet clean
dotnet build
```

### Error: "Unable to create an object of type 'ForumDbContext'"

Verifica que:
1. `ForumDbContext` tenga un constructor que acepte `DbContextOptions<ForumDbContext>`
2. La cadena de conexión esté correctamente configurada
3. El proyecto Web tenga referencia al proyecto Infrastructure

### Error: "The migration has already been applied"

```bash
# Si quieres recrear la migración:
# 1. Eliminar la base de datos
dotnet ef database drop --force

# 2. Eliminar la migración
dotnet ef migrations remove

# 3. Crear nuevamente
dotnet ef migrations add InitialCreate
```

## 📚 Flujo Completo Recomendado

```bash
# 1. Limpiar y compilar
dotnet clean
dotnet build

# 2. Crear migración
dotnet ef migrations add InitialCreate \
    --project SalesSuite.Infrastructure \
    --startup-project SalesSuite.Web

# 3. Revisar la migración generada
# Verificar los archivos en SalesSuite.Infrastructure/Data/Migrations/

# 4. Aplicar migración (automático al ejecutar la app)
dotnet run --project SalesSuite.Web

# O manualmente:
dotnet ef database update \
    --project SalesSuite.Infrastructure \
    --startup-project SalesSuite.Web
```

## ✅ Verificación

Después de aplicar las migraciones:

```sql
-- Conectarse a la base de datos y verificar tablas
USE ForumDB;
GO

-- Listar todas las tablas
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- Verificar datos insertados por el seeder
SELECT COUNT(*) as TotalUsuarios FROM Usuarios;
SELECT COUNT(*) as TotalCategorias FROM Categorias;
SELECT COUNT(*) as TotalTemas FROM Temas;
SELECT COUNT(*) as TotalMensajes FROM Mensajes;
```

## 🎯 Resultado Esperado

Después de ejecutar las migraciones y el seeding, deberías tener:

- ✅ 5 Usuarios (1 admin, 1 moderador, 3 usuarios normales)
- ✅ 8 Categorías
- ✅ 6 Temas
- ✅ 13 Mensajes

## 📖 Referencias

- [EF Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [EF Core CLI Tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)
- [Package Manager Console](https://learn.microsoft.com/en-us/ef/core/cli/powershell)
