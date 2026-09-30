using System.Web.Http;
using Abstracciones.Modelos.Seguridad;
using Microsoft.AspNetCore.Mvc;

namespace Abstracciones.Interfaces.API.Seguridad
{
    public interface IUsuarioController
    {
        Task<IActionResult> PostAsync([FromBody] Usuario usuario);
        Task<IActionResult> ObtenerUsuario([FromBody] Usuario usuario);
    }
}
