-- ============================================
-- Script de Datos de Prueba para el Foro
-- ============================================

USE ForumDb;
GO

-- ============================================
-- 1. INSERTAR USUARIOS
-- ============================================
PRINT 'Insertando usuarios de prueba...';

INSERT INTO Usuarios (Nombre, Email, Biografia, FechaRegistro, Activo)
VALUES 
  ('Administrador', 'admin@foro.com', 'Administrador del foro', GETUTCDATE(), 1),
 ('Juan Pérez', 'juan.perez@mail.com', 'Desarrollador .NET con 5 años de experiencia', GETUTCDATE(), 1),
    ('María García', 'maria.garcia@mail.com', 'Especialista en bases de datos', GETUTCDATE(), 1),
    ('Carlos López', 'carlos.lopez@mail.com', 'Apasionado por el desarrollo web', GETUTCDATE(), 1),
    ('Ana Martínez', 'ana.martinez@mail.com', 'Front-end developer', GETUTCDATE(), 1);

PRINT 'Usuarios insertados: ' + CAST(@@ROWCOUNT AS VARCHAR);

-- ============================================
-- 2. INSERTAR CATEGORÍAS
-- ============================================
PRINT 'Insertando categorías...';

INSERT INTO Categorias (Nombre, Descripcion, FechaCreacion, Activa, Orden)
VALUES 
    ('.NET Framework', 'Discusiones sobre .NET Framework, C#, VB.NET y tecnologías relacionadas', GETUTCDATE(), 1, 1),
    ('ASP.NET Core', 'Temas sobre ASP.NET Core MVC, Razor Pages, Web API', GETUTCDATE(), 1, 2),
    ('Entity Framework', 'Entity Framework Core, LINQ, bases de datos', GETUTCDATE(), 1, 3),
    ('Desarrollo Web', 'HTML, CSS, JavaScript, TypeScript, Angular, React', GETUTCDATE(), 1, 4),
    ('Bases de Datos', 'SQL Server, PostgreSQL, MySQL, MongoDB', GETUTCDATE(), 1, 5),
    ('DevOps & Cloud', 'Azure, AWS, Docker, Kubernetes, CI/CD', GETUTCDATE(), 1, 6),
    ('Arquitectura de Software', 'Clean Architecture, DDD, Microservicios, Patrones de Diseño', GETUTCDATE(), 1, 7),
    ('General', 'Temas generales de programación y tecnología', GETUTCDATE(), 1, 8);

PRINT 'Categorías insertadas: ' + CAST(@@ROWCOUNT AS VARCHAR);

-- ============================================
-- 3. INSERTAR TEMAS
-- ============================================
PRINT 'Insertando temas de prueba...';

DECLARE @Usuario1 INT = (SELECT Id FROM Usuarios WHERE Email = 'admin@foro.com');
DECLARE @Usuario2 INT = (SELECT Id FROM Usuarios WHERE Email = 'juan.perez@mail.com');
DECLARE @Usuario3 INT = (SELECT Id FROM Usuarios WHERE Email = 'maria.garcia@mail.com');
DECLARE @Usuario4 INT = (SELECT Id FROM Usuarios WHERE Email = 'carlos.lopez@mail.com');
DECLARE @Usuario5 INT = (SELECT Id FROM Usuarios WHERE Email = 'ana.martinez@mail.com');

DECLARE @Cat1 INT = (SELECT Id FROM Categorias WHERE Nombre = '.NET Framework');
DECLARE @Cat2 INT = (SELECT Id FROM Categorias WHERE Nombre = 'ASP.NET Core');
DECLARE @Cat3 INT = (SELECT Id FROM Categorias WHERE Nombre = 'Entity Framework');
DECLARE @Cat4 INT = (SELECT Id FROM Categorias WHERE Nombre = 'Desarrollo Web');
DECLARE @Cat5 INT = (SELECT Id FROM Categorias WHERE Nombre = 'Bases de Datos');
DECLARE @Cat6 INT = (SELECT Id FROM Categorias WHERE Nombre = 'DevOps & Cloud');
DECLARE @Cat7 INT = (SELECT Id FROM Categorias WHERE Nombre = 'Arquitectura de Software');
DECLARE @Cat8 INT = (SELECT Id FROM Categorias WHERE Nombre = 'General');

-- Temas destacados (fijados)
INSERT INTO Temas (Titulo, Contenido, FechaCreacion, FechaUltimaActividad, Cerrado, Fijado, Vistas, UsuarioId, CategoriaId)
VALUES 
    ('Bienvenidos al Foro de Desarrollo .NET', 
     'Este es el foro oficial para discutir sobre tecnologías .NET, compartir conocimientos y resolver dudas. ¡Esperamos que sea de gran ayuda para todos!',
     GETUTCDATE(), GETUTCDATE(), 0, 1, 250, @Usuario1, @Cat8),
    
    ('Normas y Reglas del Foro', 
     'Por favor, lee estas normas antes de publicar. Mantengamos un ambiente respetuoso y colaborativo.',
     GETUTCDATE(), GETUTCDATE(), 0, 1, 180, @Usuario1, @Cat8);

-- Temas sobre .NET
INSERT INTO Temas (Titulo, Contenido, FechaCreacion, FechaUltimaActividad, Cerrado, Fijado, Vistas, UsuarioId, CategoriaId)
VALUES 
    ('¿Cuál es la diferencia entre .NET Framework y .NET Core?', 
'Estoy empezando con .NET y me gustaría entender las diferencias principales entre .NET Framework y .NET Core. ¿Cuál debería usar para un nuevo proyecto?',
     DATEADD(DAY, -15, GETUTCDATE()), DATEADD(DAY, -10, GETUTCDATE()), 0, 0, 95, @Usuario2, @Cat1),
  
    ('Mejores prácticas para manejo de excepciones en C#', 
     'Me gustaría conocer las mejores prácticas para el manejo de excepciones en C#. ¿Cuándo usar try-catch y cuándo usar excepciones personalizadas?',
     DATEADD(DAY, -12, GETUTCDATE()), DATEADD(DAY, -8, GETUTCDATE()), 0, 0, 78, @Usuario3, @Cat1);

-- Temas sobre ASP.NET Core
INSERT INTO Temas (Titulo, Contenido, FechaCreacion, FechaUltimaActividad, Cerrado, Fijado, Vistas, UsuarioId, CategoriaId)
VALUES 
    ('Implementación de JWT Authentication en ASP.NET Core 8', 
     'Necesito implementar autenticación con JWT en mi API de ASP.NET Core 8. ¿Alguien tiene un ejemplo completo con refresh tokens?',
     DATEADD(DAY, -8, GETUTCDATE()), DATEADD(DAY, -5, GETUTCDATE()), 0, 0, 142, @Usuario4, @Cat2),
    
    ('¿Razor Pages o MVC para nuevo proyecto?', 
   'Voy a iniciar un nuevo proyecto web con ASP.NET Core 8. ¿Qué recomiendan: Razor Pages o MVC? ¿Cuáles son las ventajas de cada uno?',
     DATEADD(DAY, -7, GETUTCDATE()), DATEADD(DAY, -4, GETUTCDATE()), 0, 0, 105, @Usuario5, @Cat2),
    
    ('Cómo configurar CORS en ASP.NET Core', 
     'Tengo problemas con CORS en mi API. El frontend no puede hacer peticiones desde otro dominio. ¿Cómo lo configuro correctamente?',
     DATEADD(DAY, -6, GETUTCDATE()), DATEADD(DAY, -3, GETUTCDATE()), 0, 0, 87, @Usuario2, @Cat2);

-- Temas sobre Entity Framework
INSERT INTO Temas (Titulo, Contenido, FechaCreacion, FechaUltimaActividad, Cerrado, Fijado, Vistas, UsuarioId, CategoriaId)
VALUES 
    ('Entity Framework Core: Lazy Loading vs Eager Loading', 
     '¿Cuándo es mejor usar Lazy Loading y cuándo Eager Loading en Entity Framework Core? ¿Cuál es más eficiente?',
     DATEADD(DAY, -10, GETUTCDATE()), DATEADD(DAY, -6, GETUTCDATE()), 0, 0, 156, @Usuario3, @Cat3),
    
    ('Code First vs Database First en EF Core', 
     'Para un nuevo proyecto, ¿qué enfoque recomiendan: Code First o Database First? ¿Cuáles son las ventajas de cada uno?',
     DATEADD(DAY, -5, GETUTCDATE()), DATEADD(DAY, -2, GETUTCDATE()), 0, 0, 92, @Usuario4, @Cat3),
  
    ('Optimización de queries con Include y ThenInclude', 
     'Mis queries están muy lentas cuando cargo entidades relacionadas. ¿Cómo optimizar el uso de Include y ThenInclude?',
     DATEADD(DAY, -4, GETUTCDATE()), DATEADD(DAY, -1, GETUTCDATE()), 0, 0, 68, @Usuario5, @Cat3);

-- Temas sobre Desarrollo Web
INSERT INTO Temas (Titulo, Contenido, FechaCreacion, FechaUltimaActividad, Cerrado, Fijado, Vistas, UsuarioId, CategoriaId)
VALUES 
    ('Bootstrap 5 vs Tailwind CSS: ¿Cuál elegir?', 
     'Estoy iniciando un nuevo proyecto frontend y debo elegir entre Bootstrap 5 y Tailwind CSS. ¿Qué experiencias tienen con cada uno?',
     DATEADD(DAY, -9, GETUTCDATE()), DATEADD(DAY, -7, GETUTCDATE()), 0, 0, 125, @Usuario2, @Cat4),
  
    ('Introducción a TypeScript para desarrolladores C#', 
     'Vengo del mundo .NET/C# y necesito aprender TypeScript. ¿Qué recursos recomiendan?',
     DATEADD(DAY, -3, GETUTCDATE()), DATEADD(HOUR, -12, GETUTCDATE()), 0, 0, 54, @Usuario3, @Cat4);

-- Temas sobre Bases de Datos
INSERT INTO Temas (Titulo, Contenido, FechaCreacion, FechaUltimaActividad, Cerrado, Fijado, Vistas, UsuarioId, CategoriaId)
VALUES 
    ('Índices en SQL Server: Guía práctica', 
     '¿Alguien tiene una guía práctica sobre cuándo y cómo crear índices en SQL Server para mejorar el rendimiento?',
     DATEADD(DAY, -11, GETUTCDATE()), DATEADD(DAY, -9, GETUTCDATE()), 0, 0, 134, @Usuario4, @Cat5),
  
    ('Migraciones en Entity Framework Core', 
     'Tengo problemas con las migraciones en EF Core. ¿Cómo manejar cambios en producción de forma segura?',
     DATEADD(DAY, -2, GETUTCDATE()), DATEADD(HOUR, -6, GETUTCDATE()), 0, 0, 47, @Usuario5, @Cat5);

-- Temas sobre DevOps
INSERT INTO Temas (Titulo, Contenido, FechaCreacion, FechaUltimaActividad, Cerrado, Fijado, Vistas, UsuarioId, CategoriaId)
VALUES 
    ('CI/CD con Azure DevOps para aplicaciones .NET', 
     'Quiero implementar CI/CD para mi aplicación ASP.NET Core. ¿Algún tutorial completo de Azure DevOps?',
     DATEADD(DAY, -13, GETUTCDATE()), DATEADD(DAY, -11, GETUTCDATE()), 0, 0, 167, @Usuario2, @Cat6),
    
    ('Docker para desarrolladores .NET: Primeros pasos', 
     'Nunca he usado Docker. ¿Por dónde empiezo para containerizar mi aplicación ASP.NET Core?',
     DATEADD(DAY, -1, GETUTCDATE()), DATEADD(HOUR, -3, GETUTCDATE()), 0, 0, 38, @Usuario3, @Cat6);

-- Temas sobre Arquitectura
INSERT INTO Temas (Titulo, Contenido, FechaCreacion, FechaUltimaActividad, Cerrado, Fijado, Vistas, UsuarioId, CategoriaId)
VALUES 
    ('Clean Architecture en .NET: Implementación práctica', 
     'Estoy aprendiendo Clean Architecture. ¿Alguien tiene un ejemplo de proyecto completo en .NET?',
     DATEADD(DAY, -14, GETUTCDATE()), DATEADD(DAY, -12, GETUTCDATE()), 0, 0, 198, @Usuario4, @Cat7),
    
    ('Repository Pattern vs Direct DbContext', 
 '¿Vale la pena implementar el patrón Repository o es mejor usar DbContext directamente?',
     DATEADD(HOUR, -18, GETUTCDATE()), DATEADD(HOUR, -2, GETUTCDATE()), 0, 0, 23, @Usuario5, @Cat7),
    
    ('CQRS y MediatR en ASP.NET Core', 
     'Me gustaría implementar CQRS con MediatR en mi proyecto. ¿Alguien tiene experiencia con esto?',
     DATEADD(HOUR, -8, GETUTCDATE()), DATEADD(HOUR, -1, GETUTCDATE()), 0, 0, 15, @Usuario2, @Cat7);

PRINT 'Temas insertados: ' + CAST(@@ROWCOUNT AS VARCHAR);

-- ============================================
-- 4. INSERTAR MENSAJES
-- ============================================
PRINT 'Insertando mensajes de prueba...';

-- Mensajes para el tema "Bienvenidos"
DECLARE @Tema1 INT = (SELECT Id FROM Temas WHERE Titulo = 'Bienvenidos al Foro de Desarrollo .NET');

INSERT INTO Mensajes (Contenido, FechaCreacion, Editado, TemaId, UsuarioId)
VALUES 
    ('¡Gracias por crear este espacio! Espero aprender mucho aquí.', DATEADD(MINUTE, -30, GETUTCDATE()), 0, @Tema1, @Usuario2),
    ('Excelente iniciativa. ¡A compartir conocimientos!', DATEADD(MINUTE, -25, GETUTCDATE()), 0, @Tema1, @Usuario3);

-- Mensajes para otros temas populares
DECLARE @TemaJWT INT = (SELECT Id FROM Temas WHERE Titulo LIKE '%JWT Authentication%');

INSERT INTO Mensajes (Contenido, FechaCreacion, Editado, TemaId, UsuarioId)
VALUES 
    ('Aquí te dejo un ejemplo completo de JWT con refresh tokens...', DATEADD(DAY, -4, GETUTCDATE()), 0, @TemaJWT, @Usuario3),
    ('Muchas gracias! Me sirvió mucho tu ejemplo.', DATEADD(DAY, -3, GETUTCDATE()), 0, @TemaJWT, @Usuario4);

PRINT 'Mensajes insertados: ' + CAST(@@ROWCOUNT AS VARCHAR);

-- ============================================
-- RESUMEN
-- ============================================
PRINT '============================================';
PRINT 'Resumen de datos insertados:';
PRINT '--------------------------------------------';
SELECT 'Usuarios' AS Tabla, COUNT(*) AS Total FROM Usuarios
UNION ALL
SELECT 'Categorías' AS Tabla, COUNT(*) AS Total FROM Categorias
UNION ALL
SELECT 'Temas' AS Tabla, COUNT(*) AS Total FROM Temas
UNION ALL
SELECT 'Mensajes' AS Tabla, COUNT(*) AS Total FROM Mensajes;
PRINT '============================================';
PRINT '¡Datos de prueba insertados exitosamente!';
PRINT '============================================';

GO
