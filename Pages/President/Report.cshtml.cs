using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using CisternasGAMC.Data;
using CisternasGAMC.Model;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Authorization;

namespace CisternasGAMC.Pages.President
{
    [Authorize(Roles = "president")]
    public class ReportModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public int? WaterDeliveryId { get; private set; }

        [BindProperty]
        public WaterDelivery WaterDelivery { get; set; }

        public ReportModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int? waterDeliveryId)
        {
            if (waterDeliveryId == null)
            {
                return NotFound();
            }

            WaterDeliveryId = waterDeliveryId;
            await LoadWaterDeliveryAsync();

            if (WaterDelivery == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSetArrivalAsync(int waterDeliveryId)
        {
            var delivery = await _context.WaterDeliveries.FirstOrDefaultAsync(d => d.WaterDeliveryId == waterDeliveryId);

            if (delivery == null)
            {
                return NotFound("Water delivery not found.");
            }

            delivery.DeliveryStatus = 4; // Entrega conforme

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Error updating the database: {ex.Message}");
            }

            return RedirectToPage("/President/index", new { waterDeliveryId });
        }

        public async Task<IActionResult> OnPostReportIssueAsync(int waterDeliveryId)
        {
            var delivery = await _context.WaterDeliveries.FirstOrDefaultAsync(d => d.WaterDeliveryId == waterDeliveryId);

            if (delivery == null)
            {
                return NotFound("Water delivery not found.");
            }

            delivery.DeliveryStatus = 5; // Entrega no conforme

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Error updating the database: {ex.Message}");
            }

            return RedirectToPage("/President/index", new { waterDeliveryId });
        }

        private async Task LoadWaterDeliveryAsync()
        {
            WaterDelivery = await _context.WaterDeliveries
                .Include(wd => wd.Otb)
                .FirstOrDefaultAsync(wd => wd.WaterDeliveryId == WaterDeliveryId);
        }
    }
}