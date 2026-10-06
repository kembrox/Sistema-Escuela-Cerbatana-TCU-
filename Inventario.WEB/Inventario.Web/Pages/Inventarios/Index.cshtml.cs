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
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuracion;
        private readonly IHttpClientFactory _httpClientFactory;

        public IList<InventarioResponse> Inventarios { get; set; } = default!;

        // --- Filtros actuales ---
        [BindProperty(SupportsGet = true)]
        public string? BuscarTexto { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? BuscarCategoria { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? BuscarUbicacion { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool? BuscarEstado { get; set; }

        // --- NUEVO: Variables de Paginación ---
        [BindProperty(SupportsGet = true)]
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        private readonly int RegistrosPorPagina = 12;

        public List<SelectListItem> ListaCategorias { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListaUbicaciones { get; set; } = new List<SelectListItem>();

        public IndexModel(IConfiguration configuracion, IHttpClientFactory httpClientFactory)
        {
            _configuracion = configuracion;
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            var metodos = _configuracion.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>();
            var cliente = _httpClientFactory.CreateClient("InventarioAPI");
            var token = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Cargar listas desplegables
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

            // Descargar Inventarios y filtrar
            var endpoint = metodos?.FirstOrDefault(m => m.Nombre == "ObtenerInventarios")?.Valor;
            if (!string.IsNullOrEmpty(endpoint))
            {
                var respuesta = await cliente.GetAsync(endpoint);
                if (respuesta.IsSuccessStatusCode)
                {
                    var resultado = await respuesta.Content.ReadAsStringAsync();
                    var datosCompletos = JsonSerializer.Deserialize<List<InventarioResponse>>(resultado, opciones) ?? new List<InventarioResponse>();

                    var consulta = datosCompletos.AsEnumerable();

                    // Aplicar Filtros (igual que antes)
                    if (!string.IsNullOrEmpty(BuscarTexto))
                    {
                        consulta = consulta.Where(i =>
                            (i.CodigoFisico != null && i.CodigoFisico.Contains(BuscarTexto, StringComparison.OrdinalIgnoreCase)) ||
                            (i.Descripcion != null && i.Descripcion.Contains(BuscarTexto, StringComparison.OrdinalIgnoreCase)) ||
                            (i.Marca != null && i.Marca.Contains(BuscarTexto, StringComparison.OrdinalIgnoreCase)) ||
                            (i.Modelo != null && i.Modelo.Contains(BuscarTexto, StringComparison.OrdinalIgnoreCase))
                        );
                    }

                    if (BuscarCategoria.HasValue && BuscarCategoria.Value != Guid.Empty)
                        consulta = consulta.Where(i => i.IdCategoria == BuscarCategoria.Value);

                    if (BuscarUbicacion.HasValue && BuscarUbicacion.Value != Guid.Empty)
                        consulta = consulta.Where(i => i.IdUbicacion == BuscarUbicacion.Value);

                    if (BuscarEstado.HasValue)
                        consulta = consulta.Where(i => i.Estado == BuscarEstado.Value);

                    // --- NUEVO: Lógica de Paginación ---
                    // 1. Contamos cuántos registros quedaron después de filtrar
                    int totalRegistros = consulta.Count();

                    // 2. Calculamos las páginas totales
                    TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)RegistrosPorPagina);

                    // Prevención de errores si manipulan la URL
                    if (PaginaActual < 1) PaginaActual = 1;
                    if (PaginaActual > TotalPaginas && TotalPaginas > 0) PaginaActual = TotalPaginas;

                    // 3. Aplicamos Skip y Take para obtener solo los de esta página
                    Inventarios = consulta
                        .Skip((PaginaActual - 1) * RegistrosPorPagina)
                        .Take(RegistrosPorPagina)
                        .ToList();
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
