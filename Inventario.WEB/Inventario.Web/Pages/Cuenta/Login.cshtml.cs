using System.Security.Claims;
using System.Text.Json;
using Abstracciones.Modelos.Seguridad;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reglas;

namespace Inventario.Web.Pages.Cuenta
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Login LoginInfo { get; set; } = new Login();

        public Token TokenRespuesta { get; set; } = default!;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuracion;

        public LoginModel(IHttpClientFactory httpClientFactory, IConfiguration configuracion)
        {
            _httpClientFactory = httpClientFactory;
            _configuracion = configuracion;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // SOLUCIÓN 1: Le decimos a ASP.NET que no marque error si NombreUsuario viene vacío desde el HTML,
            // ya que nosotros lo vamos a calcular manualmente un paso más abajo.
            ModelState.Remove("LoginInfo.NombreUsuario");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 1. Extraemos el nombre de usuario del correo (ej: kquesada@... -> kquesada)
            if (!string.IsNullOrEmpty(LoginInfo.CorreoElectronico))
            {
                LoginInfo.NombreUsuario = LoginInfo.CorreoElectronico.Split('@')[0];
            }

            // 2. Buscamos el endpoint de Login en tu appsettings.json
            var metodos = _configuracion.GetSection("ApiEndPoints:Metodos").Get<List<MetodoConfiguracion>>();
            var endpointLogin = metodos?.FirstOrDefault(m => m.Nombre == "Login")?.Valor;

            if (string.IsNullOrEmpty(endpointLogin))
            {
                ModelState.AddModelError(string.Empty, "Error de configuración de API.");
                return Page();
            }

            // 3. Enviamos la petición a la API de forma segura
            var cliente = _httpClientFactory.CreateClient("InventarioAPI");
            var respuesta = await cliente.PostAsJsonAsync(endpointLogin, LoginInfo);

            if (respuesta.IsSuccessStatusCode)
            {
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                TokenRespuesta = JsonSerializer.Deserialize<Token>(await respuesta.Content.ReadAsStringAsync(), opciones);

                // 4. Si la API aprueba el acceso, creamos la sesión web
                if (TokenRespuesta != null && TokenRespuesta.ValidacionExitosa)
                {
                    var jwtToken = Autenticacion.LeerToken(TokenRespuesta.AccessToken);
                    var claims = Autenticacion.GenerarClaims(jwtToken, TokenRespuesta.AccessToken);

                    await EstablecerAutenticacion(claims);

                    // SOLUCIÓN 2: Redirigimos correctamente a la carpeta Inventarios
                    return RedirectToPage("/Index");
                }
            }

            // Si llega aquí, es porque la contraseña era incorrecta o hubo un error
            TokenRespuesta = new Token { ValidacionExitosa = false };
            return Page();
        }

        private async Task EstablecerAutenticacion(List<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}