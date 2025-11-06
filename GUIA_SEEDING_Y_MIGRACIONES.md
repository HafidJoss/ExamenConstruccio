# Guía Completa - Seeding y Migraciones

## 📋 Resumen

Se ha implementado un sistema completo de inicialización de base de datos que incluye:
- **DataSeeder**: Clase para poblar datos iniciales
- **Migraciones automáticas**: Se aplican al iniciar la aplicación
- **Seeding automático**: Se ejecuta después de las migraciones
- **Verificación de datos**: Evita duplicados

## 🏗️ Arquitectura

```
Program.cs (Startup)
    ↓
InitializeDatabaseAsync()
    ↓
    ├─→ context.Database.MigrateAsync()  (Aplicar migraciones)
    ↓
    └─→ DataSeeder.SeedAsync()           (Poblar datos)
         ↓
         ├─→ SeedUsuariosAsync()         (5 usuarios)
         ├─→ SeedCategoriasAsync()       (8 categorías)
         ├─→ SeedTemasAsync()            (6 temas)
         └─→ SeedMensajesAsync()         (13 mensajes)
```

## 📁 Archivos Creados

### **SalesSuite.Infrastructure/Seeding/DataSeeder.cs**

Clase principal de seeding con:
- `SeedAsync()`: Método principal
- `SeedUsuariosAsync()`: Inserta usuarios
- `SeedCategoriasAsync()`: Inserta categorías
- `SeedTemasAsync()`: Inserta temas
- `SeedMensajesAsync()`: Inserta mensajes

### **Modificaciones en Archivos Existentes**

**SalesSuite.Infrastructure/DependencyInjection.cs**
```csharp
services.AddScoped<DataSeeder>();  // ← Agregado
```

**SalesSuite.Web/Program.cs**
```csharp
await InitializeDatabaseAsync(app);  // ← Agregado

async Task InitializeDatabaseAsync(WebApplication application)
{
    // Aplicar migraciones
    await context.Database.MigrateAsync();
    
    // Ejecutar seeding
    await seeder.SeedAsync();
}
```

## 📊 Datos Insertados

### **Usuarios (5)**

| ID | Usuario | Email | Rol | Descripción |
|----|---------|-------|-----|-------------|
| 1 | admin | admin@forumsales.com | Administrador | Admin principal |
| 2 | moderador | moderador@forumsales.com | Moderador | Moderador del foro |
| 3 | juanperez | juan.perez@example.com | Usuario | Desarrollador .NET |
| 4 | mariagomez | maria.gomez@example.com | Usuario | Arquitecta de software |
| 5 | carlosrodriguez | carlos.rodriguez@example.com | Usuario | Estudiante |

**Nota**: Todos los usuarios tienen la misma contraseña hasheada (placeholder). En producción, usa un sistema de hashing real como ASP.NET Core Identity.

### **Categorías (8)**

| Orden | Nombre | Slug | Icono | Descripción |
|-------|--------|------|-------|-------------|
| 1 | Anuncios | anuncios | 📢 | Anuncios oficiales |
| 2 | General | general | 💬 | Discusiones generales |
| 3 | Programación | programacion | 💻 | Desarrollo de software |
| 4 | .NET y C# | dotnet-csharp | 🔷 | Ecosistema .NET |
| 5 | Bases de Datos | bases-datos | 🗄️ | SQL Server, PostgreSQL, etc. |
| 6 | Arquitectura de Software | arquitectura-software | 🏗️ | Patrones y DDD |
| 7 | Ayuda y Soporte | ayuda-soporte | 🆘 | Preguntas y ayuda |
| 8 | Off-Topic | off-topic | 🎲 | Temas casuales |

### **Temas (6)**

| Título | Categoría | Autor | Fijado | Vistas |
|--------|-----------|-------|--------|--------|
| Bienvenidos al Foro de SalesSuite | Anuncios | admin | ✅ | 150 |
| Normas y Reglas del Foro | Anuncios | admin | ✅ | 120 |
| Como implementar Clean Architecture en .NET 8 | Arquitectura | juanperez | ❌ | 85 |
| Mejores practicas con EF Core 8 | .NET y C# | mariagomez | ❌ | 92 |
| Ayuda con migraciones de EF Core | Ayuda | carlosrodriguez | ❌ | 45 |
| Presentate aqui | General | admin | ❌ | 200 |

### **Mensajes (13)**

Distribuidos en los temas con conversaciones realistas sobre:
- Bienvenidas
- Clean Architecture
- Entity Framework Core
- Ayuda con migraciones
- Presentaciones de usuarios

## 🚀 Cómo Usar

### **1. Configurar Cadena de Conexión**

Edita `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ForumDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### **2. Crear Migración InitialCreate**

#### Opción A: Usando Script PowerShell

```powershell
.\crear-migracion.ps1
```

#### Opción B: Comando Manual

```bash
dotnet ef migrations add InitialCreate \
    --project SalesSuite.Infrastructure \
    --startup-project SalesSuite.Web \
    --context ForumDbContext \
    --output-dir Data/Migrations
```

### **3. Ejecutar la Aplicación**

```bash
dotnet run --project SalesSuite.Web
```

La aplicación automáticamente:
1. ✅ Aplicará las migraciones pendientes
2. ✅ Ejecutará el seeding de datos
3. ✅ Iniciará el servidor web

### **4. Verificar Datos**

Conéctate a la base de datos y ejecuta:

```sql
USE ForumDB;

-- Verificar usuarios
SELECT Id, NombreUsuario, Email, Rol FROM Usuarios;

-- Verificar categorías
SELECT Id, Nombre, Slug, Orden FROM Categorias ORDER BY Orden;

-- Verificar temas
SELECT t.Id, t.Titulo, c.Nombre as Categoria, u.NombreUsuario as Autor, t.Fijado
FROM Temas t
INNER JOIN Categorias c ON t.CategoriaId = c.Id
INNER JOIN Usuarios u ON t.UsuarioId = u.Id;

-- Verificar mensajes
SELECT m.Id, t.Titulo as Tema, u.NombreUsuario as Autor, 
       LEFT(m.Contenido, 50) as Contenido
FROM Mensajes m
INNER JOIN Temas t ON m.TemaId = t.Id
INNER JOIN Usuarios u ON m.UsuarioId = u.Id;
```

## 🔄 Flujo de Inicialización

```
1. Aplicación inicia
   ↓
2. Program.cs ejecuta InitializeDatabaseAsync()
   ↓
3. Se crea un scope de servicios
   ↓
4. Se obtiene ForumDbContext
   ↓
5. context.Database.MigrateAsync()
   ├─→ Verifica migraciones pendientes
   ├─→ Aplica migraciones en orden
   └─→ Actualiza tabla __EFMigrationsHistory
   ↓
6. Se obtiene DataSeeder
   ↓
7. seeder.SeedAsync()
   ├─→ Verifica si ya existen datos (Usuarios.Any())
   ├─→ Si existen: Omite seeding
   └─→ Si no existen:
       ├─→ SeedUsuariosAsync()
       ├─→ SeedCategoriasAsync()
       ├─→ SeedTemasAsync()
       └─→ SeedMensajesAsync()
   ↓
8. Logging de resultados
   ↓
9. Aplicación lista para usar
```

## ✨ Características del DataSeeder

### **1. Verificación de Datos Existentes**

```csharp
if (await _context.Usuarios.AnyAsync())
{
    _logger.LogInformation("La base de datos ya contiene datos. Seeding omitido.");
    return;
}
```

Evita duplicados verificando si ya existen usuarios.

### **2. Orden de Dependencias**

```csharp
await SeedUsuariosAsync();      // Primero: entidades independientes
await SeedCategoriasAsync();    // Segundo: entidades independientes
await SeedTemasAsync();         // Tercero: depende de Usuarios y Categorías
await SeedMensajesAsync();      // Cuarto: depende de Temas y Usuarios
```

### **3. Logging Detallado**

```csharp
_logger.LogInformation("Insertados {Count} usuarios de ejemplo.", usuarios.Count);
```

Registra cada paso del proceso.

### **4. Manejo de Errores**

```csharp
try
{
    // Seeding logic
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error durante el proceso de seeding: {Message}", ex.Message);
    throw;
}
```

### **5. Datos Realistas**

- Usuarios con diferentes roles
- Categorías organizadas por orden
- Temas con vistas y actividad
- Mensajes con conversaciones coherentes
- Fechas variadas (30 días atrás hasta ahora)

## 🛠️ Personalización

### **Agregar Más Usuarios**

```csharp
private async Task SeedUsuariosAsync()
{
    var usuarios = new List<Usuario>
    {
        // ... usuarios existentes ...
        new Usuario
        {
            NombreUsuario = "nuevousuario",
            Email = "nuevo@example.com",
            NombreCompleto = "Nuevo Usuario",
            PasswordHash = "hash_aqui",
            Rol = "Usuario",
            Activo = true,
            FechaRegistro = DateTime.UtcNow
        }
    };
    
    await _context.Usuarios.AddRangeAsync(usuarios);
    await _context.SaveChangesAsync();
}
```

### **Agregar Más Categorías**

```csharp
new Categoria
{
    Nombre = "Nueva Categoria",
    Descripcion = "Descripción de la categoría",
    Slug = "nueva-categoria",
    Icono = "🎯",
    Orden = 9,
    Activa = true,
    FechaCreacion = DateTime.UtcNow
}
```

### **Deshabilitar Seeding Automático**

Comenta la línea en `Program.cs`:

```csharp
// await InitializeDatabaseAsync(app);  // ← Comentar esta línea
```

### **Ejecutar Seeding Manualmente**

```csharp
// En un endpoint o comando personalizado
using var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
await seeder.SeedAsync();
```

## 🐛 Solución de Problemas

### **Error: "La base de datos ya contiene datos"**

Esto es normal. El seeder verifica y omite la inserción si ya existen datos.

Para forzar nuevo seeding:
```bash
# Eliminar la base de datos
dotnet ef database drop --force

# Ejecutar la aplicación (recreará todo)
dotnet run --project SalesSuite.Web
```

### **Error: "Foreign key constraint"**

Verifica el orden de inserción. Las entidades deben insertarse en orden de dependencias:
1. Usuarios (independiente)
2. Categorías (independiente)
3. Temas (depende de Usuarios y Categorías)
4. Mensajes (depende de Temas y Usuarios)

### **Error: "Duplicate key"**

Si intentas ejecutar el seeding múltiples veces, asegúrate de que la verificación `Usuarios.AnyAsync()` esté funcionando.

### **Datos No Aparecen**

Verifica:
1. La migración se aplicó correctamente
2. El seeding se ejecutó sin errores (revisa los logs)
3. Estás conectado a la base de datos correcta

## 📊 Estadísticas de Datos

Después del seeding completo:

```
Usuarios:    5
Categorías:  8
Temas:       6
Mensajes:    13
Total:       32 registros
```

## 🔐 Seguridad

### **Contraseñas**

⚠️ **IMPORTANTE**: Los hashes de contraseña en el seeder son placeholders.

Para producción, usa ASP.NET Core Identity:

```csharp
var passwordHasher = new PasswordHasher<Usuario>();
usuario.PasswordHash = passwordHasher.HashPassword(usuario, "ContraseñaReal123!");
```

### **Datos Sensibles**

No incluyas datos sensibles reales en el seeder:
- Usa emails de ejemplo (@example.com)
- Usa datos ficticios
- No uses información personal real

## 📚 Próximos Pasos

1. **Implementar Autenticación**
   - ASP.NET Core Identity
   - JWT Tokens
   - OAuth/OpenID Connect

2. **Agregar Más Datos de Prueba**
   - Más temas y mensajes
   - Datos para testing
   - Escenarios edge cases

3. **Seeding Condicional**
   - Diferentes datos para Development/Production
   - Seeding basado en configuración

4. **Migración de Datos**
   - Scripts para migrar datos existentes
   - Transformación de datos legacy

## ✅ Checklist de Verificación

- [x] DataSeeder creado en Infrastructure/Seeding
- [x] DataSeeder registrado en DI
- [x] Program.cs configurado para migraciones automáticas
- [x] Program.cs configurado para seeding automático
- [x] Verificación de datos existentes implementada
- [x] Logging completo agregado
- [x] Manejo de errores robusto
- [x] Orden de dependencias correcto
- [x] Datos realistas y coherentes
- [x] Scripts PowerShell para migraciones
- [x] Documentación completa

## 🎯 Resultado Final

Al ejecutar la aplicación por primera vez:

1. ✅ Se crea la base de datos ForumDB
2. ✅ Se aplican todas las migraciones
3. ✅ Se insertan 5 usuarios
4. ✅ Se insertan 8 categorías
5. ✅ Se insertan 6 temas
6. ✅ Se insertan 13 mensajes
7. ✅ El foro está listo para usar con datos de ejemplo

¡La base de datos está completamente inicializada y lista para desarrollo!
