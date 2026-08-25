using Abstracciones.Interfaces.DA.Seguridad;
using Abstracciones.Interfaces.Flujo.Seguridad;
using Abstracciones.Modelos.Seguridad;

namespace Flujo.Seguridad
{
    public class SeguridadFlujo : ISeguridadFlujo
    {

        private ISeguridadDA _seguridadDA;

        public SeguridadFlujo(ISeguridadDA seguridadDA)
        {
            _seguridadDA = seguridadDA;
        }

        public async Task<IEnumerable<Perfil>> ObtenerPerfilesxUsuario(Usuario usuario)
        {
            return await _seguridadDA.ObtenerPerfilesxUsuario(usuario);
        }

        public async Task<Usuario> ObtenerUsuario(Usuario usuario)
        {
            return await _seguridadDA.ObtenerUsuario(usuario);
        }
    }
}
