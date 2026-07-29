using System.ComponentModel.DataAnnotations;

namespace Abstracciones.Modelos
{
    public class UbicacionBase
    {
        [Required(ErrorMessage = "El nombre de la ubicación es requerido.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string Nombre { get; set; }

        
        [MaxLength(100, ErrorMessage = "El tipo de área no puede superar los 100 caracteres.")]
        public string? TipoArea { get; set; }

        [Required(ErrorMessage = "El estado es requerido.")]
        public bool Estado { get; set; }
    }

    public class UbicacionResponse : UbicacionBase
    {
        
        public Guid Id { get; set; }
    }
}

