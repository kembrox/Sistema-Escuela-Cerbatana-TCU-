using System.Net.Http.Headers;
using System.Text.Json;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Inventario.Web.Pages.Inventarios
{
    [Authorize]
    public class EliminarModel : PageModel
    {
        private readonly IConfiguration _config; private readonly IHttpClientFactory _http;
        [BindProperty] public InventarioResponse Activo { get; set; } = default!;

        public EliminarModel(IConfiguration config, IHttpClientFactory http) { _config = config; _http = http; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();
            var end = _config.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>()?.FirstOrDefault(m => m.Nombre == "ObtenerInventarioPorId")?.Valor;
            var cliente = _http.CreateClient("InventarioAPI");
            var token = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            if (!string.IsNullOrEmpty(token)) cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await cliente.GetAsync(string.Format(end, id));
            if (res.IsSuccessStatusCode)
            {
                Activo = JsonSerializer.Deserialize<InventarioResponse>(await res.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return Page();
            }
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Activo.Id == Guid.Empty) return NotFound();
            var end = _config.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>()?.FirstOrDefault(m => m.Nombre == "EliminarInventario")?.Valor;
            var cliente = _http.CreateClient("InventarioAPI");
            var token = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            if (!string.IsNullOrEmpty(token)) cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await cliente.DeleteAsync(string.Format(end, Activo.Id));
            if (res.IsSuccessStatusCode) return RedirectToPage("./Index");
            return Page();
        }
    }
}
