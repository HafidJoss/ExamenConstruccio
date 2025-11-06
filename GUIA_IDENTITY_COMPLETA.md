# Guía Completa - Implementación de ASP.NET Core Identity

## ✅ Archivos Ya Creados

Los siguientes archivos ya han sido creados automáticamente:

1. ✅ `SalesSuite.Domain/Entities/ApplicationUser.cs`
2. ✅ `SalesSuite.Infrastructure/Data/ForumDbContext.cs` (actualizado)
3. ✅ `SalesSuite.Web/ViewModels/LoginViewModel.cs`
4. ✅ `SalesSuite.Web/ViewModels/RegisterViewModel.cs`
5. ✅ `SalesSuite.Web/Controllers/AccountController.cs`
6. ✅ `SalesSuite.Web/Program.cs` (actualizado con Identity)
7. ✅ `SalesSuite.Web/Controllers/TemasController.cs` (protegido con [Authorize])

## 📝 Archivos que Debes Crear Manualmente

### **IMPORTANTE: Primero corrige appsettings.json**

Abre `SalesSuite.Web/appsettings.json` y cambia:
```json
"Server=(localbd);Database=ForumDB;..."
```

A:
```json
"Server=(localdb)\\mssqllocaldb;Database=ForumDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

---

### **1. Login.cshtml**

Crea el archivo: `SalesSuite.Web/Views/Account/Login.cshtml`

```cshtml
@model SalesSuite.Web.ViewModels.LoginViewModel

@{
    ViewData["Title"] = "Iniciar Sesión";
}

<div class="container py-5">
    <div class="row justify-content-center">
        <div class="col-md-6 col-lg-5">
            <div class="card shadow">
                <div class="card-header bg-primary text-white text-center">
                    <h3 class="mb-0">
                        <i class="bi bi-box-arrow-in-right me-2"></i>Iniciar Sesión
                    </h3>
                </div>
                <div class="card-body p-4">
                    <form asp-action="Login" method="post">
                        <div asp-validation-summary="ModelOnly" class="alert alert-danger" role="alert"></div>
                        
                        <div class="mb-3">
                            <label asp-for="Email" class="form-label"></label>
                            <div class="input-group">
                                <span class="input-group-text"><i class="bi bi-envelope"></i></span>
                                <input asp-for="Email" class="form-control" placeholder="correo@ejemplo.com" autofocus />
                            </div>
                            <span asp-validation-for="Email" class="text-danger small"></span>
                        </div>

                        <div class="mb-3">
                            <label asp-for="Password" class="form-label"></label>
                            <div class="input-group">
                                <span class="input-group-text"><i class="bi bi-lock"></i></span>
                                <input asp-for="Password" class="form-control" placeholder="••••••••" />
                            </div>
                            <span asp-validation-for="Password" class="text-danger small"></span>
                        </div>

                        <div class="mb-3 form-check">
                            <input asp-for="RememberMe" class="form-check-input" />
                            <label asp-for="RememberMe" class="form-check-label"></label>
                        </div>

                        <input type="hidden" asp-for="ReturnUrl" />

                        <div class="d-grid gap-2">
                            <button type="submit" class="btn btn-primary btn-lg">
                                <i class="bi bi-box-arrow-in-right me-2"></i>Iniciar Sesión
                            </button>
                        </div>
                    </form>
                </div>
                <div class="card-footer text-center bg-light">
                    <p class="mb-0">
                        ¿No tienes cuenta? 
                        <a asp-action="Register" class="text-decoration-none fw-bold">Regístrate aquí</a>
                    </p>
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

---

### **2. Register.cshtml**

Crea el archivo: `SalesSuite.Web/Views/Account/Register.cshtml`

```cshtml
@model SalesSuite.Web.ViewModels.RegisterViewModel

@{
    ViewData["Title"] = "Registrarse";
}

<div class="container py-5">
    <div class="row justify-content-center">
        <div class="col-md-8 col-lg-6">
            <div class="card shadow">
                <div class="card-header bg-success text-white text-center">
                    <h3 class="mb-0">
                        <i class="bi bi-person-plus me-2"></i>Crear Cuenta
                    </h3>
                </div>
                <div class="card-body p-4">
                    <form asp-action="Register" method="post">
                        <div asp-validation-summary="ModelOnly" class="alert alert-danger" role="alert"></div>
                        
                        <div class="mb-3">
                            <label asp-for="UserName" class="form-label"></label>
                            <div class="input-group">
                                <span class="input-group-text"><i class="bi bi-person"></i></span>
                                <input asp-for="UserName" class="form-control" placeholder="usuario123" autofocus />
                            </div>
                            <span asp-validation-for="UserName" class="text-danger small"></span>
                        </div>

                        <div class="mb-3">
                            <label asp-for="Email" class="form-label"></label>
                            <div class="input-group">
                                <span class="input-group-text"><i class="bi bi-envelope"></i></span>
                                <input asp-for="Email" class="form-control" placeholder="correo@ejemplo.com" />
                            </div>
                            <span asp-validation-for="Email" class="text-danger small"></span>
                        </div>

                        <div class="mb-3">
                            <label asp-for="NombreCompleto" class="form-label"></label>
                            <div class="input-group">
                                <span class="input-group-text"><i class="bi bi-person-badge"></i></span>
                                <input asp-for="NombreCompleto" class="form-control" placeholder="Juan Pérez" />
                            </div>
                            <span asp-validation-for="NombreCompleto" class="text-danger small"></span>
                        </div>

                        <div class="mb-3">
                            <label asp-for="Password" class="form-label"></label>
                            <div class="input-group">
                                <span class="input-group-text"><i class="bi bi-lock"></i></span>
                                <input asp-for="Password" class="form-control" placeholder="••••••••" />
                            </div>
                            <span asp-validation-for="Password" class="text-danger small"></span>
                            <div class="form-text">
                                <small>Mínimo 6 caracteres, debe incluir mayúsculas, minúsculas y números</small>
                            </div>
                        </div>

                        <div class="mb-3">
                            <label asp-for="ConfirmPassword" class="form-label"></label>
                            <div class="input-group">
                                <span class="input-group-text"><i class="bi bi-lock-fill"></i></span>
                                <input asp-for="ConfirmPassword" class="form-control" placeholder="••••••••" />
                            </div>
                            <span asp-validation-for="ConfirmPassword" class="text-danger small"></span>
                        </div>

                        <div class="mb-3">
                            <label asp-for="Biografia" class="form-label"></label>
                            <textarea asp-for="Biografia" class="form-control" rows="3" 
                                      placeholder="Cuéntanos algo sobre ti (opcional)"></textarea>
                            <span asp-validation-for="Biografia" class="text-danger small"></span>
                        </div>

                        <div class="d-grid gap-2">
                            <button type="submit" class="btn btn-success btn-lg">
                                <i class="bi bi-person-plus me-2"></i>Registrarse
                            </button>
                        </div>
                    </form>
                </div>
                <div class="card-footer text-center bg-light">
                    <p class="mb-0">
                        ¿Ya tienes cuenta? 
                        <a asp-action="Login" class="text-decoration-none fw-bold">Inicia sesión aquí</a>
                    </p>
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

---

### **3. AccessDenied.cshtml**

Crea el archivo: `SalesSuite.Web/Views/Account/AccessDenied.cshtml`

```cshtml
@{
    ViewData["Title"] = "Acceso Denegado";
}

<div class="container py-5">
    <div class="row justify-content-center">
        <div class="col-md-6">
            <div class="card shadow border-danger">
                <div class="card-header bg-danger text-white text-center">
                    <h3 class="mb-0">
                        <i class="bi bi-exclamation-triangle me-2"></i>Acceso Denegado
                    </h3>
                </div>
                <div class="card-body text-center p-5">
                    <i class="bi bi-shield-x text-danger" style="font-size: 5rem;"></i>
                    <h4 class="mt-4">No tienes permiso para acceder a esta página</h4>
                    <p class="text-muted">
                        Esta sección requiere permisos especiales que tu cuenta no posee.
                    </p>
                    <div class="mt-4">
                        <a asp-controller="Home" asp-action="Index" class="btn btn-primary me-2">
                            <i class="bi bi-house me-2"></i>Ir al Inicio
                        </a>
                        <a asp-controller="Account" asp-action="Login" class="btn btn-outline-secondary">
                            <i class="bi bi-box-arrow-in-right me-2"></i>Iniciar Sesión
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
```

---

### **4. Actualizar _Layout.cshtml**

Abre `SalesSuite.Web/Views/Shared/_Layout.cshtml` y actualiza la navbar para incluir opciones de autenticación.

Busca la sección del navbar y reemplázala con:

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
                        <a class="nav-link dropdown-toggle" href="#" id="userDropdown" role="button" 
                           data-bs-toggle="dropdown" aria-expanded="false">
                            <i class="bi bi-person-circle me-1"></i>@User.Identity.Name
                        </a>
                        <ul class="dropdown-menu dropdown-menu-end" aria-labelledby="userDropdown">
                            <li>
                                <a class="dropdown-item" asp-controller="Temas" asp-action="Create">
                                    <i class="bi bi-plus-circle me-2"></i>Crear Tema
                                </a>
                            </li>
                            <li><hr class="dropdown-divider"></li>
                            <li>
                                <form asp-controller="Account" asp-action="Logout" method="post" class="d-inline">
                                    <button type="submit" class="dropdown-item text-danger">
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

---

## 🔧 Pasos Siguientes

### **1. Instalar Paquetes NuGet**

Ejecuta estos comandos en el proyecto correspondiente:

```bash
# En SalesSuite.Domain
dotnet add package Microsoft.Extensions.Identity.Stores --version 8.0.0

# En SalesSuite.Web
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.0
dotnet add package Microsoft.AspNetCore.Identity.UI --version 8.0.0
```

### **2. Eliminar Migraciones Anteriores (si existen)**

```bash
# Eliminar la base de datos actual
dotnet ef database drop --project SalesSuite.Infrastructure --startup-project SalesSuite.Web --force

# Eliminar carpeta de migraciones
Remove-Item -Path "c:\Users\PC\Downloads\Examen\SalesSuite.Infrastructure\Data\Migrations" -Recurse -Force
```

### **3. Crear Nueva Migración con Identity**

```bash
dotnet ef migrations add AddIdentity --project SalesSuite.Infrastructure --startup-project SalesSuite.Web --context ForumDbContext --output-dir Data/Migrations
```

### **4. Aplicar Migración**

```bash
dotnet ef database update --project SalesSuite.Infrastructure --startup-project SalesSuite.Web
```

O simplemente ejecuta la aplicación (se aplicará automáticamente):

```bash
dotnet run --project SalesSuite.Web
```

---

## 🎯 Crear Roles y Usuario Administrador

Crea un nuevo archivo: `SalesSuite.Infrastructure/Seeding/IdentitySeeder.cs`

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SalesSuite.Domain.Entities;

namespace SalesSuite.Infrastructure.Seeding;

public class IdentitySeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<IdentitySeeder> _logger;

    public IdentitySeeder(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<IdentitySeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Crear roles
            await CreateRolesAsync();

            // Crear usuario administrador
            await CreateAdminUserAsync();

            _logger.LogInformation("Identity seeding completado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante Identity seeding");
            throw;
        }
    }

    private async Task CreateRolesAsync()
    {
        string[] roles = { "Administrador", "Moderador", "Usuario" };

        foreach (var roleName in roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
                _logger.LogInformation("Rol {RoleName} creado", roleName);
            }
        }
    }

    private async Task CreateAdminUserAsync()
    {
        var adminEmail = "admin@forumsales.com";

        var existingUser = await _userManager.FindByEmailAsync(adminEmail);
        if (existingUser != null)
        {
            _logger.LogInformation("Usuario administrador ya existe");
            return;
        }

        var adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            NombreCompleto = "Administrador del Sistema",
            Biografia = "Administrador principal del foro",
            FechaRegistro = DateTime.UtcNow,
            Activo = true,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(adminUser, "Admin123!");

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(adminUser, "Administrador");
            _logger.LogInformation("Usuario administrador creado: {Email}", adminEmail);
        }
        else
        {
            _logger.LogError("Error al crear usuario administrador: {Errors}", 
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
```

---

Actualiza `Program.cs` para llamar al IdentitySeeder:

```csharp
// Dentro de InitializeDatabaseAsync, después del DataSeeder:

// Ejecutar Identity seeding
logger.LogInformation("Ejecutando Identity seeding...");
var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
var identitySeeder = new IdentitySeeder(userManager, roleManager, 
    services.GetRequiredService<ILogger<IdentitySeeder>>());
await identitySeeder.SeedAsync();
logger.LogInformation("Identity seeding completado exitosamente.");
```

---

## ✅ Verificación

Después de ejecutar la aplicación:

1. **Navega a**: `https://localhost:5001/Account/Register`
2. **Registra un usuario de prueba**
3. **Verifica que puedas**:
   - Iniciar sesión
   - Acceder a `/Temas/Create` (requiere autenticación)
   - Ver `/Temas` sin autenticación
   - Cerrar sesión

### **Credenciales del Administrador**
- Email: `admin@forumsales.com`
- Contraseña: `Admin123!`

---

## 🎉 Resultado Final

Ahora tienes:

- ✅ ASP.NET Core Identity configurado
- ✅ Login/Register/Logout funcionales
- ✅ Controladores protegidos con `[Authorize]`
- ✅ Rutas públicas accesibles
- ✅ Usuario administrador por defecto
- ✅ Sistema de roles (Administrador, Moderador, Usuario)
- ✅ Autenticación basada en cookies
- ✅ Vistas con Bootstrap 5
- ✅ Validación cliente y servidor

¡Tu foro está completamente seguro y funcional!
