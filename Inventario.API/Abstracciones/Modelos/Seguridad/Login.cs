using System.ComponentModel.DataAnnotations;

namespace Abstracciones.Modelos.Seguridad
{
    public class Login
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string NombreUsuario { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        [EmailAddress]
        public string CorreoElectronico { get; set; }
        public Guid IdServicio { get; set; }
    }
}
