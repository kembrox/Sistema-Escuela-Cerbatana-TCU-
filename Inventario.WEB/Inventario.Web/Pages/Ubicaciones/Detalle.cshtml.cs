using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Text.Json;
using Abstracciones.Modelos;

namespace Inventario.Web.Pages.Ubicaciones
{
    [Authorize]
    public class DetalleModel : PageModel
    {
        private readonly IConfiguration _configuracion;
        private readonly IHttpClientFactory _httpClientFactory;

        // Utilizamos UbicacionResponse para tener acceso a la propiedad Id
        [BindProperty]
        public UbicacionResponse Ubicacion { get; set; } = default!;

        public DetalleModel(IConfiguration configuracion, IHttpClientFactory httpClientFactory)
        {
            _configuracion = configuracion;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return RedirectToPage("./Index");
            }

            var metodos = _configuracion.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>();
            var endpoint = metodos?.FirstOrDefault(m => m.Nombre == "ObtenerUbicacionPorId")?.Valor;

            var cliente = _httpClientFactory.CreateClient("InventarioAPI");

            var token = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var urlFinal = string.Format(endpoint, id);
            var solicitud = new HttpRequestMessage(HttpMethod.Get, urlFinal);
            var respuesta = await cliente.SendAsync(solicitud);

            if (respuesta.IsSuccessStatusCode)
            {
                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                Ubicacion = JsonSerializer.Deserialize<UbicacionResponse>(resultado, opciones);
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}