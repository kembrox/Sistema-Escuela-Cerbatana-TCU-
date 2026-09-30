
using Abstracciones.Modelos.Seguridad;

namespace Abstracciones.Interfaces.Flujo.Seguridad
{
    public interface IAutenticacionFlujo
    {
        Task<Token> LoginAsync(Login login);
    }
}
