using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CisternasGAMC.Data;
using CisternasGAMC.Model;

namespace CisternasGAMC.Pages.Admin.Cruds.WaterDeliveryCrud
{
    [Authorize(Roles = "admin")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public WaterDelivery WaterDelivery { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            WaterDelivery = await _context.WaterDeliveries
                .Include(w => w.Cistern)
                .Include(w => w.Driver)
                .Include(w => w.Otb)
                .FirstOrDefaultAsync(m => m.WaterDeliveryId == id);

            if (WaterDelivery == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
