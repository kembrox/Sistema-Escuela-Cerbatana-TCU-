using System.Net.Http.Headers;
using System.Text.Json;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventario.Web.Pages.Inventarios
{
    [Authorize]
    public class AgregarModel : PageModel
    {
        private readonly IConfiguration _config; private readonly IHttpClientFactory _http;
        [BindProperty] public InventarioBase Inventario { get; set; } = new InventarioBase { Estado = true };
        public List<SelectListItem> ListaCategorias { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListaUbicaciones { get; set; } = new List<SelectListItem>();

        public AgregarModel(IConfiguration config, IHttpClientFactory http) { _config = config; _http = http; }

        public async Task<IActionResult> OnGetAsync()
        {
            await CargarListasDesplegables();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) { await CargarListasDesplegables(); return Page(); }

            var endpoint = _config.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>()?.FirstOrDefault(m => m.Nombre == "AgregarInventario")?.Valor;
            var cliente = CrearClienteAutorizado();

            var respuesta = await cliente.PostAsJsonAsync(endpoint, Inventario);
            if (respuesta.IsSuccessStatusCode) return RedirectToPage("./Index");

            ModelState.AddModelError(string.Empty, "Error al guardar.");
            await CargarListasDesplegables();
            return Page();
        }

        private async Task CargarListasDesplegables()
        {
            var cliente = CrearClienteAutorizado();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var metodos = _config.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>();

            // Cargar Categorías
            var endCat = metodos?.FirstOrDefault(m => m.Nombre == "ObtenerCategorias")?.Valor;
            var resCat = await cliente.GetAsync(endCat);
            if (resCat.IsSuccessStatusCode)
            {
                var catData = JsonSerializer.Deserialize<List<CategoriaResponse>>(await resCat.Content.ReadAsStringAsync(), opciones);
                ListaCategorias = catData.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nombre }).ToList();
            }

            // Cargar Ubicaciones
            var endUbi = metodos?.FirstOrDefault(m => m.Nombre == "ObtenerUbicaciones")?.Valor;
            var resUbi = await cliente.GetAsync(endUbi);
            if (resUbi.IsSuccessStatusCode)
            {
                var ubiData = JsonSerializer.Deserialize<List<UbicacionResponse>>(await resUbi.Content.ReadAsStringAsync(), opciones);
                ListaUbicaciones = ubiData.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Nombre }).ToList();
            }
        }

        private HttpClient CrearClienteAutorizado()
        {
            var cliente = _http.CreateClient("InventarioAPI");
            var token = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            if (!string.IsNullOrEmpty(token)) cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return cliente;
        }
    }
}
