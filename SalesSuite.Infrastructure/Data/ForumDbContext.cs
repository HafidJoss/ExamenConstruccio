using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SalesSuite.Domain.Entities;

namespace SalesSuite.Infrastructure.Data;

/// <summary>
/// Contexto de base de datos para el sistema de foro con soporte para Identity
/// </summary>
public class ForumDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    /// Constructor del contexto
    /// </summary>
    /// <param name="options">Opciones de configuración del contexto</param>
    public ForumDbContext(DbContextOptions<ForumDbContext> options) : base(options)
    {
    }

    // DbSets
    /// <summary>
    /// Conjunto de entidades Usuario
    /// </summary>
    public DbSet<Usuario> Usuarios { get; set; } = null!;

    /// <summary>
    /// Conjunto de entidades Categoria
    /// </summary>
    public DbSet<Categoria> Categorias { get; set; } = null!;

    /// <summary>
    /// Conjunto de entidades Tema
    /// </summary>
    public DbSet<Tema> Temas { get; set; } = null!;

    /// <summary>
    /// Conjunto de entidades Mensaje
    /// </summary>
    public DbSet<Mensaje> Mensajes { get; set; } = null!;

    /// <summary>
    /// Configura el modelo de datos usando Fluent API
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de la entidad Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            // Configuración de la tabla
            entity.ToTable("Usuarios");

            // Clave primaria
            entity.HasKey(e => e.Id);

            // Configuración de propiedades
            entity.Property(e => e.NombreUsuario)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(e => e.NombreCompleto)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Biografia)
                .HasMaxLength(500);

            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(500);

            entity.Property(e => e.FechaRegistro)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UltimoAcceso);

            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.Rol)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Usuario");

            // Índices
            entity.HasIndex(e => e.NombreUsuario)
                .IsUnique()
                .HasDatabaseName("IX_Usuarios_NombreUsuario");

            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("IX_Usuarios_Email");

            entity.HasIndex(e => e.FechaRegistro)
                .HasDatabaseName("IX_Usuarios_FechaRegistro");

            // Relaciones
            entity.HasMany(e => e.TemasCreados)
                .WithOne(t => t.Usuario)
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Temas_Usuarios");

            entity.HasMany(e => e.MensajesEscritos)
                .WithOne(m => m.Usuario)
                .HasForeignKey(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Mensajes_Usuarios");
        });

        // Configuración de la entidad Categoria
        modelBuilder.Entity<Categoria>(entity =>
        {
            // Configuración de la tabla
            entity.ToTable("Categorias");

            // Clave primaria
            entity.HasKey(e => e.Id);

            // Configuración de propiedades
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Descripcion)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Icono)
                .HasMaxLength(50);

            entity.Property(e => e.Orden)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(e => e.Activa)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.FechaCreacion)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Índices
            entity.HasIndex(e => e.Slug)
                .IsUnique()
                .HasDatabaseName("IX_Categorias_Slug");

            entity.HasIndex(e => e.Orden)
                .HasDatabaseName("IX_Categorias_Orden");

            entity.HasIndex(e => e.Nombre)
                .HasDatabaseName("IX_Categorias_Nombre");

            // Relaciones
            entity.HasMany(e => e.Temas)
                .WithOne(t => t.Categoria)
                .HasForeignKey(t => t.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Temas_Categorias");
        });

        // Configuración de la entidad Tema
        modelBuilder.Entity<Tema>(entity =>
        {
            // Configuración de la tabla
            entity.ToTable("Temas");

            // Clave primaria
            entity.HasKey(e => e.Id);

            // Configuración de propiedades
            entity.Property(e => e.Titulo)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Contenido)
                .IsRequired()
                .HasMaxLength(5000);

            entity.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(e => e.NumeroVistas)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(e => e.Cerrado)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.Fijado)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.FechaCreacion)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.FechaUltimaActividad);

            entity.Property(e => e.CategoriaId)
                .IsRequired();

            entity.Property(e => e.UsuarioId)
                .IsRequired();

            // Índices
            entity.HasIndex(e => e.Slug)
                .IsUnique()
                .HasDatabaseName("IX_Temas_Slug");

            entity.HasIndex(e => e.CategoriaId)
                .HasDatabaseName("IX_Temas_CategoriaId");

            entity.HasIndex(e => e.UsuarioId)
                .HasDatabaseName("IX_Temas_UsuarioId");

            entity.HasIndex(e => e.FechaCreacion)
                .HasDatabaseName("IX_Temas_FechaCreacion");

            entity.HasIndex(e => e.FechaUltimaActividad)
                .HasDatabaseName("IX_Temas_FechaUltimaActividad");

            entity.HasIndex(e => new { e.Fijado, e.FechaUltimaActividad })
                .HasDatabaseName("IX_Temas_Fijado_FechaUltimaActividad");

            // Relaciones
            entity.HasOne(e => e.Categoria)
                .WithMany(c => c.Temas)
                .HasForeignKey(e => e.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Temas_Categorias");

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.TemasCreados)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Temas_Usuarios");

            entity.HasMany(e => e.Mensajes)
                .WithOne(m => m.Tema)
                .HasForeignKey(m => m.TemaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Mensajes_Temas");
        });

        // Configuración de la entidad Mensaje
        modelBuilder.Entity<Mensaje>(entity =>
        {
            // Configuración de la tabla
            entity.ToTable("Mensajes");

            // Clave primaria
            entity.HasKey(e => e.Id);

            // Configuración de propiedades
            entity.Property(e => e.Contenido)
                .IsRequired()
                .HasMaxLength(5000);

            entity.Property(e => e.FechaCreacion)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.FechaEdicion);

            entity.Property(e => e.Editado)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.NumeroMeGusta)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(e => e.Oculto)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.RazonOculto)
                .HasMaxLength(500);

            entity.Property(e => e.TemaId)
                .IsRequired();

            entity.Property(e => e.UsuarioId)
                .IsRequired();

            entity.Property(e => e.MensajePadreId);

            // Índices
            entity.HasIndex(e => e.TemaId)
                .HasDatabaseName("IX_Mensajes_TemaId");

            entity.HasIndex(e => e.UsuarioId)
                .HasDatabaseName("IX_Mensajes_UsuarioId");

            entity.HasIndex(e => e.FechaCreacion)
                .HasDatabaseName("IX_Mensajes_FechaCreacion");

            entity.HasIndex(e => e.MensajePadreId)
                .HasDatabaseName("IX_Mensajes_MensajePadreId");

            entity.HasIndex(e => new { e.TemaId, e.FechaCreacion })
                .HasDatabaseName("IX_Mensajes_TemaId_FechaCreacion");

            // Relaciones
            entity.HasOne(e => e.Tema)
                .WithMany(t => t.Mensajes)
                .HasForeignKey(e => e.TemaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Mensajes_Temas");

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.MensajesEscritos)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Mensajes_Usuarios");

            // Relación auto-referencial para respuestas
            entity.HasOne(e => e.MensajePadre)
                .WithMany(m => m.Respuestas)
                .HasForeignKey(e => e.MensajePadreId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Mensajes_MensajePadre");
        });
    }
}
