# ?? Corrección de Migraciones de Base de Datos

## ? Problemas Identificados

### 1. **Columnas Duplicadas `ApplicationUserId`**
La migración inicial creaba columnas innecesarias:
- `Temas.ApplicationUserId` ? apuntando a `AspNetUsers`
- `Mensajes.ApplicationUserId` ? apuntando a `AspNetUsers`

Estas columnas eran redundantes porque las tablas ya tenían:
- `Temas.UsuarioId` ? apuntando a `Usuarios` (tabla legacy)
- `Mensajes.UsuarioId` ? apuntando a `Usuarios` (tabla legacy)

### 2. **Conflictos de Cascade Delete**
La configuración inicial tenía rutas de cascade delete en conflicto:
- `Temas.UsuarioId` con `CASCADE`
- `Mensajes.UsuarioId` con `CASCADE`

Esto causaba problemas cuando se intentaba eliminar un usuario.

### 3. **Relaciones Incorrectas**
Entity Framework detectaba relaciones implícitas entre:
- `ApplicationUser.Temas` ? `Tema`
- `ApplicationUser.Mensajes` ? `Mensaje`

Creando foreign keys no deseadas.

---

## ? Soluciones Implementadas

### 1. **Modificación del `ForumDbContext`**
Se agregaron instrucciones `Ignore()` para evitar que EF Core cree relaciones automáticas:

```csharp
modelBuilder.Entity<ApplicationUser>(entity =>
{
    entity.ToTable("AspNetUsers");
    
    // IGNORAR las propiedades de navegación hacia Tema y Mensaje
    entity.Ignore(u => u.Temas);
    entity.Ignore(u => u.Mensajes);
});
```

### 2. **Configuración de DeleteBehavior Correcta**

**Usuario ? Temas/Mensajes:**
```csharp
entity.HasMany(u => u.Temas)
.WithOne(t => t.Usuario)
    .HasForeignKey(t => t.UsuarioId)
    .OnDelete(DeleteBehavior.Restrict); // NO CASCADE
```

**Tema ? Mensajes:**
```csharp
entity.HasMany(t => t.Mensajes)
    .WithOne(m => m.Tema)
    .HasForeignKey(m => m.TemaId)
    .OnDelete(DeleteBehavior.Cascade); // SÍ CASCADE
```

**Categoria ? Temas:**
```csharp
entity.HasMany(c => c.Temas)
    .WithOne(t => t.Categoria)
    .HasForeignKey(t => t.CategoriaId)
    .OnDelete(DeleteBehavior.Restrict); // NO CASCADE
```

---

## ??? Estructura Final de la Base de Datos

### Tabla: `AspNetUsers` (Identity)
| Columna | Tipo | Restricción |
|---------|------|------------|
| **Id** | nvarchar(450) | **PK** |
| UserName | nvarchar(256) | - |
| Email | nvarchar(256) | - |
| NombreCompleto | nvarchar(max) | NOT NULL |
| Biografia | nvarchar(max) | NULL |
| FechaRegistro | datetime2 | NOT NULL |
| Activo | bit | NOT NULL |
| FotoPerfil | nvarchar(max) | NULL |
| ... (otros campos de Identity) | - | - |

### Tabla: `AspNetRoles` (Identity)
| Columna | Tipo | Restricción |
|---------|------|------------|
| **Id** | nvarchar(450) | **PK** |
| Name | nvarchar(256) | - |
| NormalizedName | nvarchar(256) | UNIQUE |

### Tabla: `Usuarios` (Legacy)
| Columna | Tipo | Restricción |
|---------|------|------------|
| **Id** | int | **PK**, IDENTITY |
| Nombre | nvarchar(100) | NOT NULL |
| Email | nvarchar(200) | NOT NULL, UNIQUE |
| Biografia | nvarchar(500) | NULL |
| FechaRegistro | datetime2 | NOT NULL, DEFAULT GETUTCDATE() |
| Activo | bit | NOT NULL, DEFAULT 1 |

### Tabla: `Categorias`
| Columna | Tipo | Restricción |
|---------|------|------------|
| **Id** | int | **PK**, IDENTITY |
| Nombre | nvarchar(150) | NOT NULL, UNIQUE |
| Descripcion | nvarchar(500) | NULL |
| FechaCreacion | datetime2 | NOT NULL, DEFAULT GETUTCDATE() |
| Activa | bit | NOT NULL, DEFAULT 1 |
| Orden | int | NOT NULL, DEFAULT 0 |

### Tabla: `Temas`
| Columna | Tipo | Restricción |
|---------|------|------------|
| **Id** | int | **PK**, IDENTITY |
| Titulo | nvarchar(250) | NOT NULL |
| Contenido | nvarchar(max) | NOT NULL |
| FechaCreacion | datetime2 | NOT NULL, DEFAULT GETUTCDATE() |
| FechaUltimaActividad | datetime2 | NULL |
| Cerrado | bit | NOT NULL, DEFAULT 0 |
| Fijado | bit | NOT NULL, DEFAULT 0 |
| Vistas | int | NOT NULL, DEFAULT 0 |
| **UsuarioId** | int | **FK** ? `Usuarios.Id` (RESTRICT) |
| **CategoriaId** | int | **FK** ? `Categorias.Id` (RESTRICT) |

**Índices:**
- `IX_Temas_FechaCreacion`
- `IX_Temas_CategoriaId`
- `IX_Temas_UsuarioId`

### Tabla: `Mensajes`
| Columna | Tipo | Restricción |
|---------|------|------------|
| **Id** | int | **PK**, IDENTITY |
| Contenido | nvarchar(max) | NOT NULL |
| FechaCreacion | datetime2 | NOT NULL, DEFAULT GETUTCDATE() |
| FechaEdicion | datetime2 | NULL |
| Editado | bit | NOT NULL, DEFAULT 0 |
| **TemaId** | int | **FK** ? `Temas.Id` (CASCADE) |
| **UsuarioId** | int | **FK** ? `Usuarios.Id` (RESTRICT) |

**Índices:**
- `IX_Mensajes_FechaCreacion`
- `IX_Mensajes_TemaId`
- `IX_Mensajes_UsuarioId`

---

## ?? Diagrama de Relaciones

```
???????????????????
?  AspNetUsers    ? (Sistema de Identity)
?  (Identity)     ?
???????????????????
        ?
        ? (Sin relación directa con Temas/Mensajes)
 ?
?
???????????????????
?    Usuarios     ? (Tabla legacy del foro)
?    (Legacy)     ?
???????????????????
  ?
 ? 1:N (RESTRICT)
      ????????????????????????
        ?       ?
        ?       ?
???????????????????    ???????????????????
?     Temas    ?    ?    Mensajes     ?
???????????????????    ???????????????????
        ?            ?
        ? 1:N (CASCADE)        ?
        ????????????????????????
        
???????????????????
?   Categorias?
???????????????????
        ?
        ? 1:N (RESTRICT)
    ?
???????????????????
?     Temas       ?
???????????????????
```

---

## ?? Comandos Ejecutados

### Eliminar migración incorrecta:
```bash
dotnet ef migrations remove
```

### Crear nueva migración:
```bash
dotnet ef migrations add InitialCreateCorrected
```

### Eliminar base de datos anterior:
```bash
dotnet ef database drop --force
```

### Aplicar migración correcta:
```bash
dotnet ef database update
```

---

## ? Beneficios de la Corrección

1. **? Sin columnas duplicadas**: Solo las foreign keys necesarias
2. **? Cascade delete controlado**: 
   - Al eliminar un Tema ? se eliminan sus Mensajes
   - Al eliminar un Usuario ? NO se eliminan sus Temas/Mensajes (RESTRICT)
   - Al eliminar una Categoria ? NO se eliminan sus Temas (RESTRICT)
3. **? Relaciones limpias**: Sin referencias circulares
4. **? Rendimiento optimizado**: Índices correctos en todas las foreign keys
5. **? Integridad referencial**: Garantizada por la base de datos

---

## ?? Puntos Clave

### ?? Importante:
- La tabla `Usuarios` es la tabla **legacy** que mantiene los datos históricos
- `AspNetUsers` es la tabla de **Identity** para autenticación
- Las entidades `Tema` y `Mensaje` **solo** se relacionan con `Usuarios`
- `ApplicationUser` tiene propiedades de navegación, pero están **ignoradas** en EF Core

### ?? Notas:
- Si necesitas migrar datos de `Usuarios` a `AspNetUsers`, deberás crear un script de migración de datos
- Las propiedades de navegación en `ApplicationUser` están presentes pero ignoradas por EF Core
- Esto permite flexibilidad futura si decides implementar la relación directa

---

## ?? Documentación Relacionada

- [DOCUMENTACION_IDENTITY.md](DOCUMENTACION_IDENTITY.md)
- [RESUMEN_VISUAL_IDENTITY.md](RESUMEN_VISUAL_IDENTITY.md)
- [Program.cs](Program.cs) - Configuración de Identity y seeding

---

**Fecha de corrección**: 8 de noviembre de 2024
**Migración actual**: `20251108165346_InitialCreateCorrected`
