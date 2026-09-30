using Abstracciones.Modelos.Seguridad;

namespace Abstracciones.Interfaces.DA.Seguridad
{
    public interface IUsuarioDA
    {
        Task<Guid> CrearUsuario(Usuario usuario);
        Task<Usuario> ObtenerUsuario(Usuario usuario);
    }
}
