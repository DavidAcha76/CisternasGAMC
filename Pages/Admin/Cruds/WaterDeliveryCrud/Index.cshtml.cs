using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CisternasGAMC.Data;
using CisternasGAMC.Model;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Collections.Generic;

namespace CisternasGAMC.Pages.Admin.Cruds.WaterDeliveryCrud
{
    [Authorize(Roles = "admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<WaterDelivery> WaterDelivery { get; set; } = default!;

        public async Task OnGetAsync()
        {
            WaterDelivery = await _context.WaterDeliveries
                .Include(w => w.Cistern)
                .Include(w => w.Driver)
                .Include(w => w.Otb)
                .AsNoTracking()
                .ToListAsync();
        }

        public IActionResult OnPostExportToExcel()
        {
            var waterDeliveries = _context.WaterDeliveries
                .Include(w => w.Cistern)
                .Include(w => w.Driver)
                .Include(w => w.Otb)
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Entregas de Agua");

                // Cabecera del Excel con formato
                worksheet.Cell(1, 1).Value = "Cisterna (Placa)";
                worksheet.Cell(1, 2).Value = "Conductor";
                worksheet.Cell(1, 3).Value = "OTB";
                worksheet.Cell(1, 4).Value = "Fecha de entrega";
                worksheet.Cell(1, 5).Value = "Monto entregado";
                worksheet.Cell(1, 6).Value = "Estado entrega";
                worksheet.Cell(1, 7).Value = "Fecha Inicio Entrega";
                worksheet.Cell(1, 8).Value = "Fecha Fin Entrega";

                // Estilo de cabecera
                var headerRange = worksheet.Range("A1:H1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Rellenar el Excel con datos
                int row = 2;
                foreach (var item in waterDeliveries)
                {
                    var estado = item.DeliveryStatus switch
                    {
                        1 => "Programada",
                        2 => "En Proceso",
                        3 => "Finalizada",
                        4 => "Conforme",
                        5 => "Denunciada",
                        _ => "Desconocido"
                    };

                    worksheet.Cell(row, 1).Value = item.Cistern.PlateNumber;
                    worksheet.Cell(row, 2).Value = $"{item.Driver.FirstName} {item.Driver.LastName}";
                    worksheet.Cell(row, 3).Value = item.Otb.Name;
                    worksheet.Cell(row, 4).Value = item.DeliveryDate.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cell(row, 5).Value = item.DeliveredAmount;
                    worksheet.Cell(row, 6).Value = estado;
                    worksheet.Cell(row, 7).Value = item.ArrivalDate.HasValue ? item.ArrivalDate.Value.ToString("dd/MM/yyyy HH:mm") : "N/A";
                    worksheet.Cell(row, 8).Value = item.DepartureDate.HasValue ? item.DepartureDate.Value.ToString("dd/MM/yyyy HH:mm") : "N/A";

                    row++;
                }

                // Ajustar el tamaño de las columnas
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EntregasDeAgua.xlsx");
                }
            }
        }
    }
}
