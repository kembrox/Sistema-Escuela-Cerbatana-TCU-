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



        [HttpGet("DescargarPlantilla")]
        public IActionResult DescargarPlantilla([FromServices] IImportadorExcelHelper importadorHelper)
        {
            var archivoBytes = importadorHelper.GenerarPlantillaVacia();
            return File(archivoBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Plantilla_Carga_Masiva_Activos.xlsx");
        }

        [HttpPost("ImportarExcel/{idCategoria}")]
        public async Task<IActionResult> ImportarExcel(
            [FromRoute] Guid idCategoria,
            IFormFile archivo,
            [FromServices] IImportadorExcelHelper importadorHelper,
            [FromServices] IUbicacionFlujo ubicacionFlujo) // Inyectamos flujo de ubicaciones para el traductor
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("Debes seleccionar un archivo Excel válido.");

            if (!archivo.FileName.EndsWith(".xlsx"))
                return BadRequest("El archivo debe ser de tipo Excel (.xlsx).");

            try
            {
                // 1. Obtenemos todas las ubicaciones vigentes en el sistema para enviárselas al traductor
                var ubicacionesSistema = await ubicacionFlujo.Obtener();

                // 2. Abrimos el archivo Excel y extraemos los activos mapeados
                using var stream = archivo.OpenReadStream();
                var listaActivos = importadorHelper.LeerActivosDeExcel(stream, ubicacionesSistema, idCategoria);

                if (!listaActivos.Any())
                    return BadRequest("El archivo Excel está vacío o no contiene filas válidas.");

                // 3. Guardamos cada activo leído en la base de datos de manera secuencial
                int guardados = 0;
                foreach (var activo in listaActivos)
                {
                    await _inventarioFlujo.Agregar(activo);
                    guardados++;
                }

                return Ok(new { Mensaje = "Carga masiva finalizada con éxito", TotalRegistrados = guardados });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando el archivo Excel de carga masiva.");
                // Retorna el mensaje exacto del error (por ejemplo, si una ubicación no existía)
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
