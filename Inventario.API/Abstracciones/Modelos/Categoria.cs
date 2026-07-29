using System.ComponentModel.DataAnnotations;

namespace Abstracciones.Modelos
{
    public class CategoriaBase
    {
        [Required(ErrorMessage = "El nombre de la categoría es requerido.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string Nombre { get; set; }

        
        [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El estado es requerido.")]
        public bool Estado { get; set; }
    }

    public class CategoriaResponse : CategoriaBase
    {
        
        public Guid Id { get; set; }
    }
}

