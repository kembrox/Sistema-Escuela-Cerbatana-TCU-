using Abstracciones.Modelos;

namespace Abstracciones.Interfaces.Reglas
{
    public interface IImportadorExcelHelper
    {
        // Genera la plantilla vacía con las columnas exactas de tu escuela
        byte[] GenerarPlantillaVacia();

        // Lee el Excel, usando la lista de ubicaciones del sistema para traducir nombres a GUIDs,
        // y le asigna la categoría seleccionada por el usuario en la web.
        IEnumerable<InventarioResponse> LeerActivosDeExcel(
            Stream archivoExcel,
            IEnumerable<UbicacionResponse> ubicacionesSistema,
            Guid idCategoriaSeleccionada
            );
    }
}
