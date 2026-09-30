using System.Net.Http.Headers;
using System.Text.Json;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Inventario.Web.Pages.Categorias
{
    [Authorize]
    public class EliminarModel : PageModel
    {
        private readonly IConfiguration _configuracion;
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public CategoriaResponse Categoria { get; set; } = default!;

        public EliminarModel(IConfiguration configuracion, IHttpClientFactory httpClientFactory)
        {
            _configuracion = configuracion;
            _httpClientFactory = httpClientFactory;
        }

        // Se ejecuta para mostrar los datos de lo que se va a borrar
        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            var metodos = _configuracion.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>();
            var endpoint = metodos?.FirstOrDefault(m => m.Nombre == "ObtenerCategoriaPorId")?.Valor;

            var cliente = _httpClientFactory.CreateClient("InventarioAPI");
            var token = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var solicitud = new HttpRequestMessage(HttpMethod.Get, string.Format(endpoint, id));
            var respuesta = await cliente.SendAsync(solicitud);

            if (respuesta.IsSuccessStatusCode)
            {
                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                Categoria = JsonSerializer.Deserialize<CategoriaResponse>(resultado, opciones);
            }

            if (Categoria == null) return RedirectToPage("./Index");

            return Page();
        }

        // Se ejecuta cuando el usuario presiona el botón rojo
        public async Task<IActionResult> OnPostAsync()
        {
            if (Categoria.Id == Guid.Empty) return NotFound();

            var metodos = _configuracion.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>();
            var endpoint = metodos?.FirstOrDefault(m => m.Nombre == "EliminarCategoria")?.Valor;

            var cliente = _httpClientFactory.CreateClient("InventarioAPI");
            var token = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Usamos HttpMethod.Delete para indicar a la API que debe borrar
            var solicitud = new HttpRequestMessage(HttpMethod.Delete, string.Format(endpoint, Categoria.Id));
            var respuesta = await cliente.SendAsync(solicitud);

            if (respuesta.IsSuccessStatusCode)
            {
                // Si se borró correctamente, devolvemos al listado
                return RedirectToPage("./Index");
            }

            // Si hay un error (ej. la categoría está en uso por un activo), recargamos la página con un error
            ModelState.AddModelError(string.Empty, "No se pudo eliminar la categoría. Es posible que esté en uso.");
            return Page();
        }
    }
}
