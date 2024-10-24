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

namespace CisternasGAMC.Pages.Admin.Cruds.OtbCrud
{
    [Authorize(Roles = "admin")]
    public class DetailsModel : PageModel
    {
        private readonly CisternasGAMC.Data.ApplicationDbContext _context;

        public DetailsModel(CisternasGAMC.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Otb Otb { get; set; } = default!;

        public List<string> Cisterns { get; set; } = new List<string>();
        public List<string> Drivers { get; set; } = new List<string>();
        public List<float> WaterPerDelivery { get; set; } = new List<float>();
        public List<string> DeliveryDates { get; set; } = new List<string>();
        public float? TotalWaterDelivered { get; set; }

        public List<int> CisternVisits { get; set; } = new List<int>(); // Cistern visits count
        public List<int> DriverVisits { get; set; } = new List<int>(); // Driver visits count

        public async Task<IActionResult> OnGetAsync(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var otb = await _context.Otbs.FirstOrDefaultAsync(m => m.OtbId == id);
            if (otb == null)
            {
                return NotFound();
            }

            Otb = otb;

            // Fetch all water deliveries to this OTB
            var waterDeliveries = await _context.WaterDeliveries
                .Where(w => w.OtbId == id)
                .Include(w => w.Cistern)
                .Include(w => w.Driver)
                .ToListAsync();

            Cisterns = waterDeliveries.Select(w => w.Cistern.PlateNumber).Distinct().ToList();
            Drivers = waterDeliveries.Select(w => w.Driver.FirstName + " " + w.Driver.LastName).Distinct().ToList();
            WaterPerDelivery = waterDeliveries.Select(w => w.DeliveredAmount ?? 0).ToList();
            DeliveryDates = waterDeliveries.Select(w => w.DeliveryDate.ToShortDateString()).ToList();
            TotalWaterDelivered = waterDeliveries.Sum(w => w.DeliveredAmount ?? 0);

            // Count visits per cistern
            CisternVisits = Cisterns.Select(c => waterDeliveries.Count(w => w.Cistern.PlateNumber == c)).ToList();

            // Count visits per driver
            DriverVisits = Drivers.Select(d => waterDeliveries.Count(w => (w.Driver.FirstName + " " + w.Driver.LastName) == d)).ToList();

            return Page();
        }
    }
}
