using System.Net.Http.Headers;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Inventario.Web.Pages.Categorias
{
    [Authorize]
    public class AgregarModel : PageModel
    {
        private readonly IConfiguration _configuracion;
        private readonly IHttpClientFactory _httpClientFactory;

        // Propiedad enlazada al formulario HTML
        [BindProperty]
        public CategoriaResponse Categoria { get; set; } = new CategoriaResponse();

        public AgregarModel(IConfiguration configuracion, IHttpClientFactory httpClientFactory)
        {
            _configuracion = configuracion;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult OnGet()
        {
            // Solo mostramos la página vacía la primera vez que se carga
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 1. Obtenemos el endpoint desde appsettings.json
            var metodos = _configuracion.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>();
            var endpoint = metodos?.FirstOrDefault(m => m.Nombre == "AgregarCategoria")?.Valor;

            // 2. Preparamos el cliente HTTP
            var cliente = _httpClientFactory.CreateClient("InventarioAPI");

            // 3. Extraemos el Token de la sesión web y lo adjuntamos
            var token = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // 4. Enviamos la nueva categoría a la API
            var respuesta = await cliente.PostAsJsonAsync(endpoint, Categoria);

            if (respuesta.IsSuccessStatusCode)
            {
                // Si todo sale bien, devolvemos al usuario a la lista principal
                return RedirectToPage("./Index");
            }

            // Si la API rechaza la petición, mostramos un error
            ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar la categoría. Verifique la conexión con el servidor.");
            return Page();
        }
    }
}
