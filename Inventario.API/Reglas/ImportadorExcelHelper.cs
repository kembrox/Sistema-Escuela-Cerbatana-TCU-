using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using ClosedXML.Excel;

namespace Reglas
{
    public class ImportadorExcelHelper : IImportadorExcelHelper
    {
        public byte[] GenerarPlantillaVacia()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Plantilla Carga Masiva");

                // Configurar encabezados basados en tu Excel real de la escuela
                worksheet.Cell(1, 1).Value = "No. De Identificación";
                worksheet.Cell(1, 2).Value = "Descripción";
                worksheet.Cell(1, 3).Value = "Marca";
                worksheet.Cell(1, 4).Value = "Modelo";
                worksheet.Cell(1, 5).Value = "Serie";
                worksheet.Cell(1, 6).Value = "Estado (Activo / Inactivo)";
                worksheet.Cell(1, 7).Value = "Ubicación";
                worksheet.Cell(1, 8).Value = "Modo de adquisición";
                worksheet.Cell(1, 9).Value = "Precio";
                worksheet.Cell(1, 10).Value = "Observaciones";

                // Estilo profesional para el encabezado (Azul oscuro)
                var rango = worksheet.Range(1, 1, 1, 10);
                rango.Style.Font.Bold = true;
                rango.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F497D");
                rango.Style.Font.FontColor = XLColor.White;
                rango.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Fila de ejemplo en gris itálico para guiar al usuario
                worksheet.Cell(2, 1).Value = "COMP-2026-001";
                worksheet.Cell(2, 2).Value = "Computadora de escritorio";
                worksheet.Cell(2, 3).Value = "Dell";
                worksheet.Cell(2, 4).Value = "OptiPlex 7090";
                worksheet.Cell(2, 5).Value = "MX-77661122";
                worksheet.Cell(2, 6).Value = "Activo";
                worksheet.Cell(2, 7).Value = "Laboratorio de Informática"; // Un ejemplo en texto
                worksheet.Cell(2, 8).Value = "Donación MEP";
                worksheet.Cell(2, 9).Value = 350000.00;
                worksheet.Cell(2, 10).Value = "Incluye mouse y teclado";

                var rangoEjemplo = worksheet.Range(2, 1, 2, 10);
                rangoEjemplo.Style.Font.FontColor = XLColor.Gray;
                rangoEjemplo.Style.Font.Italic = true;

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public IEnumerable<InventarioResponse> LeerActivosDeExcel(Stream archivoExcel, IEnumerable<UbicacionResponse> ubicacionesSistema, Guid idCategoriaSeleccionada)
        {
            var listaActivos = new List<InventarioResponse>();

            using (var workbook = new XLWorkbook(archivoExcel))
            {
                var worksheet = workbook.Worksheet(1);
                var filasUsadas = worksheet.RangeUsed().RowsUsed().Skip(1); // Saltamos la primera fila de títulos

                foreach (var fila in filasUsadas)
                {
                    // Si las celdas principales están vacías, ignoramos la fila
                    if (fila.Cell(1).IsEmpty() && fila.Cell(2).IsEmpty())
                        continue;

                    var activo = new InventarioResponse();

                    // 1. Código Físico (No. De Identificación)
                    activo.CodigoFisico = fila.Cell(1).GetString().Trim();

                    // 2. Descripción
                    activo.Descripcion = fila.Cell(2).GetString().Trim();

                    // 3. Marca (Opcional)
                    activo.Marca = fila.Cell(3).IsEmpty() ? null : fila.Cell(3).GetString().Trim();

                    // 4. Modelo (Opcional)
                    activo.Modelo = fila.Cell(4).IsEmpty() ? null : fila.Cell(4).GetString().Trim();

                    // 5. Serie (Opcional)
                    activo.Serie = fila.Cell(5).IsEmpty() ? null : fila.Cell(5).GetString().Trim();

                    // 6. Estado (Activo / Inactivo)
                    string textoEstado = fila.Cell(6).GetString().Trim().ToLower();
                    activo.Estado = (textoEstado == "activo" || textoEstado == "1" || textoEstado == "true");

                    // 7. TRADUCTOR DE UBICACIÓN (Texto -> GUID)
                    string nombreUbicacionExcel = fila.Cell(7).GetString().Trim();

                    // Buscamos en la lista del sistema una ubicación que se llame igual (sin importar mayúsculas o minúsculas)
                    var ubicacionEncontrada = ubicacionesSistema.FirstOrDefault(u =>
                        string.Equals(u.Nombre.Trim(), nombreUbicacionExcel, StringComparison.OrdinalIgnoreCase));

                    if (ubicacionEncontrada != null)
                    {
                        activo.IdUbicacion = ubicacionEncontrada.Id;
                    }
                    else
                    {
                        // Si no la encuentra, arrojamos un error amigable para que el usuario corrija su Excel
                        throw new Exception($"Error en la fila {fila.RowNumber()}: La ubicación '{nombreUbicacionExcel}' no existe registrada en el sistema. Por favor, regístrala primero o corrígela en el Excel.");
                    }

                    // 8 y 10. MODO DE ADQUISICIÓN + OBSERVACIONES (Unión inteligente)
                    string modoAdquisicion = fila.Cell(8).IsEmpty() ? "" : $"Modo de Adquisición: {fila.Cell(8).GetString().Trim()}. ";
                    string observacionesOriginales = fila.Cell(10).IsEmpty() ? "" : fila.Cell(10).GetString().Trim();

                    string observacionesFinales = (modoAdquisicion + observacionesOriginales).Trim();
                    activo.Observaciones = string.IsNullOrEmpty(observacionesFinales) ? null : observacionesFinales;

                    // 9. Precio (Opcional - Decimal)
                    if (!fila.Cell(9).IsEmpty())
                    {
                        if (decimal.TryParse(fila.Cell(9).GetString().Trim(), out decimal precioConvertido))
                            activo.Precio = precioConvertido;
                        else
                            throw new Exception($"Error en la fila {fila.RowNumber()}: El precio tiene un formato numérico inválido.");
                    }
                    else
                    {
                        activo.Precio = null;
                    }

                    // CATEGORÍA (Se asigna la que seleccionó el usuario desde la pantalla web)
                    activo.IdCategoria = idCategoriaSeleccionada;

                    // Auditoría técnica básica
                    activo.Id = Guid.NewGuid();
                    activo.FechaRegistro = DateTime.Now;

                    listaActivos.Add(activo);
                }
            }

            return listaActivos;
        }
    }
}
