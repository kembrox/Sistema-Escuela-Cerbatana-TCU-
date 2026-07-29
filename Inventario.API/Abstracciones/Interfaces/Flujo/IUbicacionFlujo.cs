using Abstracciones.Modelos;

namespace Abstracciones.Interfaces.Flujo
{
    public interface IUbicacionFlujo
    {
        Task<IEnumerable<UbicacionResponse>> Obtener();
        Task<UbicacionResponse> Obtener(Guid Id);
        Task<Guid> Agregar(UbicacionResponse ubicacion);
        Task<Guid> Editar(Guid Id, UbicacionResponse ubicacion);
        Task<Guid> Eliminar(Guid Id);
    }
}
