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
    public class EditarModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _http;

        [BindProperty]
        public InventarioResponse Inventario { get; set; } = default!;

        public List<SelectListItem> ListaCategorias { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListaUbicaciones { get; set; } = new List<SelectListItem>();

        public EditarModel(IConfiguration config, IHttpClientFactory http)
        {
            _config = config;
            _http = http;
        }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();
            var end = _config.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>()?.FirstOrDefault(m => m.Nombre == "ObtenerInventarioPorId")?.Valor;
            var cliente = CrearClienteAutorizado();

            var res = await cliente.GetAsync(string.Format(end, id));
            if (res.IsSuccessStatusCode)
            {
                Inventario = JsonSerializer.Deserialize<InventarioResponse>(await res.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Llamamos a la función que ahora sí tiene lógica
                await CargarListasDesplegables();
                return Page();
            }
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CargarListasDesplegables();
                return Page();
            }

            var end = _config.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>()?.FirstOrDefault(m => m.Nombre == "EditarInventario")?.Valor;
            var cliente = CrearClienteAutorizado();

            var res = await cliente.PutAsJsonAsync(string.Format(end, Inventario.Id), Inventario);
            if (res.IsSuccessStatusCode) return RedirectToPage("./Index");

            ModelState.AddModelError(string.Empty, "Error al actualizar.");
            await CargarListasDesplegables();
            return Page();
        }

        // 1. Método completado para descargar las listas
        private async Task CargarListasDesplegables()
        {
            var metodos = _config.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>();
            var cliente = CrearClienteAutorizado();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Descargar Categorías
            var endCat = metodos?.FirstOrDefault(m => m.Nombre == "ObtenerCategorias")?.Valor;
            if (!string.IsNullOrEmpty(endCat))
            {
                var resCat = await cliente.GetAsync(endCat);
                if (resCat.IsSuccessStatusCode)
                {
                    var catData = JsonSerializer.Deserialize<List<CategoriaResponse>>(await resCat.Content.ReadAsStringAsync(), opciones);
                    if (catData != null)
                        ListaCategorias = catData.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nombre }).ToList();
                }
            }

            // Descargar Ubicaciones
            var endUbi = metodos?.FirstOrDefault(m => m.Nombre == "ObtenerUbicaciones")?.Valor;
            if (!string.IsNullOrEmpty(endUbi))
            {
                var resUbi = await cliente.GetAsync(endUbi);
                if (resUbi.IsSuccessStatusCode)
                {
                    var ubiData = JsonSerializer.Deserialize<List<UbicacionResponse>>(await resUbi.Content.ReadAsStringAsync(), opciones);
                    if (ubiData != null)
                        ListaUbicaciones = ubiData.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Nombre }).ToList();
                }
            }
        }

        // 2. Método completado para inyectar la seguridad
        private HttpClient CrearClienteAutorizado()
        {
            var cliente = _http.CreateClient("InventarioAPI");
            var token = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return cliente;
        }
    }
}