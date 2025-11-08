# Guía de Uso - Patrón Repository y Unit of Work

## Introducción

Esta guía explica cómo usar el patrón Repository y Unit of Work implementado en el proyecto del foro.

## Arquitectura de Capas

```
???????????????????????????????????????????????????????????
?              Presentation Layer    ?
?         (Controllers, Views, Models)        ?
?  - UsuariosController.cs            ?
?  - TemasController.cs    ?
???????????????????????????????????????????????????????????
     ? Depende de ?
???????????????????????????????????????????????????????????
?      Application Layer          ?
?             (Business Logic Services)         ?
?  - UsuarioService.cs          ?
?  - ForoService.cs      ?
???????????????????????????????????????????????????????????
    ? Depende de ?
???????????????????????????????????????????????????????????
?Domain Layer       ?
?              (Entities & Interfaces)          ?
?  Entities:             Interfaces:   ?
?  - Usuario.cs       - IGenericRepository<T>    ?
?  - Categoria.cs         - IUnitOfWork          ?
?  - Tema.cs       ?
?  - Mensaje.cs      ?
???????????????????????????????????????????????????????????
         ? Implementado por ?
???????????????????????????????????????????????????????????
?                Infrastructure Layer    ?
?   (Data Access & Implementations)                 ?
?  - ForumDbContext.cs ?
?  - GenericRepository<T>.cs        ?
?  - UnitOfWork.cs ?
????????????????????????????????????????????????????????????
```

## Flujo de Datos

### Ejemplo: Crear un Usuario

```
1. Usuario hace request ? UsuariosController.Crear()
     ?
2. Controller llama ? UsuarioService.CrearUsuarioAsync()
           ?
3. Service valida ? _unitOfWork.Usuarios.AnyAsync() (verifica email)
?
4. Service ejecuta ? _unitOfWork.Usuarios.AddAsync(usuario)
      ?
5. Service guarda ? _unitOfWork.CommitAsync()
  ?
6. UnitOfWork ejecuta ? _context.SaveChangesAsync()
  ?
7. EF Core persiste ? SQL Server Database
           ?
8. Response ? Controller retorna resultado al usuario
```

## Uso del IGenericRepository<T>

### Operaciones Básicas

#### 1. Obtener Todos los Registros
```csharp
var usuarios = await _unitOfWork.Usuarios.GetAllAsync();
```

#### 2. Obtener por ID
```csharp
var usuario = await _unitOfWork.Usuarios.GetByIdAsync(5);
if (usuario == null)
{
  // Manejar caso no encontrado
}
```

#### 3. Buscar con Criterio
```csharp
// Usuarios activos
var activos = await _unitOfWork.Usuarios.FindAsync(u => u.Activo);

// Usuarios por email
var usuario = await _unitOfWork.Usuarios.FindAsync(u => u.Email == "ejemplo@mail.com");

// Temas de una categoría
var temas = await _unitOfWork.Temas.FindAsync(t => t.CategoriaId == 1);
```

#### 4. Verificar Existencia
```csharp
bool existe = await _unitOfWork.Usuarios.AnyAsync(u => u.Email == "ejemplo@mail.com");
if (existe)
{
    throw new InvalidOperationException("El email ya está registrado");
}
```

#### 5. Contar Registros
```csharp
// Contar todos
int total = await _unitOfWork.Usuarios.CountAsync();

// Contar con criterio
int activos = await _unitOfWork.Usuarios.CountAsync(u => u.Activo);
```

#### 6. Agregar
```csharp
var nuevoUsuario = new Usuario
{
    Nombre = "Juan Pérez",
    Email = "juan@mail.com",
    FechaRegistro = DateTime.UtcNow
};

await _unitOfWork.Usuarios.AddAsync(nuevoUsuario);
await _unitOfWork.CommitAsync(); // Guardar cambios
```

#### 7. Actualizar
```csharp
var usuario = await _unitOfWork.Usuarios.GetByIdAsync(5);
if (usuario != null)
{
    usuario.Nombre = "Juan Carlos Pérez";
    usuario.Biografia = "Desarrollador .NET";
    
_unitOfWork.Usuarios.Update(usuario);
    await _unitOfWork.CommitAsync();
}
```

#### 8. Eliminar
```csharp
var usuario = await _unitOfWork.Usuarios.GetByIdAsync(5);
if (usuario != null)
{
    _unitOfWork.Usuarios.Delete(usuario);
    await _unitOfWork.CommitAsync();
}
```

## Uso del IUnitOfWork

### Operaciones Simples

```csharp
// Agregar un usuario
await _unitOfWork.Usuarios.AddAsync(nuevoUsuario);
await _unitOfWork.CommitAsync();

// Actualizar múltiples entidades
_unitOfWork.Usuarios.Update(usuario);
_unitOfWork.Temas.Update(tema);
await _unitOfWork.CommitAsync(); // Guarda ambos cambios
```

### Operaciones con Transacciones

#### Escenario 1: Crear Tema con Mensaje Inicial

```csharp
public async Task<Tema> CrearTemaCompleto(int usuarioId, int categoriaId, string titulo, string contenido)
{
    try
    {
        // Iniciar transacción
        await _unitOfWork.BeginTransactionAsync();

        // Paso 1: Crear tema
        var tema = new Tema
        {
   Titulo = titulo,
      Contenido = contenido,
   UsuarioId = usuarioId,
            CategoriaId = categoriaId,
    FechaCreacion = DateTime.UtcNow
        };
        
        await _unitOfWork.Temas.AddAsync(tema);
     await _unitOfWork.CommitAsync();

    // Paso 2: Crear mensaje inicial
        var mensaje = new Mensaje
        {
    Contenido = contenido,
   TemaId = tema.Id,
      UsuarioId = usuarioId,
       FechaCreacion = DateTime.UtcNow
        };
        
     await _unitOfWork.Mensajes.AddAsync(mensaje);
        await _unitOfWork.CommitAsync();

        // Confirmar transacción
        await _unitOfWork.CommitTransactionAsync();

        return tema;
    }
    catch (Exception)
    {
        // Revertir todos los cambios
  await _unitOfWork.RollbackTransactionAsync();
        throw;
    }
}
```

#### Escenario 2: Transferir Temas entre Categorías

```csharp
public async Task TransferirTemas(int categoriaOrigenId, int categoriaDestinoId)
{
    try
    {
        await _unitOfWork.BeginTransactionAsync();

 // Obtener temas
        var temas = await _unitOfWork.Temas.FindAsync(t => t.CategoriaId == categoriaOrigenId);
        
  // Actualizar cada tema
        foreach (var tema in temas)
        {
 tema.CategoriaId = categoriaDestinoId;
 _unitOfWork.Temas.Update(tema);
        }
  
  await _unitOfWork.CommitAsync();
await _unitOfWork.CommitTransactionAsync();
    }
    catch (Exception)
    {
      await _unitOfWork.RollbackTransactionAsync();
        throw;
    }
}
```

## Patrones de Uso Recomendados

### 1. Validación Antes de Insertar

```csharp
public async Task CrearUsuarioAsync(Usuario usuario)
{
    // Validar que el email no exista
    if (await _unitOfWork.Usuarios.AnyAsync(u => u.Email == usuario.Email))
    {
        throw new InvalidOperationException("El email ya está registrado");
    }

    // Validar datos adicionales
    if (string.IsNullOrWhiteSpace(usuario.Nombre))
    {
        throw new ArgumentException("El nombre es obligatorio");
    }

 // Insertar
    await _unitOfWork.Usuarios.AddAsync(usuario);
 await _unitOfWork.CommitAsync();
}
```

### 2. Actualización Segura

```csharp
public async Task ActualizarUsuarioAsync(int id, string nuevoNombre)
{
    var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
    
    if (usuario == null)
  {
 throw new InvalidOperationException($"Usuario con ID {id} no encontrado");
    }

    usuario.Nombre = nuevoNombre;
    _unitOfWork.Usuarios.Update(usuario);
    await _unitOfWork.CommitAsync();
}
```

### 3. Eliminación Lógica vs Física

```csharp
// Eliminación lógica (recomendada)
public async Task DesactivarUsuarioAsync(int id)
{
  var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
    if (usuario != null)
    {
        usuario.Activo = false;
        _unitOfWork.Usuarios.Update(usuario);
        await _unitOfWork.CommitAsync();
    }
}

// Eliminación física (usar con precaución)
public async Task EliminarUsuarioAsync(int id)
{
    var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
    if (usuario != null)
    {
        _unitOfWork.Usuarios.Delete(usuario);
   await _unitOfWork.CommitAsync();
    }
}
```

### 4. Manejo de Errores

```csharp
public async Task<Usuario> CrearUsuarioConManejoErrores(Usuario usuario)
{
    try
    {
        await _unitOfWork.Usuarios.AddAsync(usuario);
        await _unitOfWork.CommitAsync();
        return usuario;
    }
    catch (DbUpdateException ex)
    {
        // Error de base de datos (violación de restricción, etc.)
        throw new InvalidOperationException("Error al guardar el usuario en la base de datos", ex);
    }
    catch (Exception ex)
    {
        // Otros errores
      throw new Exception("Error inesperado al crear usuario", ex);
    }
}
```

## Inyección de Dependencias

### Registrar Servicios (Program.cs)

```csharp
// DbContext
builder.Services.AddDbContext<ForumDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ForumConnection")));

// Unit of Work y Repositorios
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Servicios de aplicación
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ForoService>();
```

### Usar en Controladores

```csharp
public class UsuariosController : Controller
{
    private readonly UsuarioService _usuarioService;

    public UsuariosController(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost]
    public async Task<IActionResult> Crear(Usuario usuario)
    {
        if (!ModelState.IsValid)
return View(usuario);

     try
        {
            await _usuarioService.CrearUsuarioAsync(usuario);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
    {
       ModelState.AddModelError("", ex.Message);
    return View(usuario);
      }
    }
}
```

## Ventajas del Patrón

### 1. Separación de Responsabilidades
- **Domain**: Define QUÉ (entidades, contratos)
- **Application**: Define CÓMO (lógica de negocio)
- **Infrastructure**: Define DÓNDE (acceso a datos)

### 2. Testabilidad
```csharp
// Mock del Unit of Work para pruebas unitarias
var mockUnitOfWork = new Mock<IUnitOfWork>();
mockUnitOfWork.Setup(u => u.Usuarios.GetByIdAsync(It.IsAny<int>()))
         .ReturnsAsync(new Usuario { Id = 1, Nombre = "Test" });

var service = new UsuarioService(mockUnitOfWork.Object);
```

### 3. Reutilización
- Un solo repositorio genérico para todas las entidades
- Lógica común centralizada
- Fácil mantenimiento

### 4. Transacciones
- Control completo sobre transacciones
- Rollback automático en caso de error
- Integridad de datos garantizada

## Mejores Prácticas

1. ? Siempre validar antes de insertar/actualizar
2. ? Usar transacciones para operaciones relacionadas
3. ? Manejar excepciones específicas
4. ? Preferir eliminación lógica sobre física
5. ? Usar métodos asíncronos (async/await)
6. ? Inyectar IUnitOfWork en lugar de DbContext
7. ? Mantener servicios delgados y enfocados
8. ? Documentar métodos públicos

## Resumen

El patrón Repository + Unit of Work proporciona:
- ? Abstracción del acceso a datos
- ? Testabilidad mejorada
- ? Gestión de transacciones
- ? Código limpio y mantenible
- ? Reutilización de código
- ? Separación de responsabilidades
