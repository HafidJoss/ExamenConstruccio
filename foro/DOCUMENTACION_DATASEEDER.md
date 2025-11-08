# ?? Guía Completa: DataSeeder y Migración Automática

## ?? Resumen de Implementación

Se ha implementado un sistema completo de **seeding automático** y **migración de base de datos** que incluye:

1. ? **DataSeeder** profesional en `Infrastructure/Seeding/DataSeeder.cs`
2. ? **Migración automática** al iniciar la aplicación
3. ? **Verificación de duplicados** antes de insertar datos
4. ? **Logging detallado** de todo el proceso
5. ? **Manejo robusto de errores**

---

## ?? Archivos Creados/Modificados

### ? Nuevo Archivo

**`foro/Infrastructure/Seeding/DataSeeder.cs`**
- Clase encargada de poblar datos iniciales
- Métodos asíncronos para cada entidad
- Verificación de datos existentes
- Logging detallado

### ? Archivo Modificado

**`foro/Program.cs`**
- Registro de DataSeeder en DI
- Ejecución automática de migraciones
- Llamada automática al seeding
- Manejo de errores en startup

---

## ??? Estructura del DataSeeder

```
DataSeeder
??? SeedAsync()    ? Método principal
?   ??? Verifica si ya hay datos
?   ??? Ejecuta seeding en orden
?
??? SeedUsuariosAsync()          ? 5 usuarios de prueba
?   ??? Administrador
?   ??? Juan Pérez
?   ??? María García
?   ??? Carlos López
?   ??? Ana Martínez
?
??? SeedCategoriasAsync()      ? 8 categorías
?   ??? .NET Framework
?   ??? ASP.NET Core
?   ??? Entity Framework
?   ??? Desarrollo Web
?   ??? Bases de Datos
?   ??? DevOps & Cloud
?   ??? Arquitectura de Software
?   ??? General
?
??? SeedTemasAsync()    ? 20 temas de ejemplo
?   ??? 2 temas fijados
?   ??? 18 temas normales
?
??? SeedMensajesAsync()          ? 4 mensajes de ejemplo
    ??? 2 en tema "Bienvenidos"
    ??? 2 en tema "JWT Authentication"
```

---

## ?? Flujo de Ejecución al Iniciar la App

```
1. Aplicación inicia
   ?
2. Crear scope de servicios
   ?
3. Obtener ForumDbContext
   ?
4. context.Database.MigrateAsync()
   ? Aplica migraciones pendientes
   ?
5. Obtener DataSeeder
   ?
6. seeder.SeedAsync()
   ??? Verificar si hay datos
   ??? Si NO hay datos:
   ?   ??? SeedUsuariosAsync()
   ?   ??? SeedCategoriasAsync()
   ?   ??? SeedTemasAsync()
   ?   ??? SeedMensajesAsync()
   ??? Si SÍ hay datos: Omitir
   ?
7. Aplicación lista para usar
```

---

## ?? Datos que se Insertan

### ?? Usuarios (5)
| Nombre | Email | Rol |
|--------|-------|-----|
| Administrador | admin@foro.com | Admin |
| Juan Pérez | juan.perez@mail.com | Usuario |
| María García | maria.garcia@mail.com | Usuario |
| Carlos López | carlos.lopez@mail.com | Usuario |
| Ana Martínez | ana.martinez@mail.com | Usuario |

### ?? Categorías (8)
1. .NET Framework
2. ASP.NET Core
3. Entity Framework
4. Desarrollo Web
5. Bases de Datos
6. DevOps & Cloud
7. Arquitectura de Software
8. General

### ?? Temas (20)
- **2 temas fijados** (Bienvenida y Normas)
- **18 temas normales** sobre diversas tecnologías

### ?? Mensajes (4)
- 2 mensajes en tema "Bienvenidos"
- 2 mensajes en tema "JWT Authentication"

---

## ??? Comandos para Crear y Aplicar Migración

### Opción 1: Entity Framework Core CLI (Recomendado)

```bash
# 1. Navegar a la carpeta del proyecto
cd foro

# 2. Crear migración inicial
dotnet ef migrations add InitialCreate

# 3. Aplicar migración (opcional, se hace automático al iniciar)
dotnet ef database update

# 4. Ejecutar aplicación (automáticamente migra y hace seeding)
dotnet run
```

### Opción 2: Package Manager Console (Visual Studio)

```powershell
# En Package Manager Console de Visual Studio

# 1. Crear migración
Add-Migration InitialCreate

# 2. Aplicar migración (opcional)
Update-Database

# 3. Ejecutar aplicación (F5)
```

---

## ? Verificación

### 1. Verificar que la Migración se Creó

Busca en la carpeta `foro/Migrations/` un archivo como:
```
20250126XXXXXX_InitialCreate.cs
```

### 2. Verificar Logs al Iniciar la Aplicación

Deberías ver en la consola:
```
info: Program[0]
      Iniciando migración y seeding de base de datos...
info: Program[0]
      Aplicando migraciones pendientes...
info: Program[0]
      Migraciones aplicadas exitosamente.
info: foro.Infrastructure.Seeding.DataSeeder[0]
      Iniciando proceso de seeding de datos...
info: foro.Infrastructure.Seeding.DataSeeder[0]
      Insertando usuarios de prueba...
info: foro.Infrastructure.Seeding.DataSeeder[0]
  Usuarios insertados: 5
info: foro.Infrastructure.Seeding.DataSeeder[0]
      Insertando categorías...
info: foro.Infrastructure.Seeding.DataSeeder[0]
      Categorías insertadas: 8
info: foro.Infrastructure.Seeding.DataSeeder[0]
      Insertando temas de prueba...
info: foro.Infrastructure.Seeding.DataSeeder[0]
      Temas insertados: 20
info: foro.Infrastructure.Seeding.DataSeeder[0]
      Insertando mensajes de prueba...
info: foro.Infrastructure.Seeding.DataSeeder[0]
      Mensajes insertados: 4
info: foro.Infrastructure.Seeding.DataSeeder[0]
      Proceso de seeding completado exitosamente.
info: Program[0]
      Base de datos inicializada correctamente.
```

### 3. Verificar en SQL Server

```sql
-- Conectar a (localdb)\mssqllocaldb
USE ForumDb;

-- Verificar usuarios
SELECT * FROM Usuarios;
-- Esperado: 5 registros

-- Verificar categorías
SELECT * FROM Categorias ORDER BY Orden;
-- Esperado: 8 registros

-- Verificar temas
SELECT * FROM Temas ORDER BY Fijado DESC, FechaCreacion DESC;
-- Esperado: 20 registros (2 fijados al inicio)

-- Verificar mensajes
SELECT * FROM Mensajes;
-- Esperado: 4 registros

-- Verificar relaciones
SELECT 
    t.Titulo,
    c.Nombre AS Categoria,
    u.Nombre AS Autor,
    COUNT(m.Id) AS NumeroMensajes
FROM Temas t
INNER JOIN Categorias c ON t.CategoriaId = c.Id
INNER JOIN Usuarios u ON t.UsuarioId = u.Id
LEFT JOIN Mensajes m ON t.Id = m.TemaId
GROUP BY t.Titulo, c.Nombre, u.Nombre
ORDER BY t.Fijado DESC, t.FechaCreacion DESC;
```

---

## ?? Características Destacadas

### 1. Verificación de Datos Existentes

```csharp
if (await _context.Usuarios.AnyAsync())
{
    _logger.LogInformation("La base de datos ya contiene datos. Seeding omitido.");
    return;
}
```

**Beneficio**: No duplica datos si ejecutas la aplicación múltiples veces.

### 2. Orden de Dependencias

El seeding se ejecuta en orden correcto:
1. **Usuarios** (sin dependencias)
2. **Categorías** (sin dependencias)
3. **Temas** (depende de Usuarios y Categorías)
4. **Mensajes** (depende de Temas y Usuarios)

### 3. Logging Detallado

Cada paso registra su progreso:
```csharp
_logger.LogInformation("Insertando usuarios de prueba...");
// ... inserción ...
_logger.LogInformation("Usuarios insertados: {Count}", usuarios.Count);
```

### 4. Manejo de Errores

Si algo falla durante el seeding:
```csharp
catch (Exception ex)
{
    _logger.LogError(ex, "Error durante el proceso de seeding de datos");
    throw; // Re-lanza para ver el error en desarrollo
}
```

### 5. Migración Automática

No necesitas ejecutar `dotnet ef database update` manualmente:
```csharp
await context.Database.MigrateAsync();
```

---

## ?? Pruebas

### Test 1: Primera Ejecución (Base de Datos Vacía)

```bash
# 1. Eliminar base de datos existente (si existe)
dotnet ef database drop

# 2. Ejecutar aplicación
dotnet run

# Resultado esperado:
# - Base de datos creada
# - Migraciones aplicadas
# - Datos insertados
# - 5 usuarios, 8 categorías, 20 temas, 4 mensajes
```

### Test 2: Segunda Ejecución (Base de Datos con Datos)

```bash
# 1. Ejecutar aplicación nuevamente
dotnet run

# Resultado esperado:
# - Log: "La base de datos ya contiene datos. Seeding omitido."
# - NO se duplican datos
```

### Test 3: Migración Nueva

```bash
# 1. Agregar nueva entidad al modelo

# 2. Crear migración
dotnet ef migrations add AgregarNuevaEntidad

# 3. Ejecutar aplicación
dotnet run

# Resultado esperado:
# - Nueva migración aplicada automáticamente
# - Seeding omitido (ya hay datos)
```

---

## ?? Configuración Avanzada

### Deshabilitar Seeding Automático

Si no quieres que el seeding se ejecute automáticamente, comenta estas líneas en `Program.cs`:

```csharp
// var seeder = services.GetRequiredService<DataSeeder>();
// await seeder.SeedAsync();
```

### Forzar Re-seeding

Si quieres re-ejecutar el seeding:

```bash
# Opción 1: Eliminar y recrear base de datos
dotnet ef database drop
dotnet run

# Opción 2: Eliminar solo los datos
# En SSMS:
DELETE FROM Mensajes;
DELETE FROM Temas;
DELETE FROM Categorias;
DELETE FROM Usuarios;
# Luego:
dotnet run
```

### Seeding Solo en Desarrollo

Modifica `Program.cs` para ejecutar seeding solo en desarrollo:

```csharp
using (var scope = app.Services.CreateScope())
{
var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<ForumDbContext>();
    
    try
    {
        // Siempre aplicar migraciones
        await context.Database.MigrateAsync();
        
        // Seeding solo en desarrollo
        if (app.Environment.IsDevelopment())
  {
            var seeder = services.GetRequiredService<DataSeeder>();
        await seeder.SeedAsync();
        }
    }
    catch (Exception ex)
    {
   logger.LogError(ex, "Error al inicializar la base de datos");
        throw;
    }
}
```

---

## ?? Comparación: Script SQL vs DataSeeder C#

| Aspecto | Script SQL | DataSeeder C# |
|---------|------------|---------------|
| **Ejecución** | Manual (SSMS) | ? Automática |
| **Duplicados** | ?? Posible | ? Prevenido |
| **Logging** | ? No | ? Detallado |
| **Errores** | ?? Difícil debug | ? Stack trace claro |
| **Mantenimiento** | ?? Separado del código | ? Parte del código |
| **Versionado** | ? Git | ? Git |
| **Portabilidad** | ?? SQL Server específico | ? Agnóstico de BD |

---

## ?? Troubleshooting

### Problema 1: Error "Table already exists"

**Causa**: Intentas crear migración con tablas existentes

**Solución**:
```bash
# Opción 1: Eliminar BD y empezar de cero
dotnet ef database drop

# Opción 2: Generar migración desde esquema existente
dotnet ef migrations add InitialCreate --context-only
```

### Problema 2: Seeding se ejecuta cada vez

**Causa**: La verificación `AnyAsync()` no funciona correctamente

**Solución**: Verifica que las tablas no estén vacías:
```sql
SELECT COUNT(*) FROM Usuarios;
```

### Problema 3: Error de foreign key

**Causa**: Orden incorrecto de inserción

**Solución**: El DataSeeder ya está ordenado correctamente. Verifica que no hayas modificado el orden.

### Problema 4: "Cannot access a disposed context"

**Causa**: Context fuera del scope

**Solución**: Ya está resuelto en el código con `using (var scope = ...)`

---

## ?? Próximos Pasos

### 1. Agregar Más Datos

Modifica `DataSeeder.cs` para agregar más:
- Usuarios
- Categorías
- Temas
- Mensajes

### 2. Crear Seeders Específicos

Crea seeders separados:
```csharp
// Infrastructure/Seeding/UsuariosSeeder.cs
// Infrastructure/Seeding/CategoriasSeeder.cs
// Infrastructure/Seeding/TemasSeeder.cs
```

### 3. Agregar Datos de Producción

Crea un seeder diferente para producción:
```csharp
// Infrastructure/Seeding/ProductionDataSeeder.cs
```

### 4. Implementar Roles y Permisos

Agrega seeding para:
- Roles (Admin, Moderador, Usuario)
- Permisos
- Asignación de roles a usuarios

---

## ? Checklist de Implementación

```
DATASEEDER
??? ? Clase DataSeeder creada
??? ? Métodos asíncronos implementados
??? ? Verificación de duplicados
??? ? Logging detallado
??? ? Manejo de errores
??? ? Orden de dependencias correcto

PROGRAM.CS
??? ? DataSeeder registrado en DI
??? ? Migración automática configurada
??? ? Seeding automático configurado
??? ? Manejo de errores en startup
??? ? Ruta por defecto a Temas

DATOS
??? ? 5 usuarios de prueba
??? ? 8 categorías
??? ? 20 temas (2 fijados)
??? ? 4 mensajes

MIGRACIÓN
??? ? Comandos documentados
??? ? Verificación de migración
??? ? Aplicación automática

BUILD
??? ? Sin errores de compilación
??? ? Sin warnings
??? ? Listo para ejecutar
```

---

## ?? Resultado Final

Al ejecutar `dotnet run`, la aplicación:

1. ? **Crea la base de datos** (si no existe)
2. ? **Aplica todas las migraciones** automáticamente
3. ? **Inserta datos de prueba** (si la BD está vacía)
4. ? **Abre el foro** listo para usar con:
   - 5 usuarios
   - 8 categorías
   - 20 temas
   - 4 mensajes

**¡Todo automático, sin intervención manual!** ??

---

## ?? Comandos Rápidos de Referencia

```bash
# Crear migración inicial
dotnet ef migrations add InitialCreate

# Eliminar migración
dotnet ef migrations remove

# Listar migraciones
dotnet ef migrations list

# Aplicar migraciones
dotnet ef database update

# Eliminar base de datos
dotnet ef database drop

# Ver script SQL de migración
dotnet ef migrations script

# Ejecutar aplicación (automáticamente migra y seed)
dotnet run
```

---

**¡DataSeeder implementado con éxito!** ?
