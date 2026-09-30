using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Text.Json;
using Abstracciones.Modelos; // Importamos tu clase UbicacionResponse

namespace Inventario.Web.Pages.Ubicaciones
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuracion;
        private readonly IHttpClientFactory _httpClientFactory;

        // Utilizamos UbicacionResponse porque incluye el Guid Id
        public IList<UbicacionResponse> Ubicaciones { get; set; } = default!;

        public IndexModel(IConfiguration configuracion, IHttpClientFactory httpClientFactory)
        {
            _configuracion = configuracion;
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            // 1. Obtenemos la ruta desde appsettings.json
            var metodos = _configuracion.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>();
            var endpoint = metodos?.FirstOrDefault(m => m.Nombre == "ObtenerUbicaciones")?.Valor;

            // 2. Preparamos la petición
            var cliente = _httpClientFactory.CreateClient("InventarioAPI");
            var token = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // 3. Ejecutamos la petición a la API
            var respuesta = await cliente.GetAsync(endpoint);

            if (respuesta.IsSuccessStatusCode)
            {
                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // Mapeamos el JSON a tu lista de UbicacionResponse
                Ubicaciones = JsonSerializer.Deserialize<List<UbicacionResponse>>(resultado, opciones) ?? new List<UbicacionResponse>();
            }
        }
    }
    // Aseguramos que la clase auxiliar esté disponible si no la has hecho global
    public class MetodoConfiguracion
    {
        public string Nombre { get; set; }
        public string Valor { get; set; }
    }
}
