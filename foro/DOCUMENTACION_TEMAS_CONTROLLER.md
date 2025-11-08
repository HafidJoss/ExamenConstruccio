# TemasController - Gestión del Foro

## ?? Resumen

Este documento describe la implementación completa del `TemasController` que gestiona las operaciones principales del foro de temas con arquitectura limpia, DTOs, AutoMapper y vistas modernas con Bootstrap 5.

---

## ??? Arquitectura Implementada

```
??????????????????????????????????????????????????????????
?  Presentation Layer (Views)   ?
?  - Index.cshtml   (listado + búsqueda + paginación)    ?
?  - Create.cshtml(formulario de creación)        ?
?  - Edit.cshtml    (formulario de edición)   ?
?  - Delete.cshtml  (confirmación de eliminación)        ?
?  - Details.cshtml (detalles del tema)       ?
??????????????????????????????????????????????????????????
     ? usa
??????????????????????????????????????????????????????????
?    Web Layer (Controllers & DTOs)             ?
?  - TemasController.cs       ?
?  - TemaListDto.cs           ?
?  - TemaCreateDto.cs      ?
?  - TemaEditDto.cs   ?
?  - TemaDeleteDto.cs    ?
?  - CategoriaDto.cs      ?
?  - MappingProfile.cs (AutoMapper)    ?
??????????????????????????????????????????????????????????
     ? usa
??????????????????????????????????????????????????????????
?       Application Layer (Services)   ?
?  IUnitOfWork | ForoService | UsuarioService?
??????????????????????????????????????????????????????????
     ? usa
??????????????????????????????????????????????????????????
?         Domain Layer (Entities)       ?
?  Tema | Categoria | Usuario | Mensaje         ?
??????????????????????????????????????????????????????????
```

---

## ?? Paquetes NuGet Instalados

```xml
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
<PackageReference Include="X.PagedList.Mvc.Core" Version="10.5.9" />
<PackageReference Include="X.PagedList" Version="10.5.9" />
```

---

## ?? DTOs Creados

### 1. TemaListDto
**Propósito**: Mostrar información de temas en el listado

```csharp
public class TemaListDto
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string CategoriaNombre { get; set; }
    public int CategoriaId { get; set; }
    public string UsuarioNombre { get; set; }
  public int UsuarioId { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int NumeroMensajes { get; set; }
    public int Vistas { get; set; }
    public bool Cerrado { get; set; }
    public bool Fijado { get; set; }
}
```

### 2. TemaCreateDto
**Propósito**: Crear un nuevo tema con validaciones

```csharp
public class TemaCreateDto
{
    [Required]
    [StringLength(250, MinimumLength = 5)]
 public string Titulo { get; set; }
    
    [Required]
    [StringLength(5000, MinimumLength = 10)]
    public string Contenido { get; set; }
    
    [Required]
    public int CategoriaId { get; set; }
    
    public int UsuarioId { get; set; }
}
```

### 3. TemaEditDto
**Propósito**: Editar título y categoría de un tema existente

```csharp
public class TemaEditDto
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(250, MinimumLength = 5)]
    public string Titulo { get; set; }
    
    [Required]
    public int CategoriaId { get; set; }
    
    public int UsuarioId { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

### 4. TemaDeleteDto
**Propósito**: Mostrar información antes de eliminar

```csharp
public class TemaDeleteDto
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Contenido { get; set; }
    public string CategoriaNombre { get; set; }
 public string UsuarioNombre { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int NumeroMensajes { get; set; }
}
```

### 5. CategoriaDto
**Propósito**: Dropdowns de categorías

```csharp
public class CategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
}
```

---

## ?? AutoMapper - MappingProfile

```csharp
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Tema ? TemaListDto (con relaciones)
        CreateMap<Tema, TemaListDto>()
       .ForMember(dest => dest.CategoriaNombre, 
    opt => opt.MapFrom(src => src.Categoria.Nombre))
         .ForMember(dest => dest.UsuarioNombre, 
        opt => opt.MapFrom(src => src.Usuario.Nombre))
   .ForMember(dest => dest.NumeroMensajes, 
    opt => opt.MapFrom(src => src.Mensajes.Count));

      // Tema ? TemaEditDto
    CreateMap<Tema, TemaEditDto>();
        CreateMap<TemaEditDto, Tema>()
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.Categoria, opt => opt.Ignore())
 .ForMember(dest => dest.Mensajes, opt => opt.Ignore());

        // TemaCreateDto ? Tema
        CreateMap<TemaCreateDto, Tema>()
 .ForMember(dest => dest.FechaCreacion, 
                opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.FechaUltimaActividad, 
       opt => opt.MapFrom(src => DateTime.UtcNow));

        // Tema ? TemaDeleteDto
        CreateMap<Tema, TemaDeleteDto>()
     .ForMember(dest => dest.CategoriaNombre, 
  opt => opt.MapFrom(src => src.Categoria.Nombre))
        .ForMember(dest => dest.UsuarioNombre, 
    opt => opt.MapFrom(src => src.Usuario.Nombre));

        // Categoria ? CategoriaDto
        CreateMap<Categoria, CategoriaDto>();
    }
}
```

---

## ?? TemasController - Acciones

### GET: Index
**Ruta**: `/Temas/Index?searchString=texto&categoriaId=1&page=2`

**Funcionalidades**:
- ? Listado de todos los temas
- ? Búsqueda por título
- ? Filtro por categoría
- ? Paginación (10 items por página)
- ? Ordenamiento (fijados primero, luego por fecha)
- ? Muestra categoría, autor, fecha, mensajes y vistas

**Ejemplo de uso**:
```csharp
// Todos los temas
/Temas

// Búsqueda
/Temas?searchString=.NET

// Filtro por categoría
/Temas?categoriaId=1

// Página 2 con búsqueda
/Temas?searchString=ASP&page=2
```

### GET: Details
**Ruta**: `/Temas/Details/5`

**Funcionalidades**:
- ? Muestra detalles completos del tema
- ? Incrementa contador de vistas
- ? Muestra número de mensajes
- ? Botones de editar y eliminar

### GET: Create
**Ruta**: `/Temas/Create`

**Funcionalidades**:
- ? Formulario para crear nuevo tema
- ? Dropdown de categorías activas
- ? Validación del lado del cliente
- ? Campos: Título, Categoría, Contenido

### POST: Create
**Funcionalidades**:
- ? Validación del ModelState
- ? Validación de categoría activa
- ? Validación de usuario activo
- ? Mapeo con AutoMapper
- ? Guardado con UnitOfWork
- ? Mensajes de éxito/error con TempData

### GET: Edit
**Ruta**: `/Temas/Edit/5`

**Funcionalidades**:
- ? Formulario pre-llenado con datos actuales
- ? Solo permite editar título y categoría
- ? Muestra información de creación

### POST: Edit
**Funcionalidades**:
- ? Validación del ModelState
- ? Validación de categoría activa
- ? Actualización parcial de campos
- ? Mensajes de éxito/error

### GET: Delete
**Ruta**: `/Temas/Delete/5`

**Funcionalidades**:
- ? Muestra confirmación con todos los datos
- ? Alerta de advertencia
- ? Muestra número de mensajes que se eliminarán

### POST: Delete
**Funcionalidades**:
- ? Eliminación del tema
- ? Eliminación en cascada de mensajes (configurado en DbContext)
- ? Mensajes de éxito/error

---

## ?? Vistas con Bootstrap 5

### Características Generales
- ? Bootstrap 5.x
- ? Bootstrap Icons
- ? Diseño responsivo
- ? Alertas de éxito/error
- ? Formularios con validación
- ? Cards y badges
- ? Navegación con breadcrumbs

### Index.cshtml
**Elementos visuales**:
- ?? Barra de búsqueda con filtros
- ?? Listado paginado con 10 items por página
- ??? Badges de categoría, autor, mensajes y vistas
- ?? Íconos para temas fijados y cerrados
- ??? Botones de ver, editar y eliminar
- ?? Controles de paginación estilizados

### Create.cshtml
**Elementos visuales**:
- ?? Formulario grande y limpio
- ?? Consejos útiles con alertas info
- ?? Validación en tiempo real
- ?? Campos con íconos descriptivos

### Edit.cshtml
**Elementos visuales**:
- ?? Formulario con advertencia de campos editables
- ?? Información de fecha de creación
- ?? Alerta explicando limitaciones

### Delete.cshtml
**Elementos visuales**:
- ?? Alerta de peligro prominente
- ?? Card con toda la información del tema
- ? Botón de confirmación rojo
- ?? Botón de cancelar

### Details.cshtml
**Elementos visuales**:
- ??? Breadcrumb de navegación
- ?? Estadísticas del tema
- ?? Contenido formateado
- ?? Sección de mensajes (placeholder)

---

## ?? Configuración en Program.cs

```csharp
// Configurar AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);
```

---

## ?? Flujo de Datos - Ejemplo: Crear Tema

```
1. Usuario accede a /Temas/Create
 ?
2. GET: TemasController.Create()
   ? Carga categorías activas
   ? Retorna vista con formulario
   ?
3. Usuario llena formulario y envía
   ?
4. POST: TemasController.Create(TemaCreateDto)
   ? Valida ModelState
   ? Valida categoría activa
   ? Valida usuario activo
   ? AutoMapper: TemaCreateDto ? Tema
   ? _unitOfWork.Temas.AddAsync(tema)
   ? _unitOfWork.CommitAsync()
   ? TempData["Success"] = mensaje
   ?
5. RedirectToAction("Index")
   ? Muestra mensaje de éxito
   ? Tema aparece en el listado
```

---

## ?? Validaciones Implementadas

### En DTOs (Data Annotations)
```csharp
[Required(ErrorMessage = "...")]
[StringLength(250, MinimumLength = 5, ErrorMessage = "...")]
[Display(Name = "...")]
```

### En Controlador
- ? ModelState.IsValid
- ? Categoría existe y está activa
- ? Usuario existe y está activo
- ? Tema existe (en Edit/Delete)
- ? ID coincide (en Edit)

---

## ?? Características Destacadas

### 1. Separación de Responsabilidades
- **DTOs**: Solo datos de transferencia
- **Entities**: Lógica de dominio
- **Controller**: Coordinación y flujo
- **Views**: Presentación

### 2. AutoMapper
- Mapeo automático entre entidades y DTOs
- Mapeo de propiedades navegación
- Configuración centralizada en MappingProfile

### 3. Búsqueda y Filtrado
```csharp
// Por título (case-insensitive)
temas.Where(t => t.Titulo.Contains(searchString, StringComparison.OrdinalIgnoreCase))

// Por categoría
temas.Where(t => t.CategoriaId == categoriaId.Value)

// Ordenamiento
temas.OrderByDescending(t => t.Fijado)
     .ThenByDescending(t => t.FechaUltimaActividad ?? t.FechaCreacion)
```

### 4. Paginación con X.PagedList
```csharp
var temasPaginados = temasDto.ToPagedList(pageNumber, pageSize);
```

### 5. Mensajes al Usuario
```csharp
TempData["Success"] = "Tema creado exitosamente.";
TempData["Error"] = "Error al crear el tema.";
```

### 6. Logging
```csharp
_logger.LogInformation("Tema creado: {TemaId}", tema.Id);
_logger.LogError(ex, "Error al crear el tema");
```

---

## ?? Pruebas Manuales

### Crear Base de Datos
```bash
dotnet ef migrations add AddTemasModule
dotnet ef database update
```

### Insertar Datos de Prueba
```sql
-- Usuario de prueba
INSERT INTO Usuarios (Nombre, Email, FechaRegistro, Activo)
VALUES ('Admin', 'admin@foro.com', GETUTCDATE(), 1);

-- Categorías de prueba
INSERT INTO Categorias (Nombre, Descripcion, FechaCreacion, Activa, Orden)
VALUES 
    ('.NET', 'Temas sobre .NET y C#', GETUTCDATE(), 1, 1),
    ('Web Development', 'HTML, CSS, JavaScript', GETUTCDATE(), 1, 2),
    ('Bases de Datos', 'SQL Server, Entity Framework', GETUTCDATE(), 1, 3);
```

### Pruebas de Funcionalidad
1. ? Acceder a `/Temas` - Ver listado vacío
2. ? Crear nuevo tema con categoría
3. ? Verificar que aparece en el listado
4. ? Buscar por título
5. ? Filtrar por categoría
6. ? Editar tema existente
7. ? Ver detalles del tema
8. ? Eliminar tema
9. ? Probar paginación (crear 15+ temas)

---

## ?? Mejoras Futuras

### Autenticación y Autorización
- [ ] Integrar ASP.NET Core Identity
- [ ] Solo el autor puede editar/eliminar
- [ ] Roles: Admin, Moderador, Usuario

### Funcionalidad de Mensajes
- [ ] Crear mensajes en temas
- [ ] Editar/eliminar mensajes
- [ ] Paginación de mensajes

### Mejoras de UI/UX
- [ ] Editor WYSIWYG (TinyMCE, CKEditor)
- [ ] Markdown support
- [ ] Avatares de usuarios
- [ ] Notificaciones en tiempo real

### Performance
- [ ] Caching con IMemoryCache
- [ ] Eager loading optimizado
- [ ] Índices de base de datos

### SEO
- [ ] URLs amigables
- [ ] Meta tags dinámicos
- [ ] Sitemap.xml

---

## ?? Referencias

- [AutoMapper Documentation](https://docs.automapper.org/)
- [X.PagedList Documentation](https://github.com/dncuug/X.PagedList)
- [Bootstrap 5 Documentation](https://getbootstrap.com/docs/5.0/)
- [Bootstrap Icons](https://icons.getbootstrap.com/)
- [ASP.NET Core MVC](https://docs.microsoft.com/en-us/aspnet/core/mvc/)

---

## ? Resumen de Archivos Creados

```
foro/
??? Web/
?   ??? Controllers/
?   ?   ??? TemasController.cs
?   ??? DTOs/
?   ?   ??? TemaListDto.cs
?   ?   ??? TemaCreateDto.cs
?   ?   ??? TemaEditDto.cs
?   ?   ??? TemaDeleteDto.cs
?   ?   ??? CategoriaDto.cs
?   ??? Mappings/
?   ?   ??? MappingProfile.cs
?   ??? Views/
?   ??? Temas/
?        ??? Index.cshtml
?         ??? Create.cshtml
?           ??? Edit.cshtml
?           ??? Delete.cshtml
?  ??? Details.cshtml
??? Program.cs (actualizado)
```

---

## ?? ¡Implementación Completa!

El `TemasController` está completamente implementado con:
- ? 5 DTOs bien diseñados
- ? AutoMapper configurado
- ? Controlador con 8 acciones
- ? 5 vistas modernas con Bootstrap 5
- ? Búsqueda y filtrado
- ? Paginación funcional
- ? Validaciones completas
- ? Mensajes amigables al usuario
- ? Logging integrado
- ? Código limpio y mantenible
