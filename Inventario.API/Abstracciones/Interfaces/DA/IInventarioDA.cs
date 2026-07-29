using Abstracciones.Modelos;

namespace Abstracciones.Interfaces.DA
{
    public interface IInventarioDA
    {
        Task<IEnumerable<InventarioResponse>> Obtener();
        Task<InventarioResponse> Obtener(Guid Id);
        Task<Guid> Agregar(InventarioResponse inventario);
        Task<Guid> Editar(Guid Id, InventarioResponse inventario);
        Task<Guid> Eliminar(Guid Id);
    }
}
