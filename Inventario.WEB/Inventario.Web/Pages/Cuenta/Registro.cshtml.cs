using Abstracciones.Modelos.Seguridad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Inventario.Web.Pages.Cuenta
{
    public class RegistroModel : PageModel
    {
        [BindProperty]
        public Usuario UsuarioFormulario { get; set; } = new Usuario();

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuracion;

        // Inyectamos las herramientas para leer el appsettings y hacer peticiones web
        public RegistroModel(IHttpClientFactory httpClientFactory, IConfiguration configuracion)
        {
            _httpClientFactory = httpClientFactory;
            _configuracion = configuracion;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 1. Buscamos la ruta "RegistrarUsuario" en tu appsettings.json
            var metodos = _configuracion.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>();
            var endpointRegistro = metodos?.FirstOrDefault(m => m.Nombre == "RegistrarUsuario")?.Valor;

            if (string.IsNullOrEmpty(endpointRegistro))
            {
                ModelState.AddModelError(string.Empty, "Error de configuración: No se encontró la ruta del API.");
                return Page();
            }

            // 2. Usamos el cliente que apunta a https://localhost:7173/api/
            var cliente = _httpClientFactory.CreateClient("InventarioAPI");

            // 3. Enviamos el usuario (la API se encargará del BCrypt)
            var respuesta = await cliente.PostAsJsonAsync(endpointRegistro, UsuarioFormulario);

            if (respuesta.IsSuccessStatusCode)
            {
                // Si el API devuelve 200 OK, lo enviamos a iniciar sesión
                return RedirectToPage("Login");
            }

            ModelState.AddModelError(string.Empty, "Ocurrió un error al registrar el usuario en la base de datos.");
            return Page();
        }
    }

    // Clase auxiliar para mapear la estructura de tu appsettings.json
    public class MetodoConfiguracion
    {
        public string Nombre { get; set; }
        public string Valor { get; set; }
    }
}

