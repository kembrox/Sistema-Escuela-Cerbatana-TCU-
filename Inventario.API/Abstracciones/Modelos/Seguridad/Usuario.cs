using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Modelos.Seguridad
{
    public class Usuario
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


        // Lista de perfiles asignados a este usuario
        public IEnumerable<Perfil> Perfiles { get; set; } = new List<Perfil>();
    }
}
