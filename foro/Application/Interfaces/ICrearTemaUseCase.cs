using foro.Application.UseCases.Temas;

namespace foro.Application.Interfaces;

/// <summary>
/// Interfaz para el caso de uso de crear tema con mensaje inicial
/// </summary>
public interface ICrearTemaUseCase
{
    Task<CrearTemaResponse> ExecuteAsync(CrearTemaRequest request);
}
