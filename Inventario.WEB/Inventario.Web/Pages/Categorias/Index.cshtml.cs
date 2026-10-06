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

        // Propiedad que almacenará la lista final (filtrada) para el HTML
        public IList<CategoriaResponse> Categorias { get; set; } = default!;

        // NUEVO: Propiedad para capturar lo que el usuario escribe en la barra de búsqueda
        [BindProperty(SupportsGet = true)]
        public string? BuscarTexto { get; set; }

        public IndexModel(IConfiguration configuracion, IHttpClientFactory httpClientFactory)
        {
            _configuracion = configuracion;
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            var metodos = _configuracion.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>();
            var endpoint = metodos?.FirstOrDefault(m => m.Nombre == "ObtenerCategorias")?.Valor;

            var cliente = _httpClientFactory.CreateClient("InventarioAPI");
            var token = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var respuesta = await cliente.GetAsync(endpoint);

            if (respuesta.IsSuccessStatusCode)
            {
                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // 1. Descargamos TODAS las categorías
                var datosCompletos = JsonSerializer.Deserialize<List<CategoriaResponse>>(resultado, opciones) ?? new List<CategoriaResponse>();

                // 2. Preparamos la consulta LINQ
                var consulta = datosCompletos.AsEnumerable();

                // 3. Aplicamos el filtro si el usuario escribió algo
                if (!string.IsNullOrEmpty(BuscarTexto))
                {
                    // Comparamos ignorando mayúsculas y minúsculas
                    consulta = consulta.Where(c => c.Nombre != null && c.Nombre.Contains(BuscarTexto, StringComparison.OrdinalIgnoreCase));
                }

                // 4. Guardamos el resultado final
                Categorias = consulta.ToList();
            }
        }
    }

    // Clase auxiliar para mapear el appsettings.json
    public class MetodoConfiguracion
    {
        public string Nombre { get; set; }
        public string Valor { get; set; }
    }
}
