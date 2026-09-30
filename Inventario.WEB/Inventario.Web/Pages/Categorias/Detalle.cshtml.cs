using System.Net.Http.Headers;
using System.Text.Json;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Inventario.Web.Pages.Categorias
{
    [Authorize]
    public class DetalleModel : PageModel
    {
        private readonly IConfiguration _configuracion;
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public CategoriaResponse Categoria { get; set; } = default!;

        public DetalleModel(IConfiguration configuracion, IHttpClientFactory httpClientFactory)
        {
            _configuracion = configuracion;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            // Si alguien intenta entrar a la página sin enviar un ID, lo devolvemos a la lista
            if (id == null)
            {
                return RedirectToPage("./Index");
            }

            // 1. Buscamos el endpoint en el appsettings.json
            var metodos = _configuracion.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>();
            var endpoint = metodos?.FirstOrDefault(m => m.Nombre == "ObtenerCategoriaPorId")?.Valor;

            // 2. Preparamos el cliente HTTP
            var cliente = _httpClientFactory.CreateClient("InventarioAPI");

            // 3. Agregamos el Token de seguridad
            var token = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // 4. Formateamos la URL (cambiamos Categoria/{0} por Categoria/ID-REAL)
            var urlFinal = string.Format(endpoint, id);
            var solicitud = new HttpRequestMessage(HttpMethod.Get, urlFinal);

            // 5. Enviamos la petición
            var respuesta = await cliente.SendAsync(solicitud);

            if (respuesta.IsSuccessStatusCode)
            {
                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // Transformamos el JSON recibido a nuestro objeto C#
                Categoria = JsonSerializer.Deserialize<CategoriaResponse>(resultado, opciones);
                return Page();
            }

            // Si la API no encuentra la categoría (ej. un error 404), regresamos al inicio
            return RedirectToPage("./Index");
        }
    }
}
