using System.Net.Http.Headers;
using System.Text.Json;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Inventario.Web.Pages.Inventarios
{
    [Authorize]
    public class DetalleModel : PageModel
    {
        private readonly IConfiguration _configuracion;
        private readonly IHttpClientFactory _httpClientFactory;

        public InventarioResponse Activo { get; set; } = default!;

        // 1. Nuevas variables para guardar los nombres traducidos
        public string NombreCategoria { get; set; } = "Desconocida";
        public string NombreUbicacion { get; set; } = "Desconocida";

        public DetalleModel(IConfiguration configuracion, IHttpClientFactory httpClientFactory)
        {
            _configuracion = configuracion;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            var metodos = _configuracion.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>();
            var cliente = _httpClientFactory.CreateClient("InventarioAPI");
            var token = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var endInventario = metodos?.FirstOrDefault(m => m.Nombre == "ObtenerInventarioPorId")?.Valor;
            if (!string.IsNullOrEmpty(endInventario))
            {
                var resInventario = await cliente.GetAsync(string.Format(endInventario, id));
                if (resInventario.IsSuccessStatusCode)
                {
                    // Descargamos el activo principal
                    Activo = JsonSerializer.Deserialize<InventarioResponse>(await resInventario.Content.ReadAsStringAsync(), opciones) ?? new InventarioResponse();

                    // 2. Buscamos el nombre real de la Categoría
                    var endCat = metodos?.FirstOrDefault(m => m.Nombre == "ObtenerCategorias")?.Valor;
                    if (!string.IsNullOrEmpty(endCat))
                    {
                        var resCat = await cliente.GetAsync(endCat);
                        if (resCat.IsSuccessStatusCode)
                        {
                            var categorias = JsonSerializer.Deserialize<List<CategoriaResponse>>(await resCat.Content.ReadAsStringAsync(), opciones);
                            var categoriaEncontrada = categorias?.FirstOrDefault(c => c.Id == Activo.IdCategoria);
                            if (categoriaEncontrada != null) NombreCategoria = categoriaEncontrada.Nombre;
                        }
                    }

                    // 3. Buscamos el nombre real de la Ubicación
                    var endUbi = metodos?.FirstOrDefault(m => m.Nombre == "ObtenerUbicaciones")?.Valor;
                    if (!string.IsNullOrEmpty(endUbi))
                    {
                        var resUbi = await cliente.GetAsync(endUbi);
                        if (resUbi.IsSuccessStatusCode)
                        {
                            var ubicaciones = JsonSerializer.Deserialize<List<UbicacionResponse>>(await resUbi.Content.ReadAsStringAsync(), opciones);
                            var ubicacionEncontrada = ubicaciones?.FirstOrDefault(u => u.Id == Activo.IdUbicacion);
                            if (ubicacionEncontrada != null) NombreUbicacion = ubicacionEncontrada.Nombre;
                        }
                    }

                    return Page();
                }
            }
            return RedirectToPage("./Index");
        }
    }
}
