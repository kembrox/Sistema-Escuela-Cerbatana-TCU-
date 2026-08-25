using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Modelos.Seguridad
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string NombreUsuario { get; set; }
        public string PasswordHash { get; set; }
        public string CorreoElectronico { get; set; }


        // Lista de perfiles asignados a este usuario
        public IEnumerable<Perfil> Perfiles { get; set; } = new List<Perfil>();
    }
}
