using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Text.Json;
using Abstracciones.Modelos;

namespace Inventario.Web.Pages.Ubicaciones
{
    [Authorize]
    public class EliminarModel : PageModel
    {
        private readonly IConfiguration _configuracion;
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public UbicacionResponse Ubicacion { get; set; } = default!;

        public EliminarModel(IConfiguration configuracion, IHttpClientFactory httpClientFactory)
        {
            _configuracion = configuracion;
            _httpClientFactory = httpClientFactory;
        }

        // Recupera los datos para mostrarlos en la pantalla de confirmación
        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            var metodos = _configuracion.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>();
            var endpoint = metodos?.FirstOrDefault(m => m.Nombre == "ObtenerUbicacionPorId")?.Valor;

            var cliente = _httpClientFactory.CreateClient("InventarioAPI");
            var token = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var solicitud = new HttpRequestMessage(HttpMethod.Get, string.Format(endpoint, id));
            var respuesta = await cliente.SendAsync(solicitud);

            if (respuesta.IsSuccessStatusCode)
            {
                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                Ubicacion = JsonSerializer.Deserialize<UbicacionResponse>(resultado, opciones);
            }

            if (Ubicacion == null) return RedirectToPage("./Index");

            return Page();
        }

        // Ejecuta la eliminación en la API al confirmar
        public async Task<IActionResult> OnPostAsync()
        {
            if (Ubicacion.Id == Guid.Empty) return NotFound();

            var metodos = _configuracion.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>();
            var endpoint = metodos?.FirstOrDefault(m => m.Nombre == "EliminarUbicacion")?.Valor;

            var cliente = _httpClientFactory.CreateClient("InventarioAPI");
            var token = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Utilizamos HttpMethod.Delete para la petición destructiva
            var solicitud = new HttpRequestMessage(HttpMethod.Delete, string.Format(endpoint, Ubicacion.Id));
            var respuesta = await cliente.SendAsync(solicitud);

            if (respuesta.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }

            ModelState.AddModelError(string.Empty, "No se pudo eliminar la ubicación. Es posible que existan activos vinculados a ella.");
            return Page();
        }
    }
}
