using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace Abstracciones.Interfaces.API
{
    public interface IInventarioController
    {
        Task<IActionResult> Obtener();
        Task<IActionResult> Obtener(Guid Id);
        Task<IActionResult> Agregar(InventarioResponse inventario);
        Task<IActionResult> Editar(Guid Id, InventarioResponse inventario);
        Task<IActionResult> Eliminar(Guid Id);
    }
}
