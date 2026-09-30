using System.Web.Http;
using Abstracciones.Modelos.Seguridad;
using Microsoft.AspNetCore.Mvc;

namespace Abstracciones.Interfaces.API.Seguridad
{
    public interface IAutenticacionController
    {
        Task<IActionResult> PostAsync([FromBody] Login login);
    }
}
