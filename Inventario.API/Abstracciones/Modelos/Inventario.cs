using System.ComponentModel.DataAnnotations;

namespace Abstracciones.Modelos
{
    public class InventarioBase
    {
        [Required(ErrorMessage = "El código físico del activo es requerido.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El código físico debe tener entre 3 y 50 caracteres.")]
        public string CodigoFisico { get; set; }

        [Required(ErrorMessage = "La descripción del activo es requerida.")]
        [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La categoría es requerida.")]
        public Guid IdCategoria { get; set; }

        [Required(ErrorMessage = "La ubicación es requerida.")]
        public Guid IdUbicacion { get; set; }

        [Required(ErrorMessage = "El estado es requerido.")]
        public bool Estado { get; set; }

        public string? Observaciones { get; set; }

        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? Serie { get; set; }
        public decimal? Precio { get; set; }

        public Guid? UsuarioRegistra { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public Guid? UsuarioModifica { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }

    public class InventarioResponse : InventarioBase {
       
        public Guid Id { get; set; }

    }
}
