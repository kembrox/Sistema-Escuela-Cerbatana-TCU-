using System.Net.Http.Headers;
using System.Text.Json;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Inventario.Web.Pages
{
    // Etiqueta [Authorize] removida para permitir el acceso a la pantalla de bienvenida pública
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuracion;
        private readonly IHttpClientFactory _httpClientFactory;

        public int TotalInventarios { get; set; } = 0;
        public int TotalCategorias { get; set; } = 0;
        public int TotalUbicaciones { get; set; } = 0;
        public int ActivosFuncionales { get; set; } = 0;

        public IndexModel(IConfiguration configuracion, IHttpClientFactory httpClientFactory)
        {
            _configuracion = configuracion;
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            // Solo descargamos la información del TCU si el usuario tiene una sesión activa
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var metodos = _configuracion.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>();
                var cliente = _httpClientFactory.CreateClient("InventarioAPI");
                var token = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

                if (!string.IsNullOrEmpty(token))
                {
                    cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var endInv = metodos?.FirstOrDefault(m => m.Nombre == "ObtenerInventarios")?.Valor;
                if (!string.IsNullOrEmpty(endInv))
                {
                    var resInv = await cliente.GetAsync(endInv);
                    if (resInv.IsSuccessStatusCode)
                    {
                        var lista = JsonSerializer.Deserialize<List<InventarioResponse>>(await resInv.Content.ReadAsStringAsync(), opciones) ?? new List<InventarioResponse>();
                        TotalInventarios = lista.Count();
                        ActivosFuncionales = lista.Count(i => i.Estado == true);
                    }
                }

                var endCat = metodos?.FirstOrDefault(m => m.Nombre == "ObtenerCategorias")?.Valor;
                if (!string.IsNullOrEmpty(endCat))
                {
                    var resCat = await cliente.GetAsync(endCat);
                    if (resCat.IsSuccessStatusCode)
                    {
                        var listaCat = JsonSerializer.Deserialize<List<CategoriaResponse>>(await resCat.Content.ReadAsStringAsync(), opciones) ?? new List<CategoriaResponse>();
                        TotalCategorias = listaCat.Count();
                    }
                }

                var endUbi = metodos?.FirstOrDefault(m => m.Nombre == "ObtenerUbicaciones")?.Valor;
                if (!string.IsNullOrEmpty(endUbi))
                {
                    var resUbi = await cliente.GetAsync(endUbi);
                    if (resUbi.IsSuccessStatusCode)
                    {
                        var listaUbi = JsonSerializer.Deserialize<List<UbicacionResponse>>(await resUbi.Content.ReadAsStringAsync(), opciones) ?? new List<UbicacionResponse>();
                        TotalUbicaciones = listaUbi.Count();
                    }
                }
            }
        }
    }

    public class MetodoConfiguracion
    {
        public string Nombre { get; set; }
        public string Valor { get; set; }
    }
}