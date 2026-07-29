using Abstracciones.Modelos;

namespace Abstracciones.Interfaces.Flujo
{
    public interface IInventarioFlujo
    {
        Task<IEnumerable<InventarioResponse>> Obtener();
        Task<InventarioResponse> Obtener(Guid Id);
        Task<Guid> Agregar(InventarioResponse inventario);
        Task<Guid> Editar(Guid Id, InventarioResponse inventario);
        Task<Guid> Eliminar(Guid Id);
    }
}
