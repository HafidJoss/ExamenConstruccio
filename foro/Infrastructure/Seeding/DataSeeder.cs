using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using foro.Domain.Entities;
using foro.Infrastructure.Data;

namespace foro.Infrastructure.Seeding;

/// <summary>
/// Seeder para poblar datos iniciales del foro en la base de datos
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
    /// Método principal para ejecutar el seeding de datos
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
      _logger.LogError(ex, "Error durante el proceso de seeding de datos");
         throw;
        }
    }

    #region Seed Methods

    /// <summary>
    /// Inserta usuarios de ejemplo
    /// </summary>
    private async Task SeedUsuariosAsync()
    {
        _logger.LogInformation("Insertando usuarios de prueba...");

        var usuarios = new List<Usuario>
        {
 new Usuario
            {
      Nombre = "Administrador",
    Email = "admin@foro.com",
          Biografia = "Administrador del foro",
      FechaRegistro = DateTime.UtcNow,
        Activo = true
     },
        new Usuario
      {
     Nombre = "Juan Pérez",
Email = "juan.perez@mail.com",
   Biografia = "Desarrollador .NET con 5 años de experiencia",
  FechaRegistro = DateTime.UtcNow,
       Activo = true
   },
  new Usuario
    {
     Nombre = "María García",
         Email = "maria.garcia@mail.com",
         Biografia = "Especialista en bases de datos",
            FechaRegistro = DateTime.UtcNow,
     Activo = true
     },
   new Usuario
            {
      Nombre = "Carlos López",
  Email = "carlos.lopez@mail.com",
     Biografia = "Apasionado por el desarrollo web",
      FechaRegistro = DateTime.UtcNow,
Activo = true
 },
      new Usuario
            {
                Nombre = "Ana Martínez",
         Email = "ana.martinez@mail.com",
  Biografia = "Front-end developer",
 FechaRegistro = DateTime.UtcNow,
       Activo = true
            }
        };

        await _context.Usuarios.AddRangeAsync(usuarios);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Usuarios insertados: {Count}", usuarios.Count);
    }

    /// <summary>
    /// Inserta categorías del foro
    /// </summary>
    private async Task SeedCategoriasAsync()
    {
    _logger.LogInformation("Insertando categorías...");

        var categorias = new List<Categoria>
        {
         new Categoria
            {
    Nombre = ".NET Framework",
           Descripcion = "Discusiones sobre .NET Framework, C#, VB.NET y tecnologías relacionadas",
         FechaCreacion = DateTime.UtcNow,
                Activa = true,
                Orden = 1
},
       new Categoria
          {
     Nombre = "ASP.NET Core",
 Descripcion = "Temas sobre ASP.NET Core MVC, Razor Pages, Web API",
           FechaCreacion = DateTime.UtcNow,
         Activa = true,
             Orden = 2
            },
       new Categoria
            {
                Nombre = "Entity Framework",
            Descripcion = "Entity Framework Core, LINQ, bases de datos",
     FechaCreacion = DateTime.UtcNow,
      Activa = true,
         Orden = 3
    },
         new Categoria
         {
           Nombre = "Desarrollo Web",
              Descripcion = "HTML, CSS, JavaScript, TypeScript, Angular, React",
    FechaCreacion = DateTime.UtcNow,
        Activa = true,
    Orden = 4
    },
  new Categoria
            {
                Nombre = "Bases de Datos",
     Descripcion = "SQL Server, PostgreSQL, MySQL, MongoDB",
     FechaCreacion = DateTime.UtcNow,
                Activa = true,
             Orden = 5
      },
      new Categoria
       {
  Nombre = "DevOps & Cloud",
  Descripcion = "Azure, AWS, Docker, Kubernetes, CI/CD",
          FechaCreacion = DateTime.UtcNow,
        Activa = true,
      Orden = 6
            },
     new Categoria
    {
      Nombre = "Arquitectura de Software",
   Descripcion = "Clean Architecture, DDD, Microservicios, Patrones de Diseño",
      FechaCreacion = DateTime.UtcNow,
       Activa = true,
                Orden = 7
},
   new Categoria
            {
  Nombre = "General",
 Descripcion = "Temas generales de programación y tecnología",
        FechaCreacion = DateTime.UtcNow,
     Activa = true,
    Orden = 8
}
     };

        await _context.Categorias.AddRangeAsync(categorias);
        await _context.SaveChangesAsync();

   _logger.LogInformation("Categorías insertadas: {Count}", categorias.Count);
    }

    /// <summary>
    /// Inserta temas de ejemplo
    /// </summary>
    private async Task SeedTemasAsync()
    {
_logger.LogInformation("Insertando temas de prueba...");

        // Obtener usuarios y categorías
        var admin = await _context.Usuarios.FirstAsync(u => u.Email == "admin@foro.com");
        var juan = await _context.Usuarios.FirstAsync(u => u.Email == "juan.perez@mail.com");
        var maria = await _context.Usuarios.FirstAsync(u => u.Email == "maria.garcia@mail.com");
    var carlos = await _context.Usuarios.FirstAsync(u => u.Email == "carlos.lopez@mail.com");
var ana = await _context.Usuarios.FirstAsync(u => u.Email == "ana.martinez@mail.com");

        var catGeneral = await _context.Categorias.FirstAsync(c => c.Nombre == "General");
    var catDotNet = await _context.Categorias.FirstAsync(c => c.Nombre == ".NET Framework");
var catAspNet = await _context.Categorias.FirstAsync(c => c.Nombre == "ASP.NET Core");
        var catEF = await _context.Categorias.FirstAsync(c => c.Nombre == "Entity Framework");
        var catWeb = await _context.Categorias.FirstAsync(c => c.Nombre == "Desarrollo Web");
        var catDB = await _context.Categorias.FirstAsync(c => c.Nombre == "Bases de Datos");
        var catDevOps = await _context.Categorias.FirstAsync(c => c.Nombre == "DevOps & Cloud");
        var catArq = await _context.Categorias.FirstAsync(c => c.Nombre == "Arquitectura de Software");

      var temas = new List<Tema>
        {
         // Temas fijados
            new Tema
    {
    Titulo = "Bienvenidos al Foro de Desarrollo .NET",
             Contenido = "Este es el foro oficial para discutir sobre tecnologías .NET, compartir conocimientos y resolver dudas. ¡Esperamos que sea de gran ayuda para todos!",
       FechaCreacion = DateTime.UtcNow,
     FechaUltimaActividad = DateTime.UtcNow,
    Cerrado = false,
 Fijado = true,
        Vistas = 250,
       UsuarioId = admin.Id,
          CategoriaId = catGeneral.Id
          },
        new Tema
            {
        Titulo = "Normas y Reglas del Foro",
    Contenido = "Por favor, lee estas normas antes de publicar. Mantengamos un ambiente respetuoso y colaborativo.",
     FechaCreacion = DateTime.UtcNow,
      FechaUltimaActividad = DateTime.UtcNow,
    Cerrado = false,
     Fijado = true,
       Vistas = 180,
      UsuarioId = admin.Id,
 CategoriaId = catGeneral.Id
            },
 // Temas sobre .NET
         new Tema
 {
   Titulo = "¿Cuál es la diferencia entre .NET Framework y .NET Core?",
            Contenido = "Estoy empezando con .NET y me gustaría entender las diferencias principales entre .NET Framework y .NET Core. ¿Cuál debería usar para un nuevo proyecto?",
        FechaCreacion = DateTime.UtcNow.AddDays(-15),
         FechaUltimaActividad = DateTime.UtcNow.AddDays(-10),
        Cerrado = false,
       Fijado = false,
           Vistas = 95,
     UsuarioId = juan.Id,
       CategoriaId = catDotNet.Id
     },
   new Tema
   {
   Titulo = "Mejores prácticas para manejo de excepciones en C#",
   Contenido = "Me gustaría conocer las mejores prácticas para el manejo de excepciones en C#. ¿Cuándo usar try-catch y cuándo usar excepciones personalizadas?",
    FechaCreacion = DateTime.UtcNow.AddDays(-12),
         FechaUltimaActividad = DateTime.UtcNow.AddDays(-8),
    Cerrado = false,
            Fijado = false,
          Vistas = 78,
         UsuarioId = maria.Id,
    CategoriaId = catDotNet.Id
        },
            // Temas sobre ASP.NET Core
     new Tema
  {
 Titulo = "Implementación de JWT Authentication en ASP.NET Core 8",
        Contenido = "Necesito implementar autenticación con JWT en mi API de ASP.NET Core 8. ¿Alguien tiene un ejemplo completo con refresh tokens?",
         FechaCreacion = DateTime.UtcNow.AddDays(-8),
     FechaUltimaActividad = DateTime.UtcNow.AddDays(-5),
          Cerrado = false,
          Fijado = false,
                Vistas = 142,
     UsuarioId = carlos.Id,
     CategoriaId = catAspNet.Id
            },
    new Tema
            {
     Titulo = "¿Razor Pages o MVC para nuevo proyecto?",
         Contenido = "Voy a iniciar un nuevo proyecto web con ASP.NET Core 8. ¿Qué recomiendan: Razor Pages o MVC? ¿Cuáles son las ventajas de cada uno?",
      FechaCreacion = DateTime.UtcNow.AddDays(-7),
      FechaUltimaActividad = DateTime.UtcNow.AddDays(-4),
       Cerrado = false,
         Fijado = false,
           Vistas = 105,
            UsuarioId = ana.Id,
             CategoriaId = catAspNet.Id
    },
            new Tema
            {
      Titulo = "Cómo configurar CORS en ASP.NET Core",
     Contenido = "Tengo problemas con CORS en mi API. El frontend no puede hacer peticiones desde otro dominio. ¿Cómo lo configuro correctamente?",
         FechaCreacion = DateTime.UtcNow.AddDays(-6),
       FechaUltimaActividad = DateTime.UtcNow.AddDays(-3),
   Cerrado = false,
     Fijado = false,
              Vistas = 87,
             UsuarioId = juan.Id,
                CategoriaId = catAspNet.Id
},
  // Temas sobre Entity Framework
      new Tema
         {
    Titulo = "Entity Framework Core: Lazy Loading vs Eager Loading",
  Contenido = "¿Cuándo es mejor usar Lazy Loading y cuándo Eager Loading en Entity Framework Core? ¿Cuál es más eficiente?",
            FechaCreacion = DateTime.UtcNow.AddDays(-10),
         FechaUltimaActividad = DateTime.UtcNow.AddDays(-6),
    Cerrado = false,
        Fijado = false,
      Vistas = 156,
      UsuarioId = maria.Id,
    CategoriaId = catEF.Id
     },
       new Tema
            {
      Titulo = "Code First vs Database First en EF Core",
     Contenido = "Para un nuevo proyecto, ¿qué enfoque recomiendan: Code First o Database First? ¿Cuáles son las ventajas de cada uno?",
          FechaCreacion = DateTime.UtcNow.AddDays(-5),
         FechaUltimaActividad = DateTime.UtcNow.AddDays(-2),
      Cerrado = false,
     Fijado = false,
Vistas = 92,
         UsuarioId = carlos.Id,
           CategoriaId = catEF.Id
          },
            new Tema
            {
           Titulo = "Optimización de queries con Include y ThenInclude",
   Contenido = "Mis queries están muy lentas cuando cargo entidades relacionadas. ¿Cómo optimizar el uso de Include y ThenInclude?",
         FechaCreacion = DateTime.UtcNow.AddDays(-4),
           FechaUltimaActividad = DateTime.UtcNow.AddDays(-1),
                Cerrado = false,
  Fijado = false,
    Vistas = 68,
    UsuarioId = ana.Id,
        CategoriaId = catEF.Id
  },
   // Temas sobre Desarrollo Web
  new Tema
            {
                Titulo = "Bootstrap 5 vs Tailwind CSS: ¿Cuál elegir?",
      Contenido = "Estoy iniciando un nuevo proyecto frontend y debo elegir entre Bootstrap 5 y Tailwind CSS. ¿Qué experiencias tienen con cada uno?",
      FechaCreacion = DateTime.UtcNow.AddDays(-9),
      FechaUltimaActividad = DateTime.UtcNow.AddDays(-7),
     Cerrado = false,
        Fijado = false,
          Vistas = 125,
        UsuarioId = juan.Id,
       CategoriaId = catWeb.Id
    },
            new Tema
            {
 Titulo = "Introducción a TypeScript para desarrolladores C#",
        Contenido = "Vengo del mundo .NET/C# y necesito aprender TypeScript. ¿Qué recursos recomiendan?",
       FechaCreacion = DateTime.UtcNow.AddDays(-3),
      FechaUltimaActividad = DateTime.UtcNow.AddHours(-12),
      Cerrado = false,
         Fijado = false,
        Vistas = 54,
        UsuarioId = maria.Id,
           CategoriaId = catWeb.Id
       },
      // Temas sobre Bases de Datos
new Tema
  {
    Titulo = "Índices en SQL Server: Guía práctica",
 Contenido = "¿Alguien tiene una guía práctica sobre cuándo y cómo crear índices en SQL Server para mejorar el rendimiento?",
         FechaCreacion = DateTime.UtcNow.AddDays(-11),
          FechaUltimaActividad = DateTime.UtcNow.AddDays(-9),
                Cerrado = false,
         Fijado = false,
    Vistas = 134,
        UsuarioId = carlos.Id,
     CategoriaId = catDB.Id
      },
            new Tema
            {
  Titulo = "Migraciones en Entity Framework Core",
            Contenido = "Tengo problemas con las migraciones en EF Core. ¿Cómo manejar cambios en producción de forma segura?",
           FechaCreacion = DateTime.UtcNow.AddDays(-2),
           FechaUltimaActividad = DateTime.UtcNow.AddHours(-6),
          Cerrado = false,
    Fijado = false,
    Vistas = 47,
        UsuarioId = ana.Id,
  CategoriaId = catDB.Id
            },
     // Temas sobre DevOps
            new Tema
            {
   Titulo = "CI/CD con Azure DevOps para aplicaciones .NET",
     Contenido = "Quiero implementar CI/CD para mi aplicación ASP.NET Core. ¿Algún tutorial completo de Azure DevOps?",
    FechaCreacion = DateTime.UtcNow.AddDays(-13),
                FechaUltimaActividad = DateTime.UtcNow.AddDays(-11),
     Cerrado = false,
    Fijado = false,
              Vistas = 167,
 UsuarioId = juan.Id,
     CategoriaId = catDevOps.Id
    },
            new Tema
         {
     Titulo = "Docker para desarrolladores .NET: Primeros pasos",
           Contenido = "Nunca he usado Docker. ¿Por dónde empiezo para containerizar mi aplicación ASP.NET Core?",
          FechaCreacion = DateTime.UtcNow.AddDays(-1),
      FechaUltimaActividad = DateTime.UtcNow.AddHours(-3),
       Cerrado = false,
                Fijado = false,
Vistas = 38,
   UsuarioId = maria.Id,
         CategoriaId = catDevOps.Id
        },
      // Temas sobre Arquitectura
    new Tema
    {
      Titulo = "Clean Architecture en .NET: Implementación práctica",
         Contenido = "Estoy aprendiendo Clean Architecture. ¿Alguien tiene un ejemplo de proyecto completo en .NET?",
       FechaCreacion = DateTime.UtcNow.AddDays(-14),
 FechaUltimaActividad = DateTime.UtcNow.AddDays(-12),
      Cerrado = false,
 Fijado = false,
     Vistas = 198,
         UsuarioId = carlos.Id,
    CategoriaId = catArq.Id
            },
            new Tema
    {
                Titulo = "Repository Pattern vs Direct DbContext",
  Contenido = "¿Vale la pena implementar el patrón Repository o es mejor usar DbContext directamente?",
       FechaCreacion = DateTime.UtcNow.AddHours(-18),
       FechaUltimaActividad = DateTime.UtcNow.AddHours(-2),
     Cerrado = false,
                Fijado = false,
             Vistas = 23,
          UsuarioId = ana.Id,
                CategoriaId = catArq.Id
   },
      new Tema
            {
     Titulo = "CQRS y MediatR en ASP.NET Core",
                Contenido = "Me gustaría implementar CQRS con MediatR en mi proyecto. ¿Alguien tiene experiencia con esto?",
      FechaCreacion = DateTime.UtcNow.AddHours(-8),
     FechaUltimaActividad = DateTime.UtcNow.AddHours(-1),
       Cerrado = false,
    Fijado = false,
     Vistas = 15,
      UsuarioId = juan.Id,
    CategoriaId = catArq.Id
        }
};

        await _context.Temas.AddRangeAsync(temas);
   await _context.SaveChangesAsync();

        _logger.LogInformation("Temas insertados: {Count}", temas.Count);
    }

    /// <summary>
    /// Inserta mensajes de ejemplo
    /// </summary>
    private async Task SeedMensajesAsync()
    {
        _logger.LogInformation("Insertando mensajes de prueba...");

        // Obtener usuarios
        var juan = await _context.Usuarios.FirstAsync(u => u.Email == "juan.perez@mail.com");
        var maria = await _context.Usuarios.FirstAsync(u => u.Email == "maria.garcia@mail.com");
  var carlos = await _context.Usuarios.FirstAsync(u => u.Email == "carlos.lopez@mail.com");

 // Obtener temas
        var temaBienvenida = await _context.Temas.FirstAsync(t => t.Titulo == "Bienvenidos al Foro de Desarrollo .NET");
 var temaJWT = await _context.Temas.FirstAsync(t => t.Titulo.Contains("JWT Authentication"));

    var mensajes = new List<Mensaje>
        {
// Mensajes para "Bienvenidos"
            new Mensaje
      {
          Contenido = "¡Gracias por crear este espacio! Espero aprender mucho aquí.",
        FechaCreacion = DateTime.UtcNow.AddMinutes(-30),
      Editado = false,
    TemaId = temaBienvenida.Id,
      UsuarioId = juan.Id
      },
            new Mensaje
         {
    Contenido = "Excelente iniciativa. ¡A compartir conocimientos!",
FechaCreacion = DateTime.UtcNow.AddMinutes(-25),
 Editado = false,
                TemaId = temaBienvenida.Id,
     UsuarioId = maria.Id
    },
  // Mensajes para JWT
  new Mensaje
   {
        Contenido = "Aquí te dejo un ejemplo completo de JWT con refresh tokens...",
         FechaCreacion = DateTime.UtcNow.AddDays(-4),
    Editado = false,
      TemaId = temaJWT.Id,
        UsuarioId = maria.Id
      },
         new Mensaje
    {
         Contenido = "Muchas gracias! Me sirvió mucho tu ejemplo.",
     FechaCreacion = DateTime.UtcNow.AddDays(-3),
  Editado = false,
        TemaId = temaJWT.Id,
 UsuarioId = carlos.Id
            }
        };

        await _context.Mensajes.AddRangeAsync(mensajes);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Mensajes insertados: {Count}", mensajes.Count);
    }

    #endregion
}
