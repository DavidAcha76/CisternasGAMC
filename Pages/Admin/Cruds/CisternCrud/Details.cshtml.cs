using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CisternasGAMC.Data;
using CisternasGAMC.Model;
using Microsoft.AspNetCore.Authorization;

namespace CisternasGAMC.Pages.Admin.Cruds.CisternCrud
{
    [Authorize(Roles = "admin")]
    public class DetailsModel : PageModel
    {
        private readonly CisternasGAMC.Data.ApplicationDbContext _context;

        public DetailsModel(CisternasGAMC.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Cistern Cistern { get; set; } = default!;
        public float? TotalWaterDelivered { get; set; }
        public List<string> Drivers { get; set; } = new List<string>();
        public List<string> OTBsVisited { get; set; } = new List<string>();

        // For charts
        public List<float> WaterDeliveredByOTBs { get; set; } = new List<float>();
        public List<string> OTBs { get; set; } = new List<string>();

        // New properties for additional reports
        public List<int> DriverUsageCounts { get; set; } = new List<int>(); // Number of times each driver used the cistern
        public List<string> DriverNames { get; set; } = new List<string>(); // Driver names for chart

        public List<float> WaterPerDelivery { get; set; } = new List<float>(); // Water delivered per delivery
        public List<string> DeliveryDates { get; set; } = new List<string>(); // Delivery dates for each delivery

        public async Task<IActionResult> OnGetAsync(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cistern = await _context.Cisterns.FirstOrDefaultAsync(m => m.CisternId == id);
            if (cistern == null)
            {
                return NotFound();
            }
            else
            {
                Cistern = cistern;

                // Fetch water deliveries
                var waterDeliveries = await _context.WaterDeliveries
                    .Where(w => w.CisternId == id)
                    .Include(w => w.Driver)
                    .Include(w => w.Otb)
                    .ToListAsync();

                // Total water delivered
                TotalWaterDelivered = waterDeliveries.Sum(w => w.DeliveredAmount ?? 0);

                // Unique drivers list
                Drivers = waterDeliveries.Select(w => w.Driver.FirstName + " " + w.Driver.LastName).Distinct().ToList();

                // OTBs visited
                OTBsVisited = waterDeliveries.Select(w => w.Otb.Name).Distinct().ToList();

                // Grouped data for OTBs
                var otbGroup = waterDeliveries.GroupBy(w => w.Otb.Name)
                    .Select(group => new {
                        OTBName = group.Key,
                        TotalWaterDelivered = group.Sum(w => w.DeliveredAmount ?? 0)
                    }).ToList();

                WaterDeliveredByOTBs = otbGroup.Select(g => g.TotalWaterDelivered).ToList();
                OTBs = otbGroup.Select(g => g.OTBName).ToList();

                // New Report 1: Number of times each driver used the cistern
                var driverUsage = waterDeliveries.GroupBy(w => w.Driver.FirstName + " " + w.Driver.LastName)
                    .Select(group => new {
                        DriverName = group.Key,
                        UsageCount = group.Count()
                    }).ToList();

                DriverNames = driverUsage.Select(g => g.DriverName).ToList();
                DriverUsageCounts = driverUsage.Select(g => g.UsageCount).ToList();

                // New Report 2: Water delivered per delivery
                WaterPerDelivery = waterDeliveries.Select(w => w.DeliveredAmount ?? 0).ToList();
                DeliveryDates = waterDeliveries.Select(w => w.DeliveryDate.ToShortDateString()).ToList();
            }

            return Page();
        }
    }
}
