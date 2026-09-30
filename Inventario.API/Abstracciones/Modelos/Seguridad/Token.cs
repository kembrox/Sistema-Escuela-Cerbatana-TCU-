using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;

namespace Abstracciones.Modelos.Seguridad
{
    public class Token
    {
        public bool ValidacionExitosa {  get; set; }   
        public string AccessToken { get; set; }

    }

    public class TokenConfiguracion
    {
        [Required]
        [StringLength(100, MinimumLength =32)]
        public string key {  get; set; }
        [Required]
        public string Issuer { get; set; }
        [Required]
        public double Expires { get; set; }
        public string Audience { get; set; }
    }
}
