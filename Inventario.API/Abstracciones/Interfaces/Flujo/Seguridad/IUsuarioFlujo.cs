using Abstracciones.Modelos.Seguridad;

namespace Abstracciones.Interfaces.Flujo.Seguridad
{
    public interface IUsuarioFlujo
    {
        Task<Guid> CrearUsuario(Usuario usuario);
        Task<Usuario> ObtenerUsuario(Usuario usuario);
    }
}
