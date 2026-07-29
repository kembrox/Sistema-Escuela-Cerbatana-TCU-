using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace Abstracciones.Interfaces.API
{
    public interface IUbicacionController
    {
        Task<IActionResult> Obtener();
        Task<IActionResult> Obtener(Guid Id);
        Task<IActionResult> Agregar(UbicacionResponse ubicacion);
        Task<IActionResult> Editar(Guid Id, UbicacionResponse ubicacion);
        Task<IActionResult> Eliminar(Guid Id);
    }
}
