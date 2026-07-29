using System;
using System.Collections.Generic;
using System.IO;
using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Reglas
{
    public class ReportesHelper : IReportesHelper
    {
        public byte[] GenerarExcelInventario(IEnumerable<InventarioResponse> activos)
        {
            using (var workbook = new XLWorkbook())
            {
                // Creamos la hoja de cálculo
                var worksheet = workbook.Worksheets.Add("Inventario Activos");

                
                // 1. TÍTULO DEL REPORTE
                
                worksheet.Cell(1, 1).Value = "Reporte de Inventario - Escuela de Cerbatana";
                worksheet.Range(1, 1, 1, 6).Merge(); // Combinamos celdas de la A1 a la F1
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml("#1F497D"); // Azul clásico

                
                // 2. ENCABEZADOS DE LA TABLA (Fila 3)
                
                int fila = 3;
                worksheet.Cell(fila, 1).Value = "Código Físico";
                worksheet.Cell(fila, 2).Value = "Descripción";
                worksheet.Cell(fila, 3).Value = "Categoría (ID)";
                worksheet.Cell(fila, 4).Value = "Ubicación (ID)";
                worksheet.Cell(fila, 5).Value = "Estado";
                worksheet.Cell(fila, 6).Value = "Fecha de Registro";

                // Estilo para los encabezados
                var rangoEncabezados = worksheet.Range(fila, 1, fila, 6);
                rangoEncabezados.Style.Font.Bold = true;
                rangoEncabezados.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066CC"); // Fondo azul
                rangoEncabezados.Style.Font.FontColor = XLColor.White; // Letra blanca
                rangoEncabezados.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            
                // 3. LLENADO DE DATOS
                
                foreach (var activo in activos)
                {
                    fila++;
                    worksheet.Cell(fila, 1).Value = activo.CodigoFisico;
                    worksheet.Cell(fila, 2).Value = activo.Descripcion;
                    worksheet.Cell(fila, 3).Value = activo.IdCategoria.ToString();
                    worksheet.Cell(fila, 4).Value = activo.IdUbicacion.ToString();
                    worksheet.Cell(fila, 5).Value = activo.Estado ? "Activo" : "Inactivo";
                    worksheet.Cell(fila, 6).Value = activo.FechaRegistro?.ToString("dd/MM/yyyy") ?? "N/A";
                }

                
                // 4. RETOQUES 
               
                var rangoTabla = worksheet.Range(3, 1, fila, 6);
                rangoTabla.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                rangoTabla.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Ajustamos el ancho de las columnas automáticamente según su texto
                worksheet.Columns().AdjustToContents();

                // Convertimos todo el archivo Excel a un arreglo de bytes
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public byte[] GenerarPdfInventario(IEnumerable<InventarioResponse> activos)
        {
            // Configuración obligatoria para usar la versión gratuita 
            QuestPDF.Settings.License = LicenseType.Community;

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape()); 
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                  
                    // 1. ENCABEZADO DEL DOCUMENTO
                  
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Escuela de Cerbatana").FontSize(18).SemiBold().FontColor("#1F497D");
                            col.Item().Text("Reporte General de Inventario").FontSize(14).FontColor(Colors.Grey.Medium);
                            col.Item().Text($"Fecha de emisión: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9).Italic();
                        });
                    });

                    
                    // 2. CONTENIDO (TABLA)
                   
                    page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                    {
                        // Definimos el ancho de las 6 columnas
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Código Físico
                            columns.RelativeColumn(4); // Descripción
                            columns.RelativeColumn(2); // Categoría ID
                            columns.RelativeColumn(2); // Ubicación ID
                            columns.RelativeColumn(1.5f); // Estado
                            columns.RelativeColumn(2); // Fecha
                        });

                      
                        table.Header(header =>
                        {
                            void ConfigurarCelda(IContainer c, string texto) =>
                                c.Background("#0066CC").Padding(5).Text(texto).FontColor(Colors.White).SemiBold();

                            ConfigurarCelda(header.Cell(), "Código Físico");
                            ConfigurarCelda(header.Cell(), "Descripción");
                            ConfigurarCelda(header.Cell(), "Categoría");
                            ConfigurarCelda(header.Cell(), "Ubicación");
                            ConfigurarCelda(header.Cell(), "Estado");
                            ConfigurarCelda(header.Cell(), "Registro");
                        });

                    
                        foreach (var activo in activos)
                        {
                            void ConfigurarFila(IContainer c, string texto) =>
                                    c.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(texto);

                            ConfigurarFila(table.Cell(), activo.CodigoFisico);
                            ConfigurarFila(table.Cell(), activo.Descripcion);
                            ConfigurarFila(table.Cell(), activo.IdCategoria.ToString().Substring(0, 8) + "..."); 
                            ConfigurarFila(table.Cell(), activo.IdUbicacion.ToString().Substring(0, 8) + "...");
                            ConfigurarFila(table.Cell(), activo.Estado ? "Activo" : "Inactivo");
                            ConfigurarFila(table.Cell(), activo.FechaRegistro?.ToString("dd/MM/yyyy") ?? "N/A");
                        }
                    });

                   
                    // 3. PIE DE PÁGINA (Números de página automáticos)
                    
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            // Generamos el archivo en memoria y lo devolvemos como bytes
            return documento.GeneratePdf();
        }
    }
}
