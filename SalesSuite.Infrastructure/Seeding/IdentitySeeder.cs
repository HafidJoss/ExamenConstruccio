using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SalesSuite.Domain.Entities;

namespace SalesSuite.Infrastructure.Seeding;

/// <summary>
/// Seeder para crear roles y usuario administrador de Identity
/// </summary>
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
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Ejecuta el seeding de roles y usuario administrador
    /// </summary>
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
            _logger.LogError(ex, "Error durante el Identity seeding");
            throw;
        }
    }

    /// <summary>
    /// Crea los roles del sistema
    /// </summary>
    private async Task CreateRolesAsync()
    {
        string[] roles = { "Administrador", "Moderador", "Usuario" };

        foreach (var roleName in roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole(roleName);
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Rol {RoleName} creado exitosamente", roleName);
                }
                else
                {
                    _logger.LogError("Error al crear rol {RoleName}: {Errors}", 
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                _logger.LogInformation("Rol {RoleName} ya existe", roleName);
            }
        }
    }

    /// <summary>
    /// Crea el usuario administrador por defecto
    /// </summary>
    private async Task CreateAdminUserAsync()
    {
        const string adminEmail = "admin@forumsales.com";
        const string adminPassword = "Admin123!";

        var existingUser = await _userManager.FindByEmailAsync(adminEmail);
        if (existingUser != null)
        {
            _logger.LogInformation("Usuario administrador ya existe: {Email}", adminEmail);
            return;
        }

        var adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            NombreCompleto = "Administrador del Sistema",
            Biografia = "Usuario administrador del foro SalesSuite",
            FechaRegistro = DateTime.UtcNow,
            Activo = true,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(adminUser, adminPassword);

        if (result.Succeeded)
        {
            _logger.LogInformation("Usuario administrador creado: {Email}", adminEmail);

            // Asignar rol de Administrador
            var roleResult = await _userManager.AddToRoleAsync(adminUser, "Administrador");
            if (roleResult.Succeeded)
            {
                _logger.LogInformation("Rol Administrador asignado a {Email}", adminEmail);
            }
            else
            {
                _logger.LogError("Error al asignar rol Administrador: {Errors}", 
                    string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            _logger.LogError("Error al crear usuario administrador: {Errors}", 
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
