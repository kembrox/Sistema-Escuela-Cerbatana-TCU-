using Abstracciones.Interfaces.API;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class InventarioController : ControllerBase, IInventarioController
    {
        private readonly IInventarioFlujo _inventarioFlujo;
        private readonly ILogger<InventarioController> _logger;

        public InventarioController(IInventarioFlujo inventarioFlujo, ILogger<InventarioController> logger)
        {
            _inventarioFlujo = inventarioFlujo;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] InventarioResponse inventario)
        {
            var resultado = await _inventarioFlujo.Agregar(inventario);
            return CreatedAtAction(nameof(Obtener), new { Id = resultado }, null);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Editar([FromRoute] Guid Id, [FromBody] InventarioResponse inventario)
        {
            if (!await VerificarInventarioExiste(Id))
                return NotFound("El activo no existe en el inventario");
            var resultado = await _inventarioFlujo.Editar(Id, inventario);
            return Ok(resultado);       
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Eliminar(Guid Id)
        {
            if (!await VerificarInventarioExiste(Id))
                return NotFound("El activo no existe en el inventario");
            var resultado = await _inventarioFlujo.Eliminar(Id);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            var resultado = await _inventarioFlujo.Obtener();

            if (!resultado.Any())
            {
                return NoContent(); 
            }
            return Ok(resultado);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> Obtener(Guid Id)
        {
            if (!await VerificarInventarioExiste(Id))
                return NotFound("El activo no existe en el inventario");
            var resultado = await _inventarioFlujo.Obtener(Id);
            return Ok(resultado);
        }

        #region Helpers

        private async Task<bool> VerificarInventarioExiste(Guid Id)
        {
            var resultadoValidacion = false;
            var resultadoInventarioExiste = await _inventarioFlujo.Obtener(Id);

            if (resultadoInventarioExiste != null)
                resultadoValidacion = true;

            return resultadoValidacion;
        }

        #endregion

        #region Reportes

        [HttpGet("ExportarExcel")]
        public async Task<IActionResult> ExportarExcel([FromServices] IReportesHelper reportesHelper)
        {
            var activos = await _inventarioFlujo.Obtener();
            var archivoBytes = reportesHelper.GenerarExcelInventario(activos);

            return File(archivoBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Inventario_Cerbatana.xlsx");
        }

        [HttpGet("ExportarPdf")]
        public async Task<IActionResult> ExportarPdf([FromServices] IReportesHelper reportesHelper)
        {
            var activos = await _inventarioFlujo.Obtener();
            var archivoBytes = reportesHelper.GenerarPdfInventario(activos);

            return File(archivoBytes, "application/pdf", "Inventario_Cerbatana.pdf");
        }
        #endregion
    }
}
