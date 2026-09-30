using System.Net.Http.Headers;
using System.Text.Json;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Inventario.Web.Pages.Categorias
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuracion;
        private readonly IHttpClientFactory _httpClientFactory;

        // Propiedad que almacenará la lista de categorías para mostrar en el HTML
        public IList<CategoriaResponse> Categorias { get; set; } = default!;

        public IndexModel(IConfiguration configuracion, IHttpClientFactory httpClientFactory)
        {
            _configuracion = configuracion;
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            // 1. Obtenemos la ruta de Categorias desde el appsettings.json
            var metodos = _configuracion.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>();
            var endpoint = metodos?.FirstOrDefault(m => m.Nombre == "ObtenerCategorias")?.Valor;

            // 2. Preparamos el cliente HTTP usando nuestra configuración base
            var cliente = _httpClientFactory.CreateClient("InventarioAPI");

            // 3. Extraemos el Token de la sesión web del usuario actual
            var token = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                // Inyectamos el token en la cabecera para que la API nos permita pasar
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // 4. Hacemos la petición a la API
            var respuesta = await cliente.GetAsync(endpoint);

            if (respuesta.IsSuccessStatusCode)
            {
                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // Convertimos el JSON que responde la API en una lista de C#
                Categorias = JsonSerializer.Deserialize<List<CategoriaResponse>>(resultado, opciones) ?? new List<CategoriaResponse>();
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
