using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesSuite.Domain.Entities;
using SalesSuite.Infrastructure.Data;

namespace SalesSuite.Infrastructure.Seeding;

/// <summary>
/// Clase encargada de poblar datos iniciales en la base de datos del foro
/// </summary>
public class DataSeeder
{
    private readonly ForumDbContext _context;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(ForumDbContext context, ILogger<DataSeeder> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Ejecuta el proceso de seeding de datos iniciales
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Iniciando proceso de seeding de datos...");

            // Verificar si ya existen datos
            if (await _context.Usuarios.AnyAsync())
            {
                _logger.LogInformation("La base de datos ya contiene datos. Seeding omitido.");
                return;
            }

            // Seed en orden de dependencias
            await SeedUsuariosAsync();
            await SeedCategoriasAsync();
            await SeedTemasAsync();
            await SeedMensajesAsync();

            _logger.LogInformation("Proceso de seeding completado exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el proceso de seeding: {Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Inserta usuarios de ejemplo
    /// </summary>
    private async Task SeedUsuariosAsync()
    {
        _logger.LogInformation("Insertando usuarios de ejemplo...");

        var usuarios = new List<Usuario>
        {
            new Usuario
            {
                NombreUsuario = "admin",
                Email = "admin@forumsales.com",
                NombreCompleto = "Administrador del Sistema",
                PasswordHash = "AQAAAAIAAYagAAAAEJ8Z8X9qYvK3hZJ5xKxJ5Q==",
                Biografia = "Administrador principal del foro de SalesSuite",
                Rol = "Administrador",
                Activo = true,
                FechaRegistro = DateTime.UtcNow,
                UltimoAcceso = DateTime.UtcNow
            },
            new Usuario
            {
                NombreUsuario = "moderador",
                Email = "moderador@forumsales.com",
                NombreCompleto = "Moderador del Foro",
                PasswordHash = "AQAAAAIAAYagAAAAEJ8Z8X9qYvK3hZJ5xKxJ5Q==",
                Biografia = "Moderador encargado de mantener el orden en el foro",
                Rol = "Moderador",
                Activo = true,
                FechaRegistro = DateTime.UtcNow,
                UltimoAcceso = DateTime.UtcNow
            },
            new Usuario
            {
                NombreUsuario = "juanperez",
                Email = "juan.perez@example.com",
                NombreCompleto = "Juan Perez",
                PasswordHash = "AQAAAAIAAYagAAAAEJ8Z8X9qYvK3hZJ5xKxJ5Q==",
                Biografia = "Desarrollador .NET apasionado por la programacion",
                Rol = "Usuario",
                Activo = true,
                FechaRegistro = DateTime.UtcNow.AddDays(-30),
                UltimoAcceso = DateTime.UtcNow.AddHours(-2)
            },
            new Usuario
            {
                NombreUsuario = "mariagomez",
                Email = "maria.gomez@example.com",
                NombreCompleto = "Maria Gomez",
                PasswordHash = "AQAAAAIAAYagAAAAEJ8Z8X9qYvK3hZJ5xKxJ5Q==",
                Biografia = "Arquitecta de software especializada en Clean Architecture",
                Rol = "Usuario",
                Activo = true,
                FechaRegistro = DateTime.UtcNow.AddDays(-25),
                UltimoAcceso = DateTime.UtcNow.AddHours(-5)
            },
            new Usuario
            {
                NombreUsuario = "carlosrodriguez",
                Email = "carlos.rodriguez@example.com",
                NombreCompleto = "Carlos Rodriguez",
                PasswordHash = "AQAAAAIAAYagAAAAEJ8Z8X9qYvK3hZJ5xKxJ5Q==",
                Biografia = "Estudiante de ingenieria de software",
                Rol = "Usuario",
                Activo = true,
                FechaRegistro = DateTime.UtcNow.AddDays(-15),
                UltimoAcceso = DateTime.UtcNow.AddDays(-1)
            }
        };

        await _context.Usuarios.AddRangeAsync(usuarios);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Insertados {Count} usuarios de ejemplo.", usuarios.Count);
    }

    /// <summary>
    /// Inserta categorias del foro
    /// </summary>
    private async Task SeedCategoriasAsync()
    {
        _logger.LogInformation("Insertando categorias del foro...");

        var categorias = new List<Categoria>
        {
            new Categoria
            {
                Nombre = "Anuncios",
                Descripcion = "Anuncios oficiales y noticias importantes del foro",
                Slug = "anuncios",
                Icono = "📢",
                Orden = 1,
                Activa = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Categoria
            {
                Nombre = "General",
                Descripcion = "Discusiones generales sobre diversos temas",
                Slug = "general",
                Icono = "💬",
                Orden = 2,
                Activa = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Categoria
            {
                Nombre = "Programacion",
                Descripcion = "Discusiones sobre desarrollo de software, lenguajes y frameworks",
                Slug = "programacion",
                Icono = "💻",
                Orden = 3,
                Activa = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Categoria
            {
                Nombre = ".NET y C#",
                Descripcion = "Todo sobre el ecosistema .NET, C#, ASP.NET Core y mas",
                Slug = "dotnet-csharp",
                Icono = "🔷",
                Orden = 4,
                Activa = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Categoria
            {
                Nombre = "Bases de Datos",
                Descripcion = "SQL Server, PostgreSQL, MongoDB y otros sistemas de bases de datos",
                Slug = "bases-datos",
                Icono = "🗄️",
                Orden = 5,
                Activa = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Categoria
            {
                Nombre = "Arquitectura de Software",
                Descripcion = "Patrones de diseno, Clean Architecture, DDD y mejores practicas",
                Slug = "arquitectura-software",
                Icono = "🏗️",
                Orden = 6,
                Activa = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Categoria
            {
                Nombre = "Ayuda y Soporte",
                Descripcion = "Necesitas ayuda? Pregunta aqui y la comunidad te ayudara",
                Slug = "ayuda-soporte",
                Icono = "🆘",
                Orden = 7,
                Activa = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Categoria
            {
                Nombre = "Off-Topic",
                Descripcion = "Temas fuera del ambito tecnico, charlas casuales",
                Slug = "off-topic",
                Icono = "🎲",
                Orden = 8,
                Activa = true,
                FechaCreacion = DateTime.UtcNow
            }
        };

        await _context.Categorias.AddRangeAsync(categorias);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Insertadas {Count} categorias del foro.", categorias.Count);
    }

    /// <summary>
    /// Inserta temas de ejemplo
    /// </summary>
    private async Task SeedTemasAsync()
    {
        _logger.LogInformation("Insertando temas de ejemplo...");

        var adminId = await _context.Usuarios.Where(u => u.NombreUsuario == "admin").Select(u => u.Id).FirstAsync();
        var juanId = await _context.Usuarios.Where(u => u.NombreUsuario == "juanperez").Select(u => u.Id).FirstAsync();
        var mariaId = await _context.Usuarios.Where(u => u.NombreUsuario == "mariagomez").Select(u => u.Id).FirstAsync();
        var carlosId = await _context.Usuarios.Where(u => u.NombreUsuario == "carlosrodriguez").Select(u => u.Id).FirstAsync();

        var anunciosId = await _context.Categorias.Where(c => c.Slug == "anuncios").Select(c => c.Id).FirstAsync();
        var generalId = await _context.Categorias.Where(c => c.Slug == "general").Select(c => c.Id).FirstAsync();
        var dotnetId = await _context.Categorias.Where(c => c.Slug == "dotnet-csharp").Select(c => c.Id).FirstAsync();
        var arquitecturaId = await _context.Categorias.Where(c => c.Slug == "arquitectura-software").Select(c => c.Id).FirstAsync();
        var ayudaId = await _context.Categorias.Where(c => c.Slug == "ayuda-soporte").Select(c => c.Id).FirstAsync();

        var temas = new List<Tema>
        {
            new Tema
            {
                Titulo = "Bienvenidos al Foro de SalesSuite",
                Contenido = "Bienvenidos a nuestro foro oficial. Este es un espacio para compartir conocimientos, resolver dudas y conectar con otros desarrolladores.",
                Slug = "bienvenidos-foro-salessuite",
                CategoriaId = anunciosId,
                UsuarioId = adminId,
                Fijado = true,
                Cerrado = false,
                NumeroVistas = 150,
                FechaCreacion = DateTime.UtcNow.AddDays(-30),
                FechaUltimaActividad = DateTime.UtcNow.AddDays(-1)
            },
            new Tema
            {
                Titulo = "Normas y Reglas del Foro",
                Contenido = "Por favor, lee y respeta las siguientes normas: 1. Se respetuoso con los demas 2. No spam 3. Usa las categorias apropiadas 4. Busca antes de preguntar",
                Slug = "normas-reglas-foro",
                CategoriaId = anunciosId,
                UsuarioId = adminId,
                Fijado = true,
                Cerrado = false,
                NumeroVistas = 120,
                FechaCreacion = DateTime.UtcNow.AddDays(-30),
                FechaUltimaActividad = DateTime.UtcNow.AddDays(-2)
            },
            new Tema
            {
                Titulo = "Como implementar Clean Architecture en .NET 8",
                Contenido = "Estoy iniciando un nuevo proyecto y quiero aplicar Clean Architecture correctamente. Alguien tiene experiencia con esto?",
                Slug = "como-implementar-clean-architecture-net8",
                CategoriaId = arquitecturaId,
                UsuarioId = juanId,
                Fijado = false,
                Cerrado = false,
                NumeroVistas = 85,
                FechaCreacion = DateTime.UtcNow.AddDays(-10),
                FechaUltimaActividad = DateTime.UtcNow.AddHours(-3)
            },
            new Tema
            {
                Titulo = "Mejores practicas con Entity Framework Core 8",
                Contenido = "Quiero conocer las mejores practicas para trabajar con EF Core 8. Que patrones recomiendan?",
                Slug = "mejores-practicas-ef-core-8",
                CategoriaId = dotnetId,
                UsuarioId = mariaId,
                Fijado = false,
                Cerrado = false,
                NumeroVistas = 92,
                FechaCreacion = DateTime.UtcNow.AddDays(-8),
                FechaUltimaActividad = DateTime.UtcNow.AddHours(-6)
            },
            new Tema
            {
                Titulo = "Ayuda con migraciones de EF Core",
                Contenido = "Tengo problemas al ejecutar migraciones en mi proyecto. Alguien puede ayudarme?",
                Slug = "ayuda-migraciones-ef-core",
                CategoriaId = ayudaId,
                UsuarioId = carlosId,
                Fijado = false,
                Cerrado = false,
                NumeroVistas = 45,
                FechaCreacion = DateTime.UtcNow.AddDays(-3),
                FechaUltimaActividad = DateTime.UtcNow.AddHours(-12)
            },
            new Tema
            {
                Titulo = "Presentate aqui",
                Contenido = "Nuevo en el foro? Cuentanos quien eres y a que te dedicas!",
                Slug = "presentate-aqui",
                CategoriaId = generalId,
                UsuarioId = adminId,
                Fijado = false,
                Cerrado = false,
                NumeroVistas = 200,
                FechaCreacion = DateTime.UtcNow.AddDays(-25),
                FechaUltimaActividad = DateTime.UtcNow.AddHours(-1)
            }
        };

        await _context.Temas.AddRangeAsync(temas);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Insertados {Count} temas de ejemplo.", temas.Count);
    }

    /// <summary>
    /// Inserta mensajes de ejemplo
    /// </summary>
    private async Task SeedMensajesAsync()
    {
        _logger.LogInformation("Insertando mensajes de ejemplo...");

        var adminId = await _context.Usuarios.Where(u => u.NombreUsuario == "admin").Select(u => u.Id).FirstAsync();
        var juanId = await _context.Usuarios.Where(u => u.NombreUsuario == "juanperez").Select(u => u.Id).FirstAsync();
        var mariaId = await _context.Usuarios.Where(u => u.NombreUsuario == "mariagomez").Select(u => u.Id).FirstAsync();
        var carlosId = await _context.Usuarios.Where(u => u.NombreUsuario == "carlosrodriguez").Select(u => u.Id).FirstAsync();

        var temaBienvenida = await _context.Temas.Where(t => t.Slug == "bienvenidos-foro-salessuite").Select(t => t.Id).FirstAsync();
        var temaCleanArch = await _context.Temas.Where(t => t.Slug == "como-implementar-clean-architecture-net8").Select(t => t.Id).FirstAsync();
        var temaEFCore = await _context.Temas.Where(t => t.Slug == "mejores-practicas-ef-core-8").Select(t => t.Id).FirstAsync();
        var temaAyuda = await _context.Temas.Where(t => t.Slug == "ayuda-migraciones-ef-core").Select(t => t.Id).FirstAsync();
        var temaPresentacion = await _context.Temas.Where(t => t.Slug == "presentate-aqui").Select(t => t.Id).FirstAsync();

        var mensajes = new List<Mensaje>
        {
            new Mensaje
            {
                TemaId = temaBienvenida,
                UsuarioId = juanId,
                Contenido = "Gracias por la bienvenida! Estoy emocionado de formar parte de esta comunidad.",
                FechaCreacion = DateTime.UtcNow.AddDays(-29),
                Editado = false,
                NumeroMeGusta = 5,
                Oculto = false
            },
            new Mensaje
            {
                TemaId = temaBienvenida,
                UsuarioId = mariaId,
                Contenido = "Excelente iniciativa! Espero aprender mucho aqui.",
                FechaCreacion = DateTime.UtcNow.AddDays(-28),
                Editado = false,
                NumeroMeGusta = 3,
                Oculto = false
            },
            new Mensaje
            {
                TemaId = temaCleanArch,
                UsuarioId = mariaId,
                Contenido = "Te recomiendo empezar separando tu proyecto en capas: Domain, Application, Infrastructure y Presentation. El Domain debe ser el centro y no depender de nada.",
                FechaCreacion = DateTime.UtcNow.AddDays(-10).AddHours(2),
                Editado = false,
                NumeroMeGusta = 12,
                Oculto = false
            },
            new Mensaje
            {
                TemaId = temaCleanArch,
                UsuarioId = adminId,
                Contenido = "Exacto! Y no olvides usar inyeccion de dependencias y el patron Repository para abstraer el acceso a datos.",
                FechaCreacion = DateTime.UtcNow.AddDays(-9),
                Editado = false,
                NumeroMeGusta = 8,
                Oculto = false
            },
            new Mensaje
            {
                TemaId = temaCleanArch,
                UsuarioId = juanId,
                Contenido = "Muchas gracias por los consejos! Voy a implementarlo asi.",
                FechaCreacion = DateTime.UtcNow.AddHours(-3),
                Editado = false,
                NumeroMeGusta = 2,
                Oculto = false
            },
            new Mensaje
            {
                TemaId = temaEFCore,
                UsuarioId = juanId,
                Contenido = "Yo siempre uso el patron Unit of Work junto con Repository. Funciona muy bien!",
                FechaCreacion = DateTime.UtcNow.AddDays(-8).AddHours(3),
                Editado = false,
                NumeroMeGusta = 7,
                Oculto = false
            },
            new Mensaje
            {
                TemaId = temaEFCore,
                UsuarioId = adminId,
                Contenido = "Tambien recomiendo usar AsNoTracking() para consultas de solo lectura y evitar el tracking innecesario.",
                FechaCreacion = DateTime.UtcNow.AddDays(-7),
                Editado = false,
                NumeroMeGusta = 10,
                Oculto = false
            },
            new Mensaje
            {
                TemaId = temaAyuda,
                UsuarioId = mariaId,
                Contenido = "Que error especifico te aparece? Comparte el mensaje de error completo para poder ayudarte mejor.",
                FechaCreacion = DateTime.UtcNow.AddDays(-3).AddHours(2),
                Editado = false,
                NumeroMeGusta = 3,
                Oculto = false
            },
            new Mensaje
            {
                TemaId = temaAyuda,
                UsuarioId = carlosId,
                Contenido = "El error dice: The migration has already been applied to the database. Que debo hacer?",
                FechaCreacion = DateTime.UtcNow.AddDays(-3).AddHours(4),
                Editado = false,
                NumeroMeGusta = 0,
                Oculto = false
            },
            new Mensaje
            {
                TemaId = temaAyuda,
                UsuarioId = adminId,
                Contenido = "Necesitas revertir la migracion con: dotnet ef database update NombreMigracionAnterior. Luego puedes eliminar la migracion problematica y crear una nueva.",
                FechaCreacion = DateTime.UtcNow.AddHours(-12),
                Editado = false,
                NumeroMeGusta = 5,
                Oculto = false
            },
            new Mensaje
            {
                TemaId = temaPresentacion,
                UsuarioId = juanId,
                Contenido = "Hola! Soy Juan, desarrollador .NET con 5 anos de experiencia. Me especializo en ASP.NET Core y microservicios.",
                FechaCreacion = DateTime.UtcNow.AddDays(-24),
                Editado = false,
                NumeroMeGusta = 8,
                Oculto = false
            },
            new Mensaje
            {
                TemaId = temaPresentacion,
                UsuarioId = mariaId,
                Contenido = "Hola a todos! Soy Maria, arquitecta de software. Me apasiona el diseno de sistemas escalables y mantenibles.",
                FechaCreacion = DateTime.UtcNow.AddDays(-23),
                Editado = false,
                NumeroMeGusta = 6,
                Oculto = false
            },
            new Mensaje
            {
                TemaId = temaPresentacion,
                UsuarioId = carlosId,
                Contenido = "Que tal! Soy Carlos, estudiante de ingenieria. Estoy aprendiendo .NET y quiero mejorar mis habilidades.",
                FechaCreacion = DateTime.UtcNow.AddHours(-1),
                Editado = false,
                NumeroMeGusta = 4,
                Oculto = false
            }
        };

        await _context.Mensajes.AddRangeAsync(mensajes);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Insertados {Count} mensajes de ejemplo.", mensajes.Count);
    }
}
