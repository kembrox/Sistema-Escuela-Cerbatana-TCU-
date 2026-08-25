namespace Abstracciones.Entidades.Seguridad
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string NombreUsuario { get; set; }
        public string PasswordHash { get; set; }
        public string CorreoElectronico { get; set; }

        // Campos de auditoría reflejando la base de datos (permiten NULL)
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public Guid? UsuarioCrea { get; set; }
        public Guid? UsuarioModifica { get; set; }

    }
}
