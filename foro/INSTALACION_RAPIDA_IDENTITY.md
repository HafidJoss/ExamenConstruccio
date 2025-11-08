# ?? GUÍA DE INSTALACIÓN RÁPIDA - ASP.NET Core Identity

## ?? Pasos para Poner en Marcha

### **1. Instalar Paquetes NuGet** ?

```sh
cd foro

# Instalar paquetes de Identity
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.0
dotnet add package Microsoft.AspNetCore.Identity.UI --version 8.0.0

# Actualizar EF Core si es necesario
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0

# Restaurar todos los paquetes
dotnet restore
```

### **2. Crear Migración de Identity** ???

```sh
# Eliminar migraciones anteriores si existen conflictos
# dotnet ef database drop --force
# Remove-Item -Path "Migrations" -Recurse -Force

# Crear nueva migración con Identity
dotnet ef migrations add AddIdentityTables

# O si prefieres PowerShell (Package Manager Console):
# Add-Migration AddIdentityTables
```

### **3. Aplicar Migración** ??

```sh
# Aplicar migración a la base de datos
dotnet ef database update

# O en PowerShell:
# Update-Database
```

### **4. Ejecutar Aplicación** ??

```sh
dotnet run
```

**O presiona `F5` en Visual Studio.**

---

## ? Verificación

### **En la Consola** (Logs esperados)

```
info: Program[0]
      Iniciando migración y seeding de base de datos...
info: Program[0]
      Aplicando migraciones pendientes...
info: Program[0]
      Migraciones aplicadas exitosamente.
info: Program[0]
      Rol 'Administrador' creado exitosamente.
info: Program[0]
 Rol 'Moderador' creado exitosamente.
info: Program[0]
      Rol 'Usuario' creado exitosamente.
info: Program[0]
      Usuario administrador creado exitosamente: admin@foro.com
info: foro.Infrastructure.Seeding.DataSeeder[0]
      Iniciando proceso de seeding de datos...
info: foro.Infrastructure.Seeding.DataSeeder[0]
  Insertando usuarios de prueba...
[... más logs ...]
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
```

### **En el Navegador**

```
https://localhost:5001/
```

Deberías ver:
- ? Menú con "Iniciar Sesión" y "Registrarse"
- ? Listado de temas (público)
- ? Botón "Nuevo Tema" deshabilitado (requiere login)

### **En SQL Server**

```sql
USE ForumDb;

-- Verificar tablas de Identity
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME LIKE 'AspNet%';

-- Resultado esperado:
-- AspNetUsers
-- AspNetRoles
-- AspNetUserRoles
-- AspNetUserClaims
-- AspNetUserLogins
-- AspNetUserTokens
-- AspNetRoleClaims

-- Verificar usuario administrador
SELECT * FROM AspNetUsers WHERE Email = 'admin@foro.com';

-- Verificar roles
SELECT * FROM AspNetRoles;

-- Resultado esperado:
-- Administrador
-- Moderador
-- Usuario
```

---

## ?? Pruebas Rápidas

### **Test 1: Login como Admin**

```
1. Navegar a: https://localhost:5001/Account/Login

2. Credenciales:
   Email: admin@foro.com
   Password: Admin@123

3. Click "Iniciar Sesión"

? Esperado: Redirige a /Temas, menú muestra "Administrador del Foro"
```

### **Test 2: Registro de Usuario**

```
1. Navegar a: https://localhost:5001/Account/Register

2. Llenar formulario:
   Nombre: Test User
   Email: test@test.com
   Contraseña: Test@123
   Confirmar: Test@123
   ? Acepto términos

3. Click "Crear Cuenta"

? Esperado: Usuario creado, auto-login, redirige a /Temas
```

### **Test 3: Crear Tema Autenticado**

```
1. Login (con cualquier usuario)

2. Click "Nuevo Tema"

3. Llenar:
   Título: Mi Tema de Prueba
   Categoría: ASP.NET Core
   Contenido: Este es un tema de prueba

4. Click "Crear Tema"

? Esperado: Tema creado con usuario actual como autor
```

### **Test 4: Acceso No Autorizado**

```
1. Cerrar sesión

2. Intentar acceder a: https://localhost:5001/Temas/Create

? Esperado: Redirige a /Account/Login?ReturnUrl=%2FTemas%2FCreate
```

---

## ?? Troubleshooting

### **Error: "Cannot find package Microsoft.AspNetCore.Identity.EntityFrameworkCore"**

**Solución**:
```sh
# Verificar versión de .NET
dotnet --version
# Debe ser >= 8.0.0

# Limpiar caché de NuGet
dotnet nuget locals all --clear

# Reinstalar paquetes
dotnet restore --force
```

### **Error: "Table 'AspNetUsers' already exists"**

**Solución**:
```sh
# Opción 1: Eliminar y recrear BD
dotnet ef database drop --force
dotnet ef database update

# Opción 2: Eliminar migración conflictiva
dotnet ef migrations remove
dotnet ef migrations add AddIdentityTables
dotnet ef database update
```

### **Error: "The context 'ForumDbContext' is not configured"**

**Solución**:
Verificar `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "ForumConnection": "Server=(localdb)\\mssqllocaldb;Database=ForumDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### **Error: "No se puede iniciar sesión con admin@foro.com"**

**Causas posibles**:
1. Usuario no se creó ? Revisar logs
2. Contraseña incorrecta ? Usar `Admin@123`
3. Usuario inactivo ? Verificar `Activo = true` en BD

**Solución**:
```sql
-- Verificar usuario
SELECT * FROM AspNetUsers WHERE Email = 'admin@foro.com';

-- Si no existe, ejecutar la app (se crea automáticamente)
-- Si existe pero no funciona, eliminar y dejar que se recree
DELETE FROM AspNetUsers WHERE Email = 'admin@foro.com';
```

---

## ?? Archivos Modificados (Resumen)

```
foro/
??? Domain/
?   ??? Entities/
?       ??? ApplicationUser.cs ................ ? NUEVO
?
??? Infrastructure/
?   ??? Data/
?       ??? ForumDbContext.cs ................ ? MODIFICADO
?
??? Web/
?   ??? Controllers/
?   ?   ??? AccountController.cs ............. ? NUEVO
?   ?   ??? TemasController.cs ............... ? MODIFICADO
?   ?
?   ??? ViewModels/
?   ?   ??? LoginViewModel.cs ................ ? NUEVO
?   ?   ??? RegisterViewModel.cs ............. ? NUEVO
?   ?
?   ??? Views/
?    ??? Account/
?     ? ??? Login.cshtml ................. ? NUEVO
?       ?   ??? Register.cshtml .............. ? NUEVO
?       ?   ??? AccessDenied.cshtml .......... ? NUEVO
?   ?
?       ??? Shared/
?  ??? _Layout.cshtml ............... ? MODIFICADO
?
??? Program.cs .............................. ? MODIFICADO
??? foro.csproj ............................. ? MODIFICADO
?
??? Migrations/
    ??? XXXXXX_AddIdentityTables.cs ......... ? GENERADO
```

---

## ? Checklist de Instalación

```
PAQUETES NUGET
??? ? Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.0)
??? ? Microsoft.AspNetCore.Identity.UI (8.0.0)
??? ? dotnet restore ejecutado

MIGRACIÓN
??? ? dotnet ef migrations add AddIdentityTables
??? ? dotnet ef database update

TABLAS EN BD
??? ? AspNetUsers creada
??? ? AspNetRoles creada
??? ? AspNetUserRoles creada
??? ? Otras tablas de Identity

DATOS INICIALES
??? ? Roles creados (Administrador, Moderador, Usuario)
??? ? Usuario admin creado (admin@foro.com)

EJECUCIÓN
??? ? dotnet run sin errores
??? ? Logs muestran creación de roles
??? ? Logs muestran creación de usuario admin
??? ? App escuchando en https://localhost:5001

PRUEBAS
??? ? Login funciona
??? ? Register funciona
??? ? Logout funciona
??? ? Redirección a login cuando no autenticado
??? ? Crear tema con usuario autenticado
```

---

## ?? Comando de Inicio Completo

```sh
# Paso 1: Instalar paquetes
cd foro
dotnet restore

# Paso 2: Crear migración
dotnet ef migrations add AddIdentityTables

# Paso 3: Aplicar migración
dotnet ef database update

# Paso 4: Ejecutar aplicación
dotnet run

# Paso 5: Abrir navegador
start https://localhost:5001

# Paso 6: Login como admin
# Email: admin@foro.com
# Pass: Admin@123
```

---

## ?? Estado Final

```
??????????????????????????????????????????????????????
?          ?
?  ? IDENTITY INSTALADO Y FUNCIONANDO       ?
??
?  Credenciales Admin:     ?
?    ?? Email: admin@foro.com         ?
?    ?? Pass:  Admin@123  ?
?      ?
?  URLs:    ?
?    ?? https://localhost:5001/ ?
?    ?? https://localhost:5001/Account/Login         ?
?    ? https://localhost:5001/Account/Register      ?
?    ?? https://localhost:5001/Temas      ?
?           ?
??????????????????????????????????????????????????????
```

**¡Todo listo para usar!** ??

---

## ?? Soporte

Si encuentras algún problema:

1. **Revisar logs** en la consola al ejecutar `dotnet run`
2. **Verificar base de datos** con SQL Server Management Studio
3. **Limpiar y rebuild**:
   ```sh
   dotnet clean
   dotnet build
   ```
4. **Eliminar BD y empezar de cero**:
   ```sh
   dotnet ef database drop --force
   dotnet ef migrations remove
   dotnet ef migrations add AddIdentityTables
   dotnet run
   ```

**Documentación completa**: `DOCUMENTACION_IDENTITY.md`

---

**¡Identity implementado con éxito!** ?
