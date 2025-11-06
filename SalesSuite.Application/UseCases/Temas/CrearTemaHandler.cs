using Microsoft.Extensions.Logging;
using SalesSuite.Domain.Entities;
using SalesSuite.Domain.Interfaces;

namespace SalesSuite.Application.UseCases.Temas;

/// <summary>
/// Handler que implementa la lógica de negocio para crear un tema con su primer mensaje
/// </summary>
public class CrearTemaHandler : ICrearTemaHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CrearTemaHandler> _logger;

    public CrearTemaHandler(IUnitOfWork unitOfWork, ILogger<CrearTemaHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Ejecuta el caso de uso de crear tema con mensaje inicial
    /// </summary>
    public async Task<CrearTemaResult> HandleAsync(CrearTemaCommand command)
    {
        // Validar comando
        var validationErrors = ValidateCommand(command);
        if (validationErrors.Any())
        {
            _logger.LogWarning("Validación fallida al crear tema: {Errors}", string.Join(", ", validationErrors));
            return CrearTemaResult.CreateValidationError(validationErrors);
        }

        try
        {
            // Iniciar transacción
            await _unitOfWork.BeginTransactionAsync();

            // Validar que la categoría existe
            var categoriaExiste = await _unitOfWork.Categorias.ExistsAsync(c => c.Id == command.CategoriaId && c.Activa);
            if (!categoriaExiste)
            {
                _logger.LogWarning("Categoría {CategoriaId} no existe o no está activa", command.CategoriaId);
                return CrearTemaResult.CreateError("La categoría seleccionada no existe o no está activa");
            }

            // Validar que el usuario existe
            var usuarioExiste = await _unitOfWork.Usuarios.ExistsAsync(u => u.Id == command.UsuarioId && u.Activo);
            if (!usuarioExiste)
            {
                _logger.LogWarning("Usuario {UsuarioId} no existe o no está activo", command.UsuarioId);
                return CrearTemaResult.CreateError("El usuario no existe o no está activo");
            }

            // Crear el tema
            var tema = new Tema
            {
                Titulo = command.Titulo.Trim(),
                Contenido = command.ContenidoMensaje.Trim(),
                Slug = GenerarSlug(command.Titulo),
                CategoriaId = command.CategoriaId,
                UsuarioId = command.UsuarioId,
                Fijado = command.Fijado,
                Cerrado = false,
                NumeroVistas = 0,
                FechaCreacion = DateTime.UtcNow,
                FechaUltimaActividad = DateTime.UtcNow
            };

            await _unitOfWork.Temas.AddAsync(tema);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Tema creado: ID={TemaId}, Título='{Titulo}'", tema.Id, tema.Titulo);

            // Crear el primer mensaje
            var primerMensaje = new Mensaje
            {
                TemaId = tema.Id,
                UsuarioId = command.UsuarioId,
                Contenido = command.ContenidoMensaje.Trim(),
                FechaCreacion = DateTime.UtcNow,
                Editado = false,
                NumeroMeGusta = 0,
                Oculto = false
            };

            await _unitOfWork.Mensajes.AddAsync(primerMensaje);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Primer mensaje creado: ID={MensajeId}, TemaId={TemaId}", primerMensaje.Id, tema.Id);

            // Confirmar transacción
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Tema y mensaje creados exitosamente en transacción: TemaId={TemaId}, MensajeId={MensajeId}", 
                tema.Id, primerMensaje.Id);

            return CrearTemaResult.CreateSuccess(tema.Id, primerMensaje.Id);
        }
        catch (Exception ex)
        {
            // Revertir transacción en caso de error
            await _unitOfWork.RollbackTransactionAsync();
            
            _logger.LogError(ex, "Error al crear tema con mensaje: {Message}", ex.Message);
            return CrearTemaResult.CreateError($"Error al crear el tema: {ex.Message}");
        }
    }

    /// <summary>
    /// Valida el comando antes de procesarlo
    /// </summary>
    private List<string> ValidateCommand(CrearTemaCommand command)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(command.Titulo))
        {
            errors.Add("El título es obligatorio");
        }
        else if (command.Titulo.Length < 5 || command.Titulo.Length > 200)
        {
            errors.Add("El título debe tener entre 5 y 200 caracteres");
        }

        if (string.IsNullOrWhiteSpace(command.ContenidoMensaje))
        {
            errors.Add("El contenido del mensaje es obligatorio");
        }
        else if (command.ContenidoMensaje.Length < 10 || command.ContenidoMensaje.Length > 5000)
        {
            errors.Add("El contenido debe tener entre 10 y 5000 caracteres");
        }

        if (command.CategoriaId <= 0)
        {
            errors.Add("Debe seleccionar una categoría válida");
        }

        if (command.UsuarioId <= 0)
        {
            errors.Add("El usuario no es válido");
        }

        return errors;
    }

    /// <summary>
    /// Genera un slug amigable para URLs a partir de un título
    /// </summary>
    private string GenerarSlug(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            return string.Empty;

        // Convertir a minúsculas
        string slug = titulo.ToLowerInvariant();

        // Reemplazar caracteres especiales
        slug = slug.Replace("á", "a").Replace("é", "e").Replace("í", "i")
                   .Replace("ó", "o").Replace("ú", "u").Replace("ñ", "n");

        // Remover caracteres no alfanuméricos excepto espacios y guiones
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");

        // Reemplazar espacios múltiples por uno solo
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", " ").Trim();

        // Reemplazar espacios por guiones
        slug = slug.Replace(" ", "-");

        // Remover guiones múltiples
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-");

        // Limitar longitud
        if (slug.Length > 100)
            slug = slug.Substring(0, 100).TrimEnd('-');

        return slug;
    }
}
