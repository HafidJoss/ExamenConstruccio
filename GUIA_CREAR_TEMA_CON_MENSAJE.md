# Guía de Implementación - Crear Tema con Primer Mensaje

## 📋 Resumen

Se ha implementado un sistema completo para crear temas con su primer mensaje asociado usando:
- **Casos de Uso (Use Cases)** en la capa de Application
- **Transacciones** para garantizar consistencia de datos
- **DTOs** para separar capas
- **AutoMapper** para mapeo automático
- **Vista previa** del mensaje antes de enviar
- **Validación** del lado cliente y servidor

## 🏗️ Arquitectura Implementada

```
┌─────────────────────────────────────────────────────────────┐
│                    SalesSuite.Web (Presentation)             │
│  ┌──────────────────┐        ┌──────────────────┐           │
│  │ TemasController  │───────▶│  CrearTemaDto    │           │
│  └──────────────────┘        └──────────────────┘           │
│           │                                                  │
│           ▼                                                  │
├─────────────────────────────────────────────────────────────┤
│              SalesSuite.Application (Business Logic)         │
│  ┌──────────────────────────────────────────────┐           │
│  │         ICrearTemaHandler                    │           │
│  │  ┌────────────────────────────────────────┐  │           │
│  │  │  CrearTemaHandler                      │  │           │
│  │  │  - Validación de negocio               │  │           │
│  │  │  - Gestión de transacciones            │  │           │
│  │  │  - Creación de Tema + Mensaje          │  │           │
│  │  └────────────────────────────────────────┘  │           │
│  └──────────────────────────────────────────────┘           │
│           │                                                  │
│           ▼                                                  │
├─────────────────────────────────────────────────────────────┤
│            SalesSuite.Infrastructure (Data Access)           │
│  ┌──────────────────┐        ┌──────────────────┐           │
│  │   UnitOfWork     │───────▶│  ForumDbContext  │           │
│  │  - Repositories  │        │  - DbSets        │           │
│  │  - Transactions  │        │  - Migrations    │           │
│  └──────────────────┘        └──────────────────┘           │
│           │                                                  │
│           ▼                                                  │
├─────────────────────────────────────────────────────────────┤
│                  SalesSuite.Domain (Entities)                │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐                   │
│  │   Tema   │  │ Mensaje  │  │ Usuario  │                   │
│  └──────────┘  └──────────┘  └──────────┘                   │
└─────────────────────────────────────────────────────────────┘
```

## 📁 Archivos Creados/Modificados

### **Capa Application (Nueva)**

```
SalesSuite.Application/
├── UseCases/
│   └── Temas/
│       ├── CrearTemaCommand.cs          - Comando con datos de entrada
│       ├── CrearTemaResult.cs           - Resultado de la operación
│       ├── ICrearTemaHandler.cs         - Interfaz del handler
│       └── CrearTemaHandler.cs          - Implementación del caso de uso
└── DependencyInjection.cs               - Configuración de DI
```

### **DTOs (Modificados)**

```
SalesSuite.Web/DTOs/
└── TemaDto.cs
    ├── CrearTemaDto                     - DTO para crear tema con mensaje
    └── TemaCreateDto                    - DTO simple (legacy)
```

### **Controlador (Modificado)**

```
SalesSuite.Web/Controllers/
└── TemasController.cs
    ├── Create (GET)                     - Muestra formulario
    └── Create (POST)                    - Procesa con caso de uso
```

### **Vista (Modificada)**

```
SalesSuite.Web/Views/Temas/
└── Create.cshtml
    ├── Formulario con validación
    ├── Vista previa del mensaje
    ├── Contador de caracteres
    └── JavaScript personalizado
```

### **Configuración (Modificada)**

```
SalesSuite.Web/
└── Program.cs                           - Agregado AddApplication()
```

## 🔄 Flujo de Creación de Tema

### **1. Usuario accede al formulario (GET /Temas/Create)**

```
Usuario → TemasController.Create() → Vista con formulario
```

### **2. Usuario completa el formulario**

- Título del tema
- Categoría
- Contenido del primer mensaje
- Opción de fijar (opcional)

### **3. Usuario hace clic en "Vista Previa" (Opcional)**

```javascript
JavaScript valida campos → Muestra preview → Scroll suave
```

### **4. Usuario envía el formulario (POST /Temas/Create)**

```
TemasController.Create(CrearTemaDto)
    ↓
Validación ModelState
    ↓
Crear CrearTemaCommand
    ↓
ICrearTemaHandler.HandleAsync(command)
    ↓
┌─────────────────────────────────────┐
│  CrearTemaHandler                   │
│  1. Validar comando                 │
│  2. Iniciar transacción             │
│  3. Validar categoría existe        │
│  4. Validar usuario existe          │
│  5. Crear entidad Tema              │
│  6. Guardar Tema (CommitAsync)      │
│  7. Crear entidad Mensaje           │
│  8. Guardar Mensaje (CommitAsync)   │
│  9. Confirmar transacción           │
│  10. Retornar resultado             │
└─────────────────────────────────────┘
    ↓
Si éxito: Redirect a Details
Si error: Mostrar errores en formulario
```

## ✨ Características Implementadas

### **1. Caso de Uso (CrearTemaHandler)**

#### **Validaciones de Negocio**
```csharp
- Título: 5-200 caracteres
- Contenido: 10-5000 caracteres
- Categoría: debe existir y estar activa
- Usuario: debe existir y estar activo
```

#### **Gestión de Transacciones**
```csharp
await _unitOfWork.BeginTransactionAsync();
try {
    // Crear tema
    // Crear mensaje
    await _unitOfWork.CommitTransactionAsync();
} catch {
    await _unitOfWork.RollbackTransactionAsync();
}
```

#### **Generación Automática de Slug**
```csharp
"Mi Tema de Prueba" → "mi-tema-de-prueba"
```

### **2. Vista con JavaScript Avanzado**

#### **Contador de Caracteres**
- Actualización en tiempo real
- Cambio de color según límite
- 4000+ caracteres: amarillo
- 4500+ caracteres: rojo

#### **Vista Previa del Mensaje**
- Validación antes de mostrar
- Renderizado con saltos de línea
- Scroll suave automático
- Animación de entrada

#### **Validación del Lado Cliente**
```javascript
- Campos obligatorios
- Longitud mínima/máxima
- Formato correcto
- Prevención de doble submit
- Advertencia de pérdida de datos
```

#### **Prevención de Pérdida de Datos**
```javascript
window.beforeunload → Advertencia si hay cambios sin guardar
```

### **3. Validación del Lado Servidor**

#### **Data Annotations en DTO**
```csharp
[Required(ErrorMessage = "...")]
[StringLength(200, MinimumLength = 5)]
```

#### **Validación en Caso de Uso**
```csharp
ValidateCommand() → Lista de errores
```

#### **Validación en Controlador**
```csharp
ModelState.IsValid → Retornar vista con errores
```

## 🎯 Ejemplo de Uso

### **Código del Controlador**

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(CrearTemaDto crearTemaDto)
{
    if (!ModelState.IsValid)
    {
        await CargarCategoriasViewBag(crearTemaDto.CategoriaId);
        return View(crearTemaDto);
    }

    // Obtener usuario autenticado
    int usuarioId = 1; // TODO: Implementar autenticación

    // Crear comando
    var command = new CrearTemaCommand
    {
        Titulo = crearTemaDto.Titulo,
        ContenidoMensaje = crearTemaDto.ContenidoMensaje,
        CategoriaId = crearTemaDto.CategoriaId,
        UsuarioId = usuarioId,
        Fijado = crearTemaDto.Fijado
    };

    // Ejecutar caso de uso
    var result = await _crearTemaHandler.HandleAsync(command);

    if (!result.Success)
    {
        // Manejar errores
        foreach (var error in result.ValidationErrors)
        {
            ModelState.AddModelError(string.Empty, error);
        }
        return View(crearTemaDto);
    }

    TempData["Success"] = "Tema y primer mensaje creados exitosamente.";
    return RedirectToAction(nameof(Details), new { id = result.TemaId });
}
```

### **Código del Caso de Uso**

```csharp
public async Task<CrearTemaResult> HandleAsync(CrearTemaCommand command)
{
    // Validar
    var errors = ValidateCommand(command);
    if (errors.Any())
        return CrearTemaResult.CreateValidationError(errors);

    try
    {
        // Iniciar transacción
        await _unitOfWork.BeginTransactionAsync();

        // Crear tema
        var tema = new Tema { /* ... */ };
        await _unitOfWork.Temas.AddAsync(tema);
        await _unitOfWork.CommitAsync();

        // Crear mensaje
        var mensaje = new Mensaje { TemaId = tema.Id, /* ... */ };
        await _unitOfWork.Mensajes.AddAsync(mensaje);
        await _unitOfWork.CommitAsync();

        // Confirmar transacción
        await _unitOfWork.CommitTransactionAsync();

        return CrearTemaResult.CreateSuccess(tema.Id, mensaje.Id);
    }
    catch (Exception ex)
    {
        await _unitOfWork.RollbackTransactionAsync();
        return CrearTemaResult.CreateError(ex.Message);
    }
}
```

## 📦 Paquetes NuGet Adicionales

### **SalesSuite.Application**
```bash
# No requiere paquetes adicionales, solo referencias a:
# - SalesSuite.Domain
# - SalesSuite.Infrastructure (para IUnitOfWork)
# - Microsoft.Extensions.Logging.Abstractions
```

## 🚀 Pasos para Ejecutar

### **1. Compilar la Solución**

```bash
dotnet build
```

### **2. Crear Datos de Prueba**

Asegúrate de tener al menos:
- 1 Usuario activo (ID = 1)
- 1 Categoría activa

```sql
-- Usuario de prueba
INSERT INTO Usuarios (NombreUsuario, Email, NombreCompleto, PasswordHash, Rol, Activo, FechaRegistro)
VALUES ('admin', 'admin@forum.com', 'Administrador', 'hash', 'Administrador', 1, GETUTCDATE());

-- Categoría de prueba
INSERT INTO Categorias (Nombre, Descripcion, Slug, Orden, Activa, FechaCreacion)
VALUES ('General', 'Temas generales', 'general', 1, 1, GETUTCDATE());
```

### **3. Ejecutar la Aplicación**

```bash
dotnet run --project SalesSuite.Web
```

### **4. Navegar al Formulario**

```
https://localhost:5001/Temas/Create
```

### **5. Completar el Formulario**

1. Título: "Mi primer tema"
2. Categoría: Seleccionar "General"
3. Contenido: "Este es el contenido de mi primer mensaje..."
4. (Opcional) Hacer clic en "Vista Previa"
5. Hacer clic en "Crear Tema"

## 🔍 Verificación

### **En la Base de Datos**

```sql
-- Verificar tema creado
SELECT * FROM Temas ORDER BY Id DESC;

-- Verificar mensaje creado
SELECT * FROM Mensajes ORDER BY Id DESC;

-- Verificar relación
SELECT t.Titulo, m.Contenido, u.NombreUsuario, c.Nombre as Categoria
FROM Temas t
INNER JOIN Mensajes m ON m.TemaId = t.Id
INNER JOIN Usuarios u ON t.UsuarioId = u.Id
INNER JOIN Categorias c ON t.CategoriaId = c.Id
ORDER BY t.Id DESC;
```

### **En los Logs**

```
info: SalesSuite.Application.UseCases.Temas.CrearTemaHandler[0]
      Tema creado: ID=1, Título='Mi primer tema'
info: SalesSuite.Application.UseCases.Temas.CrearTemaHandler[0]
      Primer mensaje creado: ID=1, TemaId=1
info: SalesSuite.Application.UseCases.Temas.CrearTemaHandler[0]
      Tema y mensaje creados exitosamente en transacción: TemaId=1, MensajeId=1
```

## 🐛 Solución de Problemas

### **Error: "La categoría seleccionada no existe"**
- Verifica que exista al menos una categoría activa en la BD
- Ejecuta: `SELECT * FROM Categorias WHERE Activa = 1`

### **Error: "El usuario no existe"**
- Verifica que exista el usuario con ID = 1
- Ejecuta: `SELECT * FROM Usuarios WHERE Id = 1 AND Activo = 1`

### **Error: "No se puede resolver ICrearTemaHandler"**
- Verifica que `builder.Services.AddApplication()` esté en `Program.cs`
- Verifica que el proyecto Web tenga referencia a Application

### **La vista previa no funciona**
- Verifica que jQuery esté cargado antes del script personalizado
- Abre la consola del navegador (F12) para ver errores JavaScript

### **El contador de caracteres no se actualiza**
- Verifica que el ID del textarea sea `contenidoMensaje`
- Verifica que jQuery esté cargado

## 📚 Próximos Pasos

1. **Implementar Autenticación**
   - ASP.NET Core Identity
   - Obtener usuario autenticado real
   - Proteger acciones con `[Authorize]`

2. **Mejorar Vista Previa**
   - Soporte para Markdown
   - Resaltado de sintaxis
   - Emojis

3. **Agregar Notificaciones**
   - Notificar a seguidores de la categoría
   - Email de confirmación
   - Notificaciones en tiempo real (SignalR)

4. **Implementar Caché**
   - Caché de categorías
   - Caché de usuarios
   - Reducir consultas a BD

5. **Agregar Más Validaciones**
   - Detección de spam
   - Límite de temas por usuario/día
   - Validación de contenido ofensivo

## ✅ Checklist de Implementación

- [x] Capa Application creada
- [x] Caso de uso CrearTemaHandler implementado
- [x] DTOs actualizados
- [x] Controlador modificado
- [x] Vista con preview y validación
- [x] JavaScript personalizado
- [x] Transacciones implementadas
- [x] Validación cliente y servidor
- [x] Contador de caracteres
- [x] Prevención de pérdida de datos
- [x] Logging completo
- [x] Manejo de errores robusto
- [x] Documentación completa

## 🎉 Resultado Final

El sistema ahora permite:
1. ✅ Crear un tema con su primer mensaje en una sola operación
2. ✅ Ver vista previa antes de enviar
3. ✅ Validación completa en cliente y servidor
4. ✅ Transacciones para garantizar consistencia
5. ✅ Manejo robusto de errores
6. ✅ Experiencia de usuario mejorada
7. ✅ Arquitectura limpia y mantenible
8. ✅ Código testeable y escalable
