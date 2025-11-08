# ?? Guía de Inicio Rápido - TemasController

## ? Pasos para ejecutar el proyecto

### 1?? Crear la Base de Datos

```bash
# En la carpeta del proyecto foro
cd foro

# Crear migración inicial
dotnet ef migrations add InitialCreate

# Aplicar migración a la base de datos
dotnet ef database update
```

### 2?? Insertar Datos de Prueba

Ejecuta el script SQL en SQL Server Management Studio o Azure Data Studio:

```
Ubicación: foro\Database\SeedData.sql
```

Este script insertará:
- ? 5 usuarios de prueba
- ? 8 categorías
- ? 20 temas variados
- ? Varios mensajes

### 3?? Ejecutar el Proyecto

```bash
dotnet run
```

O presiona **F5** en Visual Studio

### 4?? Probar las Funcionalidades

Abre el navegador en: `https://localhost:5001/Temas` (o el puerto que use tu proyecto)

---

## ?? URLs Disponibles

| Acción | URL | Descripción |
|--------|-----|-------------|
| **Listado** | `/Temas` | Ver todos los temas |
| **Búsqueda** | `/Temas?searchString=JWT` | Buscar temas por título |
| **Filtro** | `/Temas?categoriaId=2` | Filtrar por categoría |
| **Crear** | `/Temas/Create` | Formulario de nuevo tema |
| **Editar** | `/Temas/Edit/5` | Editar tema #5 |
| **Detalles** | `/Temas/Details/5` | Ver detalles del tema #5 |
| **Eliminar** | `/Temas/Delete/5` | Confirmar eliminación |

---

## ?? Pruebas Funcionales

### ? Test 1: Ver Listado
1. Navegar a `/Temas`
2. Verificar que se muestran los temas
3. Verificar paginación si hay más de 10 temas

### ? Test 2: Buscar Tema
1. En el campo de búsqueda, escribir "JWT"
2. Click en "Filtrar"
3. Verificar que solo aparecen temas que contienen "JWT"

### ? Test 3: Filtrar por Categoría
1. Seleccionar "ASP.NET Core" del dropdown
2. Click en "Filtrar"
3. Verificar que solo aparecen temas de esa categoría

### ? Test 4: Crear Tema
1. Click en "Nuevo Tema"
2. Llenar el formulario:
   - **Título**: "Mi primer tema de prueba"
   - **Categoría**: Seleccionar una
   - **Contenido**: "Este es el contenido de prueba"
3. Click en "Crear Tema"
4. Verificar mensaje de éxito
5. Verificar que aparece en el listado

### ? Test 5: Editar Tema
1. En el listado, click en el ícono de editar (lápiz)
2. Cambiar el título
3. Cambiar la categoría
4. Click en "Guardar Cambios"
5. Verificar mensaje de éxito
6. Verificar cambios en el listado

### ? Test 6: Ver Detalles
1. Click en el ícono de ver (ojo)
2. Verificar que se muestran todos los datos
3. Verificar que incrementa el contador de vistas

### ? Test 7: Eliminar Tema
1. Click en el ícono de eliminar (basurero)
2. Leer la advertencia
3. Click en "Confirmar Eliminación"
4. Verificar mensaje de éxito
5. Verificar que ya no aparece en el listado

### ? Test 8: Validaciones
1. Intentar crear tema sin título ? Error
2. Intentar crear tema sin categoría ? Error
3. Intentar crear tema con contenido corto ? Error
4. Verificar que los mensajes de validación son claros

---

## ?? Características Visuales a Verificar

### Index (Listado)
- [ ] Header con título y botón "Nuevo Tema"
- [ ] Alertas de éxito/error (si hay)
- [ ] Card con filtros de búsqueda
- [ ] Listado de temas con:
  - [ ] Badge de categoría
  - [ ] Nombre del autor
  - [ ] Fecha de creación
  - [ ] Número de mensajes
  - [ ] Número de vistas
  - [ ] Íconos para temas fijados/cerrados
  - [ ] Botones de ver/editar/eliminar
- [ ] Paginación funcional con "««", "«", números, "»", "»»"

### Create (Crear)
- [ ] Card con formulario limpio
- [ ] Campo de título grande
- [ ] Dropdown de categorías poblado
- [ ] Textarea grande para contenido
- [ ] Alerta info con consejos
- [ ] Botones "Cancelar" y "Crear Tema"
- [ ] Validación en tiempo real

### Edit (Editar)
- [ ] Card con header amarillo (warning)
- [ ] Campos pre-llenados
- [ ] Alerta explicando limitaciones
- [ ] Info de fecha de creación
- [ ] Botones "Cancelar" y "Guardar Cambios"

### Delete (Eliminar)
- [ ] Card con header rojo (danger)
- [ ] Alerta de advertencia prominente
- [ ] Card con toda la información del tema
- [ ] Contenido truncado si es muy largo
- [ ] Botones "Cancelar" y "Confirmar Eliminación"

### Details (Detalles)
- [ ] Breadcrumb de navegación
- [ ] Título del tema
- [ ] Badges de categoría, autor, fecha, mensajes
- [ ] Botones de editar y eliminar
- [ ] Contenido formateado
- [ ] Sección de mensajes
- [ ] Botón "Volver al Listado"

---

## ?? Troubleshooting

### Error: "No se puede conectar a la base de datos"
**Solución**: Verificar la cadena de conexión en `appsettings.json`

```json
"ConnectionStrings": {
  "ForumConnection": "Server=(localdb)\\mssqllocaldb;Database=ForumDb;Trusted_Connection=true;MultipleActiveResultSets=true"
}
```

### Error: "AutoMapper.AutoMapperMappingException"
**Solución**: Verificar que `MappingProfile` está registrado en `Program.cs`

```csharp
builder.Services.AddAutoMapper(typeof(Program).Assembly);
```

### Error: "No se encuentra X.PagedList"
**Solución**: Restaurar paquetes NuGet

```bash
dotnet restore
```

### Los temas no se filtran correctamente
**Solución**: Verificar que las categorías tienen el campo `Activa = true`

### Las vistas no se encuentran
**Solución**: Verificar que las vistas están en `Views/Temas/`

---

## ?? Datos de Prueba Incluidos

### Usuarios
- admin@foro.com
- juan.perez@mail.com
- maria.garcia@mail.com
- carlos.lopez@mail.com
- ana.martinez@mail.com

### Categorías
1. .NET Framework
2. ASP.NET Core
3. Entity Framework
4. Desarrollo Web
5. Bases de Datos
6. DevOps & Cloud
7. Arquitectura de Software
8. General

### Temas Destacados
- 2 temas fijados (Bienvenida y Normas)
- 18 temas variados sobre diferentes tecnologías
- Temas con diferentes fechas y vistas
- Algunos con mensajes asociados

---

## ?? Próximos Pasos Sugeridos

1. **Autenticación**: Integrar ASP.NET Core Identity
2. **Autorización**: Solo el autor puede editar/eliminar
3. **Avatares**: Agregar imágenes de perfil
4. **Markdown**: Soporte para formato de texto
5. **Búsqueda Avanzada**: Búsqueda por contenido, autor, fechas
6. **Notificaciones**: Alertar cuando hay respuestas
7. **Likes/Votos**: Sistema de votación
8. **Tags**: Etiquetar temas para mejor organización
9. **Moderación**: Herramientas para moderadores
10. **API REST**: Exponer endpoints para consumo externo

---

## ?? Estructura de Archivos

```
foro/
??? Web/
?   ??? Controllers/
?   ?   ??? TemasController.cs ?
?   ??? DTOs/
?   ?   ??? TemaListDto.cs ?
?   ?   ??? TemaCreateDto.cs ?
?   ?   ??? TemaEditDto.cs ?
?   ?   ??? TemaDeleteDto.cs ?
?   ?   ??? CategoriaDto.cs ?
?   ??? Mappings/
?   ?   ??? MappingProfile.cs ?
?   ??? Views/
?       ??? Temas/
?           ??? Index.cshtml ?
?           ??? Create.cshtml ?
?        ??? Edit.cshtml ?
?        ??? Delete.cshtml ?
?       ??? Details.cshtml ?
??? Database/
?   ??? SeedData.sql ?
??? Program.cs (actualizado) ?
??? foro.csproj (paquetes agregados) ?
```

---

## ? Checklist de Verificación

Antes de considerar el módulo completo, verifica:

- [ ] Base de datos creada y migrada
- [ ] Datos de prueba insertados
- [ ] Proyecto compila sin errores
- [ ] Listado de temas funciona
- [ ] Búsqueda funciona
- [ ] Filtro por categoría funciona
- [ ] Paginación funciona
- [ ] Crear tema funciona con validaciones
- [ ] Editar tema funciona
- [ ] Ver detalles funciona
- [ ] Eliminar tema funciona
- [ ] Mensajes de TempData se muestran
- [ ] Estilos de Bootstrap se aplican correctamente
- [ ] Iconos de Bootstrap se muestran
- [ ] Responsive design funciona en móvil

---

## ?? ¡Listo!

Si todos los checkpoints están ?, el módulo de **TemasController** está completamente funcional y listo para usar.

**Documentación completa en**: `DOCUMENTACION_TEMAS_CONTROLLER.md`

---

## ?? Tips de Desarrollo

### Modo Debug
```bash
# Ver logs detallados
dotnet run --environment Development
```

### Hot Reload
Visual Studio 2022 tiene Hot Reload activado por defecto. Los cambios en vistas se reflejan automáticamente.

### Ver SQL Generado
Agrega logging en `appsettings.Development.json`:

```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.EntityFrameworkCore.Database.Command": "Information"
  }
}
```

### Depuración de AutoMapper
```csharp
// En Program.cs, después de AddAutoMapper
var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});
mappingConfig.AssertConfigurationIsValid();
```

---

## ?? Soporte

Si encuentras algún problema:

1. Revisa los logs de la aplicación
2. Verifica los errores de compilación
3. Consulta la documentación completa
4. Revisa que todos los archivos estén en las ubicaciones correctas

**¡Feliz coding! ??**
