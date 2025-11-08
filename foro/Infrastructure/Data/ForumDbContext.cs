using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using foro.Domain.Entities;

namespace foro.Infrastructure.Data;

/// <summary>
/// Contexto de base de datos para el foro con soporte para ASP.NET Core Identity
/// </summary>
public class ForumDbContext : IdentityDbContext<ApplicationUser>
{
    public ForumDbContext(DbContextOptions<ForumDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Tema> Temas { get; set; }
    public DbSet<Mensaje> Mensajes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

      // Configuración de ApplicationUser (Identity)
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
  entity.ToTable("AspNetUsers"); // Tabla estándar de Identity
            
    // IGNORAR las propiedades de navegación hacia Tema y Mensaje
       // porque estas entidades usan la tabla Usuario legacy
   entity.Ignore(u => u.Temas);
            entity.Ignore(u => u.Mensajes);
        });

        // Configuración de Usuario (entidad antigua, mantener para compatibilidad)
        modelBuilder.Entity<Usuario>(entity =>
        {
    entity.ToTable("Usuarios");
 entity.HasKey(u => u.Id);

            entity.Property(u => u.Nombre)
            .IsRequired()
      .HasMaxLength(100);

     entity.Property(u => u.Email)
           .IsRequired()
       .HasMaxLength(200);

            entity.HasIndex(u => u.Email)
           .IsUnique()
      .HasDatabaseName("IX_Usuarios_Email");

      entity.Property(u => u.Biografia)
     .HasMaxLength(500);

            entity.Property(u => u.FechaRegistro)
     .IsRequired()
     .HasDefaultValueSql("GETUTCDATE()");

         entity.Property(u => u.Activo)
                .IsRequired()
       .HasDefaultValue(true);

     // Configurar relaciones con comportamiento NO CASCADE
            entity.HasMany(u => u.Temas)
                .WithOne(t => t.Usuario)
       .HasForeignKey(t => t.UsuarioId)
   .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.Mensajes)
                .WithOne(m => m.Usuario)
             .HasForeignKey(m => m.UsuarioId)
      .OnDelete(DeleteBehavior.Restrict);
        });

   // Configuración de Categoria
     modelBuilder.Entity<Categoria>(entity =>
        {
       entity.ToTable("Categorias");
            entity.HasKey(c => c.Id);

      entity.Property(c => c.Nombre)
     .IsRequired()
       .HasMaxLength(150);

entity.HasIndex(c => c.Nombre)
      .IsUnique()
                .HasDatabaseName("IX_Categorias_Nombre");

            entity.Property(c => c.Descripcion)
       .HasMaxLength(500);

 entity.Property(c => c.FechaCreacion)
           .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        entity.Property(c => c.Activa)
                .IsRequired()
                .HasDefaultValue(true);

    entity.Property(c => c.Orden)
         .IsRequired()
       .HasDefaultValue(0);

   // Relaciones
       entity.HasMany(c => c.Temas)
             .WithOne(t => t.Categoria)
   .HasForeignKey(t => t.CategoriaId)
         .OnDelete(DeleteBehavior.Restrict);
        });

      // Configuración de Tema
     modelBuilder.Entity<Tema>(entity =>
     {
      entity.ToTable("Temas");
      entity.HasKey(t => t.Id);

    entity.Property(t => t.Titulo)
           .IsRequired()
     .HasMaxLength(250);

  entity.Property(t => t.Contenido)
    .IsRequired()
         .HasMaxLength(5000);

       entity.Property(t => t.FechaCreacion)
      .IsRequired()
       .HasDefaultValueSql("GETUTCDATE()");

 entity.Property(t => t.FechaUltimaActividad)
             .IsRequired(false);

     entity.Property(t => t.Cerrado)
              .IsRequired()
                .HasDefaultValue(false);

            entity.Property(t => t.Fijado)
        .IsRequired()
         .HasDefaultValue(false);

         entity.Property(t => t.Vistas)
     .IsRequired()
        .HasDefaultValue(0);

      // Índices para mejorar el rendimiento
     entity.HasIndex(t => t.FechaCreacion)
      .HasDatabaseName("IX_Temas_FechaCreacion");

            entity.HasIndex(t => t.CategoriaId)
       .HasDatabaseName("IX_Temas_CategoriaId");

         entity.HasIndex(t => t.UsuarioId)
            .HasDatabaseName("IX_Temas_UsuarioId");

    // Relación con Mensajes: Cascade (si se borra un tema, se borran sus mensajes)
      entity.HasMany(t => t.Mensajes)
     .WithOne(m => m.Tema)
 .HasForeignKey(m => m.TemaId)
     .OnDelete(DeleteBehavior.Cascade);
            
      // Relaciones con Usuario y Categoria ya configuradas en esas entidades
        });

        // Configuración de Mensaje
        modelBuilder.Entity<Mensaje>(entity =>
        {
       entity.ToTable("Mensajes");
     entity.HasKey(m => m.Id);

            entity.Property(m => m.Contenido)
      .IsRequired()
      .HasMaxLength(5000);

    entity.Property(m => m.FechaCreacion)
     .IsRequired()
  .HasDefaultValueSql("GETUTCDATE()");

       entity.Property(m => m.FechaEdicion)
        .IsRequired(false);

            entity.Property(m => m.Editado)
        .IsRequired()
        .HasDefaultValue(false);

            // Índices para mejorar el rendimiento
            entity.HasIndex(m => m.TemaId)
    .HasDatabaseName("IX_Mensajes_TemaId");

   entity.HasIndex(m => m.UsuarioId)
      .HasDatabaseName("IX_Mensajes_UsuarioId");

       entity.HasIndex(m => m.FechaCreacion)
        .HasDatabaseName("IX_Mensajes_FechaCreacion");

// Relaciones ya configuradas desde Tema y Usuario
        });
    }
}
