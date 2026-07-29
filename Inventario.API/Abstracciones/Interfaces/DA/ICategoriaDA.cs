using Abstracciones.Modelos;

namespace Abstracciones.Interfaces.DA
{
    public interface ICategoriaDA
    {
        Task<IEnumerable<CategoriaResponse>> Obtener();
        Task<CategoriaResponse> Obtener(Guid Id);
        Task<Guid> Agregar(CategoriaResponse categoria);
        Task<Guid> Editar(Guid Id, CategoriaResponse categoria);
        Task<Guid> Eliminar(Guid Id);
    }
}
