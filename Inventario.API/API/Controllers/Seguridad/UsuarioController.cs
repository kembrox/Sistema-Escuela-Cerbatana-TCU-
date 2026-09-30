using Abstracciones.Modelos.Seguridad;
using Abstracciones.Interfaces.API.Seguridad;
using Abstracciones.Interfaces.Flujo.Seguridad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers.Seguridad
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase, IUsuarioController
    {
        private IUsuarioFlujo _usuarioFLujo;

        public UsuarioController(IUsuarioFlujo usuarioFLujo)
        {
            _usuarioFLujo = usuarioFLujo;
        }

        [Authorize(Roles = "2")]
        [HttpPost("ObtenerUsuario")]
        public async Task<IActionResult> ObtenerUsuario([FromBody] Usuario usuario)
        {
            return Ok(await _usuarioFLujo.ObtenerUsuario(usuario));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Usuario usuario)
        {
            return Ok(await _usuarioFLujo.CrearUsuario(usuario));
        }
    }
}
