using Microsoft.Extensions.Logging;
using foro.Domain.Entities;
using foro.Domain.Interfaces;
using foro.Application.Interfaces;

namespace foro.Application.UseCases.Temas;

/// <summary>
/// Caso de uso para crear un tema con su mensaje inicial usando transacciones
/// </summary>
public class CrearTemaUseCase : ICrearTemaUseCase
{
    private readonly IUnitOfWork _unitOfWork;
  private readonly ILogger<CrearTemaUseCase> _logger;

    public CrearTemaUseCase(IUnitOfWork unitOfWork, ILogger<CrearTemaUseCase> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CrearTemaResponse> ExecuteAsync(CrearTemaRequest request)
    {
        var response = new CrearTemaResponse();

        try
        {
 // Validar request
            var validationErrors = await ValidateRequestAsync(request);
      if (validationErrors.Any())
            {
                response.Exito = false;
           response.Errores = validationErrors;
       response.Mensaje = "Error de validación";
 return response;
     }

            // Iniciar transacción
            await _unitOfWork.BeginTransactionAsync();

            try
            {
      // 1. Crear el tema
 var tema = new Tema
     {
Titulo = request.Titulo,
    Contenido = request.Contenido,
          UsuarioId = request.UsuarioId,
         CategoriaId = request.CategoriaId,
      FechaCreacion = DateTime.UtcNow,
       FechaUltimaActividad = DateTime.UtcNow,
           Cerrado = false,
  Fijado = false,
          Vistas = 0
 };

      await _unitOfWork.Temas.AddAsync(tema);
 await _unitOfWork.CommitAsync(); // Guardar para obtener el ID

      _logger.LogInformation("Tema creado con ID: {TemaId}", tema.Id);

   // 2. Crear el mensaje inicial
        var mensaje = new Mensaje
{
      Contenido = request.Contenido,
     TemaId = tema.Id,
        UsuarioId = request.UsuarioId,
         FechaCreacion = DateTime.UtcNow,
           Editado = false
    };

     await _unitOfWork.Mensajes.AddAsync(mensaje);
         await _unitOfWork.CommitAsync();

          _logger.LogInformation("Mensaje inicial creado para tema {TemaId}", tema.Id);

         // Confirmar transacción
            await _unitOfWork.CommitTransactionAsync();

     response.Exito = true;
    response.TemaId = tema.Id;
         response.Mensaje = "Tema creado exitosamente";

      _logger.LogInformation("Tema {TemaId} creado exitosamente con mensaje inicial", tema.Id);
    }
          catch (Exception ex)
          {
         // Revertir transacción en caso de error
  await _unitOfWork.RollbackTransactionAsync();
    _logger.LogError(ex, "Error al crear tema. Transacción revertida");
      throw;
  }
        }
        catch (Exception ex)
        {
       response.Exito = false;
          response.Mensaje = "Error al crear el tema";
        response.Errores.Add(ex.Message);
         _logger.LogError(ex, "Error en CrearTemaUseCase");
        }

return response;
    }

 private async Task<List<string>> ValidateRequestAsync(CrearTemaRequest request)
    {
        var errors = new List<string>();

      // Validar título
 if (string.IsNullOrWhiteSpace(request.Titulo))
        {
            errors.Add("El título es obligatorio");
        }
     else if (request.Titulo.Length < 5 || request.Titulo.Length > 250)
        {
         errors.Add("El título debe tener entre 5 y 250 caracteres");
  }

        // Validar contenido
     if (string.IsNullOrWhiteSpace(request.Contenido))
        {
            errors.Add("El contenido es obligatorio");
      }
else if (request.Contenido.Length < 10 || request.Contenido.Length > 5000)
        {
          errors.Add("El contenido debe tener entre 10 y 5000 caracteres");
     }

        // Validar categoría
    if (request.CategoriaId <= 0)
        {
            errors.Add("Debe seleccionar una categoría válida");
        }
   else
        {
       var categoria = await _unitOfWork.Categorias.GetByIdAsync(request.CategoriaId);
   if (categoria == null)
            {
        errors.Add("La categoría seleccionada no existe");
   }
            else if (!categoria.Activa)
  {
       errors.Add("La categoría seleccionada no está activa");
            }
     }

   // Validar usuario
     if (request.UsuarioId <= 0)
        {
        errors.Add("Usuario no válido");
        }
      else
      {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(request.UsuarioId);
      if (usuario == null)
       {
            errors.Add("El usuario no existe");
            }
            else if (!usuario.Activo)
            {
      errors.Add("El usuario no está activo");
            }
        }

        return errors;
    }
}
