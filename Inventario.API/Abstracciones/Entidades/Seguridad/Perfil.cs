namespace Abstracciones.Entidades.Seguridad
{
    public class Perfil
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        // Campos de auditoría reflejando la base de datos
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public Guid? UsuarioCrea { get; set; }
        public Guid? UsuarioModifica { get; set; }
    }
}
