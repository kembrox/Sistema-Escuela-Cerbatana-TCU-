using Abstracciones.Modelos.Seguridad;

namespace Abstracciones.Interfaces.Flujo.Seguridad
{
    public interface ISeguridadFlujo
    {
        Task<Usuario> ObtenerUsuario(Usuario usuario);
        Task<IEnumerable<Perfil>> ObtenerPerfilesxUsuario(Usuario usuario);
    }
}
