using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abstracciones.Modelos;

namespace Abstracciones.Interfaces.Reglas
{
    public interface IReportesHelper
    {
        byte[] GenerarExcelInventario(IEnumerable<InventarioResponse> activos);

        byte[] GenerarPdfInventario(IEnumerable<InventarioResponse> activos);
    }
}
