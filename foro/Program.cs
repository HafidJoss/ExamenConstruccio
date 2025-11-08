using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using foro.Infrastructure.Data;
using foro.Domain.Interfaces;
using foro.Infrastructure.Repositories;
using foro.Application.Services;
using foro.Application.Interfaces;
using foro.Application.UseCases.Temas;
using foro.Infrastructure.Seeding;
using foro.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar DbContext con SQL Server
builder.Services.AddDbContext<ForumDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ForumConnection")));

// ========================================
// CONFIGURAR ASP.NET CORE IDENTITY
// ========================================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configuración de contraseñas
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
  options.Password.RequiredUniqueChars = 1;

    // Configuración de bloqueo de cuenta
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Configuración de usuario
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false; // Cambiar a true en producción
})
.AddEntityFrameworkStores<ForumDbContext>()
.AddDefaultTokenProviders();

// Configurar opciones de cookies de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Registrar Unit of Work y Repositorios
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Registro de servicios
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ForoService>();

// Registro de casos de uso
builder.Services.AddScoped<ICrearTemaUseCase, CrearTemaUseCase>();

// Registrar DataSeeder
builder.Services.AddScoped<DataSeeder>();

// Configurar AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

var app = builder.Build();

// ========================================
// MIGRACIÓN, SEEDING Y ROLES
// ========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Iniciando migración y seeding de base de datos...");
  
      // Obtener servicios necesarios
     var context = services.GetRequiredService<ForumDbContext>();
 var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
  // Ejecutar migraciones pendientes
   logger.LogInformation("Aplicando migraciones pendientes...");
 await context.Database.MigrateAsync();
        logger.LogInformation("Migraciones aplicadas exitosamente.");
     
        // Crear roles si no existen
        await CreateRolesAsync(roleManager, logger);
 
     // Crear usuario administrador por defecto
     await CreateAdminUserAsync(userManager, logger);
        
     // Ejecutar seeding de datos del foro
        var seeder = services.GetRequiredService<DataSeeder>();
        await seeder.SeedAsync();
        
      logger.LogInformation("Base de datos inicializada correctamente.");
 }
    catch (Exception ex)
    {
logger.LogError(ex, "Error al inicializar la base de datos");
        if (app.Environment.IsDevelopment())
  {
throw;
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ? IMPORTANTE: Autenticación y Autorización
app.UseAuthentication(); // Debe ir antes de UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// ========================================
// MÉTODOS AUXILIARES
// ========================================

/// <summary>
/// Crea los roles necesarios para la aplicación
/// </summary>
static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
{
    string[] roleNames = { "Administrador", "Moderador", "Usuario" };

    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
  var result = await roleManager.CreateAsync(new IdentityRole(roleName));
 if (result.Succeeded)
            {
                logger.LogInformation("Rol '{RoleName}' creado exitosamente.", roleName);
         }
        }
    }
}

/// <summary>
/// Crea el usuario administrador por defecto si no existe
/// </summary>
static async Task CreateAdminUserAsync(UserManager<ApplicationUser> userManager, ILogger logger)
{
    var adminEmail = "admin@foro.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        var admin = new ApplicationUser
   {
     UserName = adminEmail,
   Email = adminEmail,
      NombreCompleto = "Administrador del Foro",
            Biografia = "Usuario administrador del sistema",
  FechaRegistro = DateTime.UtcNow,
            Activo = true,
         EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, "Admin@123");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Administrador");
   logger.LogInformation("Usuario administrador creado exitosamente: {Email}", adminEmail);
        }
        else
        {
       logger.LogError("Error al crear usuario administrador: {Errors}", 
        string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
