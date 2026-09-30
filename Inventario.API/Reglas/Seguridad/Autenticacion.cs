using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Reglas
{
    public static class Autenticacion
    {
        
        public static JwtSecurityToken? LeerToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            return jsonToken;
        }

        
        public static List<Claim> GenerarClaims(JwtSecurityToken? jwtToken, string accessToken)
        {
            var claims = new List<Claim>();

           
            var usuarioClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "usuario");
            if (usuarioClaim != null)
            {
                claims.Add(new Claim(ClaimTypes.Name, usuarioClaim.Value));
            }

            
            var rolesClaims = jwtToken?.Claims.Where(c => c.Type == ClaimTypes.Role);
            if (rolesClaims != null)
            {
                foreach (var rol in rolesClaims)
                {
                    claims.Add(new Claim(ClaimTypes.Role, rol.Value));
                }
            }

            
            claims.Add(new Claim("Token", accessToken));

            return claims;
        }
    }
}
