using Abstracciones.Interfaces.Flujo.Seguridad;
using Abstracciones.Interfaces.Reglas.Seguridad;
using Abstracciones.Modelos.Seguridad;

namespace Flujo.Seguridad
{
    public class AutenticacionFlujo : IAutenticacionFlujo
    {
        private IAutenticacionReglas _autenticacionReglas;

        public AutenticacionFlujo(IAutenticacionReglas autenticacionReglas)
        {
            _autenticacionReglas = autenticacionReglas;
        }

        public async Task<Token> LoginAsync(Login login)
        {
            return await _autenticacionReglas.LoginAsync(login);
        }
    }
}
