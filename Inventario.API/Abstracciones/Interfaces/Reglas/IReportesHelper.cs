using Abstracciones.Modelos;

namespace Abstracciones.Interfaces.Reglas
{
    public interface IReportesHelper
    {
        byte[] GenerarExcelInventario(IEnumerable<InventarioResponse> activos);

        byte[] GenerarPdfInventario(IEnumerable<InventarioResponse> activos);
    }
}
