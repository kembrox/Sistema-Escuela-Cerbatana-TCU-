using System.Formats.Asn1;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Abstracciones.Interfaces.DA.Seguridad;
using Abstracciones.Interfaces.Reglas.Seguridad;
using Abstracciones.Modelos.Seguridad;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

namespace Reglas.Seguridad
{
    public class AutenticacionReglas : IAutenticacionReglas
    {
        public IConfiguration _configuration;
        public ISeguridadDA _seguridadDA;

        public AutenticacionReglas(IConfiguration configuration, ISeguridadDA seguridadDA)
        {
            _configuration = configuration;
            _seguridadDA = seguridadDA;
        }

        public async Task<Token> LoginAsync(Login login)
        {
            Token respuestaToken = new Token() {AccessToken = string.Empty, ValidacionExitosa = false};
            var resultadoVerificacionCrendenciales = await VerificarLoginAsync(login);
            if (!resultadoVerificacionCrendenciales)
                return respuestaToken;
            TokenConfiguracion tokenConfiguracion = _configuration.GetSection("Token").Get<TokenConfiguracion>();
            JwtSecurityToken token = await GenerarTokenJWT(login, tokenConfiguracion);
            respuestaToken.AccessToken = new JwtSecurityTokenHandler().WriteToken(token);
            respuestaToken.ValidacionExitosa = true;
            return respuestaToken;
        }

        private async Task<bool> VerificarLoginAsync(Login login)
        {
            if (login == null)
            {
                return false;
            }
            var usuario  = await _seguridadDA.ObtenerUsuario(new Usuario { NombreUsuario = login.NombreUsuario, CorreoElectronico = login.CorreoElectronico});
            if (usuario == null)
            {
                return false;
            }
            bool contraseñaCorrecta = BCrypt.Net.BCrypt.Verify(login.PasswordHash, usuario.PasswordHash);

            return contraseñaCorrecta;
        }

        private async Task<JwtSecurityToken> GenerarTokenJWT(Login login, TokenConfiguracion tokenConfiguracion)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfiguracion.key));
            List<Claim> claims = await GenerarClaims(login);
            var credentials =  new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(tokenConfiguracion.Issuer, tokenConfiguracion.Audience, claims, expires: DateTime.Now.AddMinutes(tokenConfiguracion.Expires), signingCredentials: credentials);
            return token;
        }

        private async Task<List<Claim>> GenerarClaims(Login login)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("usuario", login.NombreUsuario));
            claims.Add(new Claim("servicio", login.IdServicio.ToString()));

            var perfiles = await ObtenerPerfiles(login);
            
            {
                foreach (var perfil in perfiles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, perfil.Id.ToString()));
                }
            }

            return claims;
        }

        private async Task<IEnumerable<Perfil>> ObtenerPerfiles(Login login)
        {
            
            return await _seguridadDA.ObtenerPerfilesxUsuario(new Usuario
            {
                NombreUsuario = login.NombreUsuario,
                CorreoElectronico = login.CorreoElectronico
            });
        }
    }
}
