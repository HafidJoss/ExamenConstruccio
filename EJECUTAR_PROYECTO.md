# 🚀 Guía Final - Ejecutar Proyecto SalesSuite

## ✅ Componentes Creados Automáticamente

### **Archivos de Proyecto**
- ✅ `SalesSuite.Domain.csproj`
- ✅ `SalesSuite.Application.csproj`
- ✅ `SalesSuite.Infrastructure.csproj`
- ✅ `SalesSuite.Web.csproj`
- ✅ `SalesSuite.sln`

### **Vistas de Autenticación**
- ✅ `Views/Account/Login.cshtml`
- ✅ `Views/Account/Register.cshtml`
- ✅ `Views/Account/AccessDenied.cshtml`

### **Vistas de ViewComponents**
- ✅ `Views/Shared/Components/TemasRecientes/Default.cshtml`
- ✅ `Views/Shared/Components/UsuarioPanel/Default.cshtml`
- ✅ `Views/Shared/Components/UsuarioPanel/Anonymous.cshtml`
- ✅ `Views/Shared/Components/EstadisticasForo/Default.cshtml`

### **Seeding**
- ✅ `Infrastructure/Seeding/IdentitySeeder.cs`
- ✅ `Program.cs` actualizado con llamada a IdentitySeeder

### **Estilos**
- ✅ `wwwroot/css/forum-ux.css`

---

## 📝 Tareas Pendientes (Manuales)

### **1. Actualizar _Layout.cshtml**

Abre `Views/Shared/_Layout.cshtml` y reemplaza el navbar con:

```cshtml
<nav class="navbar navbar-expand-lg navbar-dark bg-primary">
    <div class="container-fluid">
        <a class="navbar-brand" asp-controller="Home" asp-action="Index">
            <i class="bi bi-chat-square-text me-2"></i>SalesSuite Forum
        </a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarNav">
            <ul class="navbar-nav me-auto">
                <li class="nav-item">
                    <a class="nav-link" asp-controller="Home" asp-action="Index">
                        <i class="bi bi-house me-1"></i>Inicio
                    </a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" asp-controller="Temas" asp-action="Index">
                        <i class="bi bi-list-ul me-1"></i>Temas
                    </a>
                </li>
            </ul>
            <ul class="navbar-nav">
                @if (User.Identity?.IsAuthenticated == true)
                {
                    <li class="nav-item dropdown">
                        <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown">
                            <i class="bi bi-person-circle me-1"></i>@User.Identity.Name
                        </a>
                        <ul class="dropdown-menu dropdown-menu-end">
                            <li>
                                <a class="dropdown-item" asp-controller="Temas" asp-action="Create">
                                    <i class="bi bi-plus-circle me-2"></i>Crear Tema
                                </a>
                            </li>
                            <li><hr class="dropdown-divider"></li>
                            <li>
                                <form asp-controller="Account" asp-action="Logout" method="post" class="d-inline">
                                    <button type="submit" class="dropdown-item">
                                        <i class="bi bi-box-arrow-right me-2"></i>Cerrar Sesión
                                    </button>
                                </form>
                            </li>
                        </ul>
                    </li>
                }
                else
                {
                    <li class="nav-item">
                        <a class="nav-link" asp-controller="Account" asp-action="Login">
                            <i class="bi bi-box-arrow-in-right me-1"></i>Iniciar Sesión
                        </a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" asp-controller="Account" asp-action="Register">
                            <i class="bi bi-person-plus me-1"></i>Registrarse
                        </a>
                    </li>
                }
            </ul>
        </div>
    </div>
</nav>
```

### **2. Agregar Bootstrap Icons al _Layout.cshtml**

En el `<head>` de `_Layout.cshtml`, agrega:

```cshtml
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css">
<link rel="stylesheet" href="~/css/forum-ux.css" />
```

---

## 🔨 Comandos para Compilar y Ejecutar

### **PASO 1: Restaurar Paquetes NuGet**

```bash
cd c:\Users\PC\Downloads\Examen
dotnet restore
```

### **PASO 2: Compilar la Solución**

```bash
dotnet build
```

**Verifica que no haya errores de compilación.**

### **PASO 3: Eliminar Migraciones Anteriores (si existen)**

```bash
# Eliminar base de datos
dotnet ef database drop --project SalesSuite.Infrastructure --startup-project SalesSuite.Web --force

# Eliminar carpeta de migraciones
Remove-Item -Path "c:\Users\PC\Downloads\Examen\SalesSuite.Infrastructure\Data\Migrations" -Recurse -Force -ErrorAction SilentlyContinue
```

### **PASO 4: Crear Nueva Migración**

```bash
dotnet ef migrations add InitialWithIdentity `
    --project SalesSuite.Infrastructure `
    --startup-project SalesSuite.Web `
    --context ForumDbContext `
    --output-dir Data/Migrations
```

### **PASO 5: Ejecutar la Aplicación**

```bash
dotnet run --project SalesSuite.Web
```

**Deberías ver en los logs:**

```
info: Aplicando migraciones pendientes...
info: Migraciones aplicadas exitosamente.
info: Ejecutando seeding de datos...
info: Insertados 5 usuarios de ejemplo.
info: Insertadas 8 categorías del foro.
info: Insertados 6 temas de ejemplo.
info: Insertados 13 mensajes de ejemplo.
info: Seeding completado exitosamente.
info: Ejecutando Identity seeding...
info: Rol Administrador creado exitosamente
info: Rol Moderador creado exitosamente
info: Rol Usuario creado exitosamente
info: Usuario administrador creado: admin@forumsales.com
info: Rol Administrador asignado a admin@forumsales.com
info: Identity seeding completado exitosamente.
info: Now listening on: https://localhost:5001
```

---

## 🧪 Verificación del Proyecto

### **1. Abrir Navegador**

Navega a: `https://localhost:5001`

### **2. Probar Registro**

1. Ve a `/Account/Register`
2. Completa el formulario:
   - Nombre de usuario: `testuser`
   - Email: `test@example.com`
   - Nombre completo: `Usuario de Prueba`
   - Contraseña: `Test123!`
   - Confirmar contraseña: `Test123!`
3. Click en "Crear Cuenta"
4. Deberías ser redirigido al inicio con sesión iniciada

### **3. Probar Login con Admin**

1. Cierra sesión
2. Ve a `/Account/Login`
3. Credenciales:
   - Email: `admin@forumsales.com`
   - Contraseña: `Admin123!`
4. Click en "Iniciar Sesión"

### **4. Verificar ViewComponents**

En cualquier página, deberías ver en el sidebar:
- ✅ Panel de usuario (con tu nombre y estadísticas)
- ✅ Temas recientes (últimos 5 temas)
- ✅ Estadísticas del foro (totales)

### **5. Probar Crear Tema**

1. Estando logueado, ve a `/Temas/Create`
2. Completa:
   - Título: "Mi primer tema"
   - Categoría: Selecciona una
   - Contenido: "Este es mi primer mensaje"
3. Click en "Crear Tema"

---

## 📊 Estructura Final del Proyecto

```
SalesSuite/
├── SalesSuite.Domain/
│   ├── Entities/
│   │   ├── ApplicationUser.cs ✅
│   │   ├── Usuario.cs
│   │   ├── Categoria.cs
│   │   ├── Tema.cs
│   │   └── Mensaje.cs
│   ├── Interfaces/
│   └── SalesSuite.Domain.csproj ✅
│
├── SalesSuite.Application/
│   ├── UseCases/
│   ├── DependencyInjection.cs
│   └── SalesSuite.Application.csproj ✅
│
├── SalesSuite.Infrastructure/
│   ├── Data/
│   │   ├── ForumDbContext.cs
│   │   └── Migrations/ (se creará)
│   ├── Repositories/
│   ├── Seeding/
│   │   ├── DataSeeder.cs
│   │   └── IdentitySeeder.cs ✅
│   ├── DependencyInjection.cs
│   └── SalesSuite.Infrastructure.csproj ✅
│
├── SalesSuite.Web/
│   ├── Controllers/
│   │   ├── AccountController.cs
│   │   ├── TemasController.cs
│   │   └── HomeController.cs
│   ├── ViewComponents/
│   │   ├── TemasRecientesViewComponent.cs ✅
│   │   ├── UsuarioPanelViewComponent.cs ✅
│   │   └── EstadisticasForoViewComponent.cs ✅
│   ├── Views/
│   │   ├── Account/
│   │   │   ├── Login.cshtml ✅
│   │   │   ├── Register.cshtml ✅
│   │   │   └── AccessDenied.cshtml ✅
│   │   ├── Shared/
│   │   │   ├── Components/
│   │   │   │   ├── TemasRecientes/Default.cshtml ✅
│   │   │   │   ├── UsuarioPanel/Default.cshtml ✅
│   │   │   │   ├── UsuarioPanel/Anonymous.cshtml ✅
│   │   │   │   └── EstadisticasForo/Default.cshtml ✅
│   │   │   └── _Layout.cshtml (⚠️ ACTUALIZAR)
│   │   └── Temas/
│   ├── wwwroot/
│   │   └── css/
│   │       └── forum-ux.css ✅
│   ├── ViewModels/
│   ├── DTOs/
│   ├── Mappings/
│   ├── Program.cs ✅
│   ├── appsettings.json ✅
│   └── SalesSuite.Web.csproj ✅
│
└── SalesSuite.sln ✅
```

---

## ✅ Checklist Final

### **Archivos Creados**
- [x] Archivos .csproj (4 proyectos)
- [x] SalesSuite.sln
- [x] IdentitySeeder.cs
- [x] Vistas de Account (3 archivos)
- [x] Vistas de ViewComponents (4 archivos)
- [x] forum-ux.css
- [x] Program.cs actualizado

### **Tareas Pendientes**
- [ ] Actualizar _Layout.cshtml con navbar de autenticación
- [ ] Agregar Bootstrap Icons al _Layout.cshtml
- [ ] Restaurar paquetes NuGet
- [ ] Compilar proyecto
- [ ] Crear migración
- [ ] Ejecutar aplicación

### **Verificación**
- [ ] Aplicación compila sin errores
- [ ] Base de datos se crea automáticamente
- [ ] Datos de ejemplo se insertan
- [ ] Roles y admin se crean
- [ ] Login funciona
- [ ] Registro funciona
- [ ] ViewComponents se muestran
- [ ] Crear tema funciona

---

## 🎯 Credenciales de Acceso

### **Usuario Administrador**
- Email: `admin@forumsales.com`
- Contraseña: `Admin123!`
- Rol: Administrador

### **Usuarios de Ejemplo (del DataSeeder)**
- Email: `juan.perez@email.com` / Contraseña: `Password123!`
- Email: `maria.garcia@email.com` / Contraseña: `Password123!`
- Email: `carlos.lopez@email.com` / Contraseña: `Password123!`
- Email: `ana.martinez@email.com` / Contraseña: `Password123!`
- Email: `luis.rodriguez@email.com` / Contraseña: `Password123!`

---

## 🚨 Solución de Problemas

### **Error: No se encuentra el proyecto**
```bash
# Asegúrate de estar en la raíz del proyecto
cd c:\Users\PC\Downloads\Examen
```

### **Error de compilación**
```bash
# Limpia y reconstruye
dotnet clean
dotnet build
```

### **Error de migración**
```bash
# Verifica que la cadena de conexión esté correcta en appsettings.json
# Debe ser: Server=(localdb)\\mssqllocaldb;Database=ForumDB;...
```

### **ViewComponents no se muestran**
- Verifica que las vistas estén en las carpetas correctas
- Verifica que los nombres coincidan exactamente
- Revisa los logs para ver errores específicos

---

## 🎉 ¡Proyecto Listo!

Tu foro SalesSuite está completamente configurado con:

✅ **Arquitectura Limpia** (Domain, Application, Infrastructure, Web)
✅ **ASP.NET Core Identity** (Autenticación y autorización)
✅ **Entity Framework Core** (ORM con SQL Server)
✅ **ViewComponents Reutilizables** (Temas recientes, panel de usuario, estadísticas)
✅ **Bootstrap 5** (Diseño responsive y moderno)
✅ **Seeding Automático** (Datos de ejemplo y usuario admin)
✅ **Migraciones Automáticas** (Base de datos se crea al iniciar)

**¡Disfruta tu foro!** 🚀
