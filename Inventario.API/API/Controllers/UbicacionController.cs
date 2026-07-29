using Abstracciones.Interfaces.API;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class UbicacionController : ControllerBase , IUbicacionController
    {

        private readonly IUbicacionFlujo _ubicacionFlujo;
        private readonly ILogger<UbicacionController> _logger;

        public UbicacionController(IUbicacionFlujo ubicacionFlujo, ILogger<UbicacionController> logger)
        {
            _ubicacionFlujo = ubicacionFlujo;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] UbicacionResponse ubicacion)
        {
            var resultado = await _ubicacionFlujo.Agregar(ubicacion);
            return CreatedAtAction(nameof(Obtener), new { Id = resultado }, null);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Editar([FromRoute] Guid Id, [FromBody] UbicacionResponse ubicacion)
        {
            if (!await VerificarUbicacionExiste(Id))
                return NotFound("La ubicación no existe");
            var resultado = await _ubicacionFlujo.Editar(Id, ubicacion);
            return Ok(resultado); 
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Eliminar([FromRoute] Guid Id)
        {
            if (!await VerificarUbicacionExiste(Id))
                return NotFound("La ubicación no existe");
            var resultado = await _ubicacionFlujo.Eliminar(Id);
            return NoContent(); 
        }

        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            var resultado = await _ubicacionFlujo.Obtener();

            if (!resultado.Any())
            {
                return NoContent(); 
            }
            return Ok(resultado); 
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> Obtener([FromRoute] Guid Id)
        {
            if (!await VerificarUbicacionExiste(Id))
                return NotFound("La ubicación no existe");
            var resultado = await _ubicacionFlujo.Obtener(Id);
            return Ok(resultado);
        }

        #region Helpers

        private async Task<bool> VerificarUbicacionExiste(Guid Id)
        {
            var resultadoValidacion = false;
            var resultadoUbicacionExiste = await _ubicacionFlujo.Obtener(Id);

            if (resultadoUbicacionExiste != null)
                resultadoValidacion = true;

            return resultadoValidacion;
        }

        #endregion

    }
}
