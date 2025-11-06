# Guía de Implementación - TemasController

## 📋 Resumen

Se ha implementado un sistema completo de gestión de temas para el foro con:
- **TemasController** con operaciones CRUD completas
- **DTOs** para separar la capa de presentación del dominio
- **AutoMapper** para mapeo automático entre entidades y DTOs
- **Vistas Razor** modernas con Bootstrap 5
- **Paginación** usando X.PagedList
- **Búsqueda y filtrado** por título y categoría

## 📁 Estructura de Archivos Creados

### **DTOs (SalesSuite.Web/DTOs/)**
```
├── TemaDto.cs              - DTO para mostrar temas
├── TemaCreateDto.cs        - DTO para crear temas
├── TemaEditDto.cs          - DTO para editar temas
├── CategoriaDto.cs         - DTO para categorías
└── UsuarioDto.cs           - DTO para usuarios
```

### **Mappings (SalesSuite.Web/Mappings/)**
```
└── MappingProfile.cs       - Configuración de AutoMapper
```

### **Controllers (SalesSuite.Web/Controllers/)**
```
└── TemasController.cs      - Controlador con CRUD completo
```

### **Views (SalesSuite.Web/Views/Temas/)**
```
├── Index.cshtml            - Listado con búsqueda y paginación
├── Create.cshtml           - Formulario de creación
├── Edit.cshtml             - Formulario de edición
├── Delete.cshtml           - Confirmación de eliminación
└── Details.cshtml          - Detalles del tema
```

### **Configuración**
```
├── Program.cs              - Configuración de servicios
├── appsettings.json        - Configuración de producción
└── appsettings.Development.json - Configuración de desarrollo
```

### **Layout y Estilos**
```
├── Views/Shared/_Layout.cshtml
├── Views/Shared/_ValidationScriptsPartial.cshtml
├── Views/_ViewImports.cshtml
├── Views/_ViewStart.cshtml
├── wwwroot/css/site.css
└── wwwroot/js/site.js
```

## 🚀 Instalación de Paquetes NuGet

Ejecuta los siguientes comandos en cada proyecto:

### **SalesSuite.Domain**
```bash
# No requiere paquetes adicionales
```

### **SalesSuite.Infrastructure**
```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0
dotnet add package Microsoft.Extensions.Configuration.Abstractions --version 8.0.0
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions --version 8.0.0
```

### **SalesSuite.Web**
```bash
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add package AutoMapper --version 12.0.1
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 12.0.1
dotnet add package X.PagedList --version 8.4.7
dotnet add package X.PagedList.Mvc.Core --version 8.4.7
```

## ⚙️ Configuración

### **1. Cadena de Conexión**

Edita `appsettings.json` con tu cadena de conexión:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=ForumDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### **2. Crear Base de Datos**

Ejecuta las migraciones de Entity Framework:

```bash
# Desde la carpeta raíz del proyecto
dotnet ef migrations add InitialCreate --project SalesSuite.Infrastructure --startup-project SalesSuite.Web

# Aplicar la migración
dotnet ef database update --project SalesSuite.Infrastructure --startup-project SalesSuite.Web
```

### **3. Datos de Prueba (Opcional)**

Puedes crear un seeder para agregar datos de prueba:

```csharp
// En SalesSuite.Infrastructure/Data/DbSeeder.cs
public static class DbSeeder
{
    public static async Task SeedAsync(ForumDbContext context)
    {
        // Crear usuario de prueba
        if (!await context.Usuarios.AnyAsync())
        {
            var usuario = new Usuario
            {
                NombreUsuario = "admin",
                Email = "admin@forum.com",
                NombreCompleto = "Administrador",
                PasswordHash = "hash_aqui",
                Rol = "Administrador",
                Activo = true
            };
            await context.Usuarios.AddAsync(usuario);
            await context.SaveChangesAsync();
        }

        // Crear categorías
        if (!await context.Categorias.AnyAsync())
        {
            var categorias = new[]
            {
                new Categoria { Nombre = "General", Descripcion = "Temas generales", Slug = "general", Orden = 1 },
                new Categoria { Nombre = "Tecnología", Descripcion = "Discusiones sobre tecnología", Slug = "tecnologia", Orden = 2 },
                new Categoria { Nombre = "Ayuda", Descripcion = "Solicita ayuda aquí", Slug = "ayuda", Orden = 3 }
            };
            await context.Categorias.AddRangeAsync(categorias);
            await context.SaveChangesAsync();
        }
    }
}
```

Llama al seeder en `Program.cs`:

```csharp
// Después de app.Build()
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ForumDbContext>();
    await DbSeeder.SeedAsync(context);
}
```

## 🎯 Funcionalidades Implementadas

### **TemasController**

#### **Index (GET)**
- ✅ Listado paginado de temas (10 por página)
- ✅ Búsqueda por título o contenido
- ✅ Filtro por categoría
- ✅ Ordenamiento (fijados primero, luego por última actividad)
- ✅ Muestra información del autor, categoría, vistas y respuestas

#### **Details (GET)**
- ✅ Muestra detalles completos del tema
- ✅ Incrementa contador de vistas automáticamente
- ✅ Incluye información del autor y categoría
- ✅ Preparado para mostrar respuestas (mensajes)

#### **Create (GET/POST)**
- ✅ Formulario con validaciones
- ✅ Selector de categoría
- ✅ Opción para fijar tema
- ✅ Generación automática de slug
- ✅ Validación de ModelState

#### **Edit (GET/POST)**
- ✅ Formulario pre-rellenado
- ✅ Permite cambiar título, contenido y categoría
- ✅ Opciones para cerrar/fijar tema
- ✅ Actualiza fecha de última actividad

#### **Delete (GET/POST)**
- ✅ Vista de confirmación con detalles
- ✅ Advertencia sobre eliminación en cascada
- ✅ Eliminación segura con validaciones

### **Características de las Vistas**

#### **Bootstrap 5**
- ✅ Diseño responsive
- ✅ Iconos de Bootstrap Icons
- ✅ Cards con efectos hover
- ✅ Alertas con auto-cierre
- ✅ Formularios estilizados
- ✅ Botones con iconos

#### **Validación**
- ✅ Validación del lado del cliente (jQuery Validate)
- ✅ Validación del lado del servidor (Data Annotations)
- ✅ Mensajes de error personalizados en español
- ✅ Resaltado de campos con errores

#### **UX/UI**
- ✅ Breadcrumbs de navegación
- ✅ Mensajes de éxito/error con TempData
- ✅ Contador de caracteres en textareas
- ✅ Tooltips y popovers
- ✅ Animaciones suaves
- ✅ Diseño limpio y moderno

## 📝 Uso del Sistema

### **Crear un Tema**

1. Navega a `/Temas`
2. Haz clic en "Crear Nuevo Tema"
3. Completa el formulario:
   - Título (5-200 caracteres)
   - Categoría (obligatoria)
   - Contenido (10-5000 caracteres)
   - Opción de fijar (opcional)
4. Haz clic en "Crear Tema"

### **Buscar Temas**

1. En la página de índice, usa los filtros:
   - Campo de búsqueda: busca en título y contenido
   - Selector de categoría: filtra por categoría específica
2. Haz clic en "Filtrar"
3. Para limpiar filtros, haz clic en "Limpiar"

### **Editar un Tema**

1. En el listado o detalles, haz clic en el botón "Editar"
2. Modifica los campos deseados
3. Haz clic en "Guardar Cambios"

### **Eliminar un Tema**

1. Haz clic en el botón "Eliminar"
2. Revisa la información del tema
3. Confirma haciendo clic en "Sí, eliminar tema"

## 🔧 Personalización

### **Cambiar Tamaño de Página**

En `TemasController.cs`, línea 36:
```csharp
int pageSize = 10; // Cambia este valor
```

### **Modificar Estilos**

Edita `wwwroot/css/site.css` para personalizar:
- Colores
- Tipografía
- Espaciados
- Animaciones

### **Agregar Campos al Tema**

1. Agrega la propiedad en `Tema.cs` (Domain)
2. Agrega la propiedad en los DTOs correspondientes
3. Actualiza el mapeo en `MappingProfile.cs`
4. Agrega el campo en las vistas
5. Crea una nueva migración

## 🐛 Solución de Problemas

### **Error: No se puede conectar a la base de datos**
- Verifica la cadena de conexión en `appsettings.json`
- Asegúrate de que SQL Server esté ejecutándose
- Verifica los permisos de usuario

### **Error: No se encuentran las vistas**
- Verifica que las vistas estén en `Views/Temas/`
- Asegúrate de que `_ViewImports.cshtml` y `_ViewStart.cshtml` existan
- Limpia y reconstruye la solución

### **Error: AutoMapper no mapea correctamente**
- Verifica que `MappingProfile` esté registrado en `Program.cs`
- Revisa los nombres de las propiedades en entidades y DTOs
- Usa `ForMember` para mapeos personalizados

### **Error: X.PagedList no funciona**
- Instala el paquete NuGet `X.PagedList.Mvc.Core`
- Agrega `@using X.PagedList.Mvc.Core` en `_ViewImports.cshtml`
- Verifica que el modelo de la vista sea `IPagedList<TemaDto>`

## 📚 Próximos Pasos

1. **Implementar autenticación y autorización**
   - ASP.NET Core Identity
   - Roles y permisos
   - Proteger acciones del controlador

2. **Agregar funcionalidad de mensajes/respuestas**
   - MensajesController
   - Vistas para crear/editar mensajes
   - Sistema de hilos anidados

3. **Implementar búsqueda avanzada**
   - Búsqueda full-text
   - Filtros adicionales
   - Ordenamiento personalizado

4. **Agregar características sociales**
   - Sistema de "me gusta"
   - Seguir temas
   - Notificaciones

5. **Mejorar el rendimiento**
   - Caché de datos
   - Lazy loading
   - Compresión de respuestas

## 📖 Referencias

- [ASP.NET Core MVC](https://docs.microsoft.com/aspnet/core/mvc/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [AutoMapper](https://docs.automapper.org/)
- [Bootstrap 5](https://getbootstrap.com/docs/5.3/)
- [X.PagedList](https://github.com/dncuug/X.PagedList)

## ✅ Checklist de Implementación

- [x] DTOs creados y documentados
- [x] AutoMapper configurado
- [x] TemasController implementado
- [x] Vistas Razor con Bootstrap 5
- [x] Validaciones del lado del cliente y servidor
- [x] Paginación con X.PagedList
- [x] Búsqueda y filtrado
- [x] Mensajes de éxito/error
- [x] Estilos personalizados
- [x] JavaScript para UX mejorada
- [x] Documentación completa
