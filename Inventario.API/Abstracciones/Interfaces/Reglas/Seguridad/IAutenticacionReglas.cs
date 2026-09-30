using Abstracciones.Modelos.Seguridad;

namespace Abstracciones.Interfaces.Reglas.Seguridad
{
    public interface IAutenticacionReglas
    {
        Task<Token> LoginAsync(Login login);
    }
}
