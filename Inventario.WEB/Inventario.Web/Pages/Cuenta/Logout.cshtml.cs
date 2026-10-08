using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Inventario.Web.Pages.Cuenta
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            // Esta es la instrucción mágica que destruye la Cookie de seguridad de ASP.NET Core
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Una vez destruida la sesión, redirigimos al usuario a la pantalla de inicio
            return RedirectToPage("/Index");
        }
    }
}