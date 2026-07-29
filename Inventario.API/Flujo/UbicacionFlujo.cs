using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;

namespace Flujo
{
    public class UbicacionFlujo : IUbicacionFlujo
    {
        private readonly IUbicacionDA _ubicacionDA;

        public UbicacionFlujo(IUbicacionDA ubicacionDA)
        {
            _ubicacionDA = ubicacionDA;
        }

        public async Task<Guid> Agregar(UbicacionResponse ubicacion)
        {
            return await _ubicacionDA.Agregar(ubicacion);
        }

        public async Task<Guid> Editar(Guid Id, UbicacionResponse ubicacion)
        {
            return await _ubicacionDA.Editar(Id, ubicacion);
        }

        public async Task<Guid> Eliminar(Guid Id)
        {
            return await _ubicacionDA.Eliminar(Id);
        }

        public async Task<IEnumerable<UbicacionResponse>> Obtener()
        {
            return await _ubicacionDA.Obtener();
        }

        public async Task<UbicacionResponse> Obtener(Guid Id)
        {
            var resultado = await _ubicacionDA.Obtener(Id);
            return resultado;
        }
    }
}
