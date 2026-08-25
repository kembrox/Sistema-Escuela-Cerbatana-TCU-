using System.Security.Claims;
using Abstracciones.Interfaces.Flujo.Seguridad;
using Abstracciones.Modelos.Seguridad;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Autorizacion.Middleware
{
    public class ClaimsPerfiles
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        // Utilizamos nuestra interfaz de Flujo en lugar de la del profesor
        private ISeguridadFlujo _seguridadFlujo;

        public ClaimsPerfiles(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext httpContext, ISeguridadFlujo seguridadFlujo)
        {
            _seguridadFlujo = seguridadFlujo;

            ClaimsIdentity appIdentity = await verificarAutorizacion(httpContext);
            httpContext.User.AddIdentity(appIdentity);

            await _next(httpContext);
        }

        private async Task<ClaimsIdentity> verificarAutorizacion(HttpContext httpContext)
        {
            var claims = new List<Claim>();

            // Verificamos que el usuario venga autenticado en la petición
            if (httpContext.User != null && httpContext.User.Identity != null && httpContext.User.Identity.IsAuthenticated)
            {
                await ObtenerUsuario(httpContext, claims);
                await ObtenerPerfiles(httpContext, claims);
            }

            var appIdentity = new ClaimsIdentity(claims);
            return appIdentity;
        }

        private async Task ObtenerUsuario(HttpContext httpContext, List<Claim> claims)
        {
            var usuario = await obtenerInformacionUsuario(httpContext);

            // Validamos que el usuario exista y tenga los datos requeridos
            if (usuario != null && !string.IsNullOrEmpty(usuario.Id.ToString()) && !string.IsNullOrEmpty(usuario.NombreUsuario) && !string.IsNullOrEmpty(usuario.CorreoElectronico))
            {
                claims.Add(new Claim(ClaimTypes.Email, usuario.CorreoElectronico));
                claims.Add(new Claim(ClaimTypes.Name, usuario.NombreUsuario));
                claims.Add(new Claim("IdUsuario", usuario.Id.ToString()));
            }
        }

        private async Task<Usuario> obtenerInformacionUsuario(HttpContext httpContext)
        {
            // Extraemos el claim "usuario" que debió enviarse en el token o cookie
            var nombreUsuario = httpContext.User.Claims.Where(c => c.Type == "usuario").FirstOrDefault()?.Value;

            return await _seguridadFlujo.ObtenerUsuario(new Usuario { NombreUsuario = nombreUsuario });
        }

        private async Task ObtenerPerfiles(HttpContext httpContext, List<Claim> claims)
        {
            var perfiles = await obtenerInformacionPerfiles(httpContext);

            if (perfiles != null && perfiles.Any())
            {
                foreach (var perfil in perfiles)
                {
                    // Agregamos cada perfil como un Rol de seguridad
                    claims.Add(new Claim(ClaimTypes.Role, perfil.Id.ToString()));
                }
            }
        }

        private async Task<IEnumerable<Perfil>> obtenerInformacionPerfiles(HttpContext httpContext)
        {
            var nombreUsuario = httpContext.User.Claims.Where(c => c.Type == "usuario").FirstOrDefault()?.Value;

            return await _seguridadFlujo.ObtenerPerfilesxUsuario(new Usuario { NombreUsuario = nombreUsuario });
        }
    }
}
