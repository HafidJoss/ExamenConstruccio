using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SalesSuite.Application;
using SalesSuite.Domain.Entities;
using SalesSuite.Infrastructure;
using SalesSuite.Infrastructure.Data;
using SalesSuite.Infrastructure.Seeding;
using SalesSuite.Web.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();

// Configurar la capa de infraestructura (DbContext, Repositories, UnitOfWork)
builder.Services.AddInfrastructure(builder.Configuration);

// Configurar la capa de aplicación (Casos de uso)
builder.Services.AddApplication();

// Configurar ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configuración de contraseñas
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // Configuración de usuario
    options.User.RequireUniqueEmail = true;

    // Configuración de inicio de sesión
    options.SignIn.RequireConfirmedEmail = false; // Para desarrollo
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ForumDbContext>()
.AddDefaultTokenProviders();

// Configurar cookies de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Configurar AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configurar logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Ejecutar migraciones y seeding automáticamente
await InitializeDatabaseAsync(app);

async Task InitializeDatabaseAsync(WebApplication application)
{
    using var scope = application.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Iniciando inicialización de base de datos...");

        // Obtener el contexto de base de datos
        var context = services.GetRequiredService<ForumDbContext>();

        // Ejecutar migraciones pendientes
        logger.LogInformation("Aplicando migraciones pendientes...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Migraciones aplicadas exitosamente.");

        // Ejecutar seeding de datos
        logger.LogInformation("Ejecutando seeding de datos...");
        var seeder = services.GetRequiredService<DataSeeder>();
        await seeder.SeedAsync();
        logger.LogInformation("Seeding completado exitosamente.");

        // Ejecutar Identity seeding
        logger.LogInformation("Ejecutando Identity seeding...");
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var identitySeeder = new IdentitySeeder(userManager, roleManager, 
            services.GetRequiredService<ILogger<IdentitySeeder>>());
        await identitySeeder.SeedAsync();
        logger.LogInformation("Identity seeding completado exitosamente.");

        logger.LogInformation("Inicialización de base de datos completada.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error durante la inicialización de la base de datos: {Message}", ex.Message);
        throw;
    }
}

// Configurar el pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Habilitar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
