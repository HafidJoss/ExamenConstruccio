# Foro - Sistema de Mensajería con Clean Architecture

## Arquitectura del Proyecto

Este proyecto implementa un foro de mensajería siguiendo los principios de Clean Architecture en .NET 8.

### Estructura de Carpetas

```
foro/
??? Domain/
?   ??? Entities/     # Entidades del dominio
?   ?   ??? Usuario.cs
?   ?   ??? Categoria.cs
?   ?   ??? Tema.cs
?   ?   ??? Mensaje.cs
?   ??? Interfaces/# Contratos del dominio
?       ??? IGenericRepository.cs
?    ??? IUnitOfWork.cs
??? Application/
?   ??? Services/      # Lógica de negocio
?       ??? UsuarioService.cs
?    ??? ForoService.cs
??? Infrastructure/
    ??? Data/       # Acceso a datos
    ?   ??? ForumDbContext.cs
    ??? Repositories/          # Implementaciones de repositorios
        ??? GenericRepository.cs
        ??? UnitOfWork.cs
```

## Entidades del Dominio

### Usuario
Representa un participante del foro con:
- Nombre (3-100 caracteres, requerido)
- Email (formato válido, único, requerido)
- Biografía (opcional, max 500 caracteres)
- Fecha de registro
- Estado activo

### Categoria
Agrupa los temas del foro:
- Nombre (3-150 caracteres, único, requerido)
- Descripción (opcional, max 500 caracteres)
- Orden para visualización
- Estado activa

### Tema
Hilo de conversación:
- Título (5-250 caracteres, requerido)
- Contenido (10-5000 caracteres, requerido)
- Fechas de creación y última actividad
- Estados: cerrado, fijado
- Contador de vistas
- Pertenece a una Categoría y un Usuario

### Mensaje
Respuesta o comentario:
- Contenido (1-5000 caracteres, requerido)
- Fechas de creación y edición
- Estado de editado
- Pertenece a un Tema y un Usuario

## Relaciones

- **Usuario ? Temas**: Un usuario puede crear muchos temas (1:N)
- **Usuario ? Mensajes**: Un usuario puede crear muchos mensajes (1:N)
- **Categoria ? Temas**: Una categoría contiene muchos temas (1:N)
- **Tema ? Mensajes**: Un tema tiene muchos mensajes (1:N)

## Patrón Repository y Unit of Work

### IGenericRepository<T>
Interfaz genérica que define operaciones CRUD asíncronas:
- `GetAllAsync()`: Obtiene todas las entidades
- `GetByIdAsync(id)`: Obtiene una entidad por ID
- `FindAsync(predicate)`: Busca entidades que cumplan un criterio
- `AddAsync(entity)`: Agrega una nueva entidad
- `Update(entity)`: Actualiza una entidad existente
- `Delete(entity)`: Elimina una entidad
- `AnyAsync(predicate)`: Verifica si existe una entidad
- `CountAsync(predicate)`: Cuenta entidades

### IUnitOfWork
Coordina las operaciones de múltiples repositorios:
- **Propiedades**: `Usuarios`, `Categorias`, `Temas`, `Mensajes`
- `CommitAsync()`: Guarda todos los cambios
- `BeginTransactionAsync()`: Inicia una transacción
- `CommitTransactionAsync()`: Confirma una transacción
- `RollbackTransactionAsync()`: Revierte una transacción

### GenericRepository<T>
Implementación genérica usando Entity Framework Core que:
- Trabaja con cualquier entidad del dominio
- Utiliza operaciones asíncronas (async/await)
- Incluye validaciones de nulidad
- Optimiza consultas con LINQ

### UnitOfWork
Implementación que:
- Mantiene una única instancia de DbContext
- Gestiona transacciones de base de datos
- Implementa lazy loading para repositorios
- Garantiza la integridad de datos con transacciones

## Servicios de Aplicación

### UsuarioService
Gestiona las operaciones relacionadas con usuarios:
- Crear, actualizar y eliminar usuarios
- Validación de emails duplicados
- Desactivación lógica de usuarios
- Obtener estadísticas de usuarios

### ForoService
Gestiona las operaciones del foro:
- Crear temas con mensaje inicial (usando transacciones)
- Agregar mensajes a temas
- Editar mensajes
- Cerrar temas
- Incrementar contador de vistas
- Obtener estadísticas de categorías

#### Ejemplo de uso con transacciones:
```csharp
public async Task<Tema> CrearTemaConMensajeInicialAsync(...)
{
    await _unitOfWork.BeginTransactionAsync();
    try
 {
        // Crear tema
        await _unitOfWork.Temas.AddAsync(nuevoTema);
        await _unitOfWork.CommitAsync();
 
        // Crear mensaje inicial
        await _unitOfWork.Mensajes.AddAsync(mensajeInicial);
     await _unitOfWork.CommitAsync();
        
  await _unitOfWork.CommitTransactionAsync();
        return nuevoTema;
    }
    catch
    {
        await _unitOfWork.RollbackTransactionAsync();
 throw;
    }
}
```

## Configuración de Base de Datos

### Cadena de Conexión

La cadena de conexión está configurada en `appsettings.json`:

```json
"ConnectionStrings": {
  "ForumConnection": "Server=(localdb)\\mssqllocaldb;Database=ForumDb;Trusted_Connection=true;MultipleActiveResultSets=true"
}
```

### Inyección de Dependencias

En `Program.cs` se registran todos los servicios:

```csharp
// DbContext
builder.Services.AddDbContext<ForumDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ForumConnection")));

// Repositorios y Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Servicios de aplicación
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ForoService>();
```

### Crear Migraciones

Para crear la base de datos, ejecuta los siguientes comandos en el directorio del proyecto:

```bash
# Agregar la primera migración
dotnet ef migrations add InitialCreate

# Aplicar la migración a la base de datos
dotnet ef database update
```

### Comandos Útiles de Entity Framework

```bash
# Ver las migraciones disponibles
dotnet ef migrations list

# Crear una nueva migración
dotnet ef migrations add NombreDeLaMigracion

# Aplicar migraciones pendientes
dotnet ef database update

# Revertir a una migración específica
dotnet ef database update NombreDeLaMigracion

# Eliminar la última migración (si no se ha aplicado)
dotnet ef migrations remove

# Generar script SQL de las migraciones
dotnet ef migrations script

# Eliminar la base de datos
dotnet ef database drop
```

## Características de la Implementación

### Validaciones con Data Annotations
- `[Required]`: Campos obligatorios
- `[StringLength]`: Longitud mínima y máxima de cadenas
- `[EmailAddress]`: Validación de formato de email

### Configuración Fluent API
- `HasMaxLength`: Define longitudes máximas en la base de datos
- `IsRequired`: Marca campos como NOT NULL
- `HasDefaultValue` / `HasDefaultValueSql`: Valores predeterminados
- Índices únicos para Email y Nombres de categorías
- Índices de rendimiento para búsquedas frecuentes

### Relaciones Configuradas
- `HasMany().WithOne()`: Relaciones uno a muchos
- `HasForeignKey()`: Define claves foráneas explícitas
- `OnDelete()`: Comportamiento en cascada
  - `Restrict`: Previene eliminación si hay dependencias (Usuario ? Tema/Mensaje, Categoria ? Tema)
  - `Cascade`: Elimina en cascada (Tema ? Mensajes)

### Índices de Base de Datos
- Email único en Usuarios
- Nombre único en Categorías
- Índices en fechas para ordenamiento eficiente
- Índices en claves foráneas para joins rápidos

### Arquitectura en Capas
1. **Domain**: Entidades e interfaces (no depende de nada)
2. **Application**: Servicios de negocio (depende de Domain)
3. **Infrastructure**: Implementaciones técnicas (depende de Domain)
4. **Web**: Capa de presentación (depende de Application e Infrastructure)

### Principios SOLID Aplicados
- **Single Responsibility**: Cada clase tiene una única responsabilidad
- **Open/Closed**: Abierto a extensión mediante interfaces genéricas
- **Liskov Substitution**: Las implementaciones son intercambiables
- **Interface Segregation**: Interfaces específicas y cohesivas
- **Dependency Inversion**: Dependencias hacia abstracciones (interfaces)

## Ejemplo de Uso en Controlador

```csharp
public class TemasController : Controller
{
    private readonly ForoService _foroService;
    
    public TemasController(ForoService foroService)
    {
        _foroService = foroService;
    }
    
    [HttpPost]
    public async Task<IActionResult> Crear(CrearTemaViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
         
   try
   {
          var tema = await _foroService.CrearTemaConMensajeInicialAsync(
         model.UsuarioId,
      model.CategoriaId,
     model.Titulo,
     model.Contenido
            );

            return RedirectToAction("Detalle", new { id = tema.Id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
     return View(model);
  }
    }
}
```

## Tecnologías Utilizadas

- **.NET 8**: Framework principal
- **Entity Framework Core 9.0.10**: ORM
- **SQL Server**: Base de datos (compatible con LocalDB)
- **Data Annotations**: Validaciones en el modelo
- **Fluent API**: Configuración avanzada de base de datos
- **Repository Pattern**: Abstracción de acceso a datos
- **Unit of Work Pattern**: Coordinación de transacciones

## Próximos Pasos

1. ? Crear entidades del dominio
2. ? Configurar DbContext con Fluent API
3. ? Implementar Repository y Unit of Work
4. ? Crear servicios de aplicación
5. ? Crear controladores y vistas
6. ? Agregar autenticación y autorización
7. ? Implementar paginación y búsqueda
8. ? Agregar sistema de likes/votos
9. ? Implementar notificaciones
10. ? Agregar pruebas unitarias e integración
