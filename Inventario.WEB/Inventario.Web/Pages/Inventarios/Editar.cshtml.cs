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
        private readonly IConfiguration _config; private readonly IHttpClientFactory _http;
        [BindProperty] public InventarioResponse Inventario { get; set; } = default!;
        public List<SelectListItem> ListaCategorias { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListaUbicaciones { get; set; } = new List<SelectListItem>();

        public EditarModel(IConfiguration config, IHttpClientFactory http) { _config = config; _http = http; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();
            var end = _config.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>()?.FirstOrDefault(m => m.Nombre == "ObtenerInventarioPorId")?.Valor;
            var cliente = CrearClienteAutorizado();

            var res = await cliente.GetAsync(string.Format(end, id));
            if (res.IsSuccessStatusCode)
            {
                Inventario = JsonSerializer.Deserialize<InventarioResponse>(await res.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                await CargarListasDesplegables();
                return Page();
            }
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) { await CargarListasDesplegables(); return Page(); }
            var end = _config.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>()?.FirstOrDefault(m => m.Nombre == "EditarInventario")?.Valor;
            var cliente = CrearClienteAutorizado();

            var res = await cliente.PutAsJsonAsync(string.Format(end, Inventario.Id), Inventario);
            if (res.IsSuccessStatusCode) return RedirectToPage("./Index");

            ModelState.AddModelError(string.Empty, "Error al actualizar.");
            await CargarListasDesplegables();
            return Page();
        }

        private async Task CargarListasDesplegables() { /* Mismo código que en AgregarModel para cargar selects */ }
        private HttpClient CrearClienteAutorizado() { /* Mismo código que en AgregarModel */ return _http.CreateClient("InventarioAPI"); } // Asegúrate de agregar los headers aquí.
    }
}
