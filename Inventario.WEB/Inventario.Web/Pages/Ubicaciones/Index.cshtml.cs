using System.Net.Http.Headers;
using System.Text.Json;
using Abstracciones.Modelos; // Importamos tu clase UbicacionResponse
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventario.Web.Pages.Ubicaciones
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuracion;
        private readonly IHttpClientFactory _httpClientFactory;

        public IList<UbicacionResponse> Ubicaciones { get; set; } = default!;

        // 1. Propiedades para capturar los filtros desde la URL
        [BindProperty(SupportsGet = true)]
        public string? BuscarTexto { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool? BuscarEstado { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? BuscarTipoArea { get; set; }

        // Lista para armar el menú desplegable de áreas dinámicamente
        public List<SelectListItem> TiposDeArea { get; set; } = new List<SelectListItem>();

        public IndexModel(IConfiguration configuracion, IHttpClientFactory httpClientFactory)
        {
            _configuracion = configuracion;
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            var metodos = _configuracion.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>();
            var endpoint = metodos?.FirstOrDefault(m => m.Nombre == "ObtenerUbicaciones")?.Valor;

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

                // Obtenemos la lista completa sin filtrar
                var datosCompletos = JsonSerializer.Deserialize<List<UbicacionResponse>>(resultado, opciones) ?? new List<UbicacionResponse>();

                // 2. Llenar el menú desplegable de Tipos de Área con los datos existentes
                TiposDeArea = datosCompletos
                    .Where(u => !string.IsNullOrEmpty(u.TipoArea))
                    .Select(u => u.TipoArea!)
                    .Distinct()
                    .Select(t => new SelectListItem { Value = t, Text = t })
                    .ToList();

                // 3. Aplicar filtros usando LINQ
                var consulta = datosCompletos.AsEnumerable();

                // Si el usuario escribió algo, filtramos por nombre
                if (!string.IsNullOrEmpty(BuscarTexto))
                {
                    consulta = consulta.Where(u => u.Nombre != null && u.Nombre.Contains(BuscarTexto, StringComparison.OrdinalIgnoreCase));
                }

                // Si el usuario seleccionó un estado (y no dejó la opción "Todos"), filtramos
                if (BuscarEstado.HasValue)
                {
                    consulta = consulta.Where(u => u.Estado == BuscarEstado.Value);
                }

                // Si el usuario seleccionó un área, filtramos
                if (!string.IsNullOrEmpty(BuscarTipoArea))
                {
                    consulta = consulta.Where(u => u.TipoArea == BuscarTipoArea);
                }

                // Finalmente, guardamos el resultado filtrado para mandarlo al HTML
                Ubicaciones = consulta.ToList();
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
