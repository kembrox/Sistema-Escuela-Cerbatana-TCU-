using Abstracciones.Modelos.Seguridad;

namespace Abstracciones.Interfaces.DA.Seguridad
{
    public interface ISeguridadDA
    {
        Task<Usuario> ObtenerUsuario(Usuario usuario);
        Task<IEnumerable<Perfil>> ObtenerPerfilesxUsuario(Usuario usuario);
    }
}
