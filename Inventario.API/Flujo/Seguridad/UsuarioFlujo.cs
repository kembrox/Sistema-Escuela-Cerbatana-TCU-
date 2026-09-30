using Abstracciones.Interfaces.DA.Seguridad;
using Abstracciones.Interfaces.Flujo.Seguridad;
using Abstracciones.Modelos.Seguridad;
using BCrypt.Net;

namespace Flujo.Seguridad
{
    public class UsuarioFlujo : IUsuarioFlujo
    {
        private IUsuarioDA _usuarioDA;

        public UsuarioFlujo(IUsuarioDA usuarioDA)
        {
            _usuarioDA = usuarioDA;
        }

        public async Task<Guid> CrearUsuario(Usuario usuario)
        {
            usuario.Perfiles = new List<Perfil>
            {
                new Perfil { Id = 1, Nombre = "Administrador" }
            };
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);
            return await _usuarioDA.CrearUsuario(usuario);
        }

        public async Task<Usuario> ObtenerUsuario(Usuario usuario)
        {
            return await _usuarioDA.ObtenerUsuario(usuario);
        }
    }
}
