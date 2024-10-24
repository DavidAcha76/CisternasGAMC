using CisternasGAMC.Data;
using CisternasGAMC.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CisternasGAMC.Pages.Driver
{
    [Authorize(Roles = "driver")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<WaterDelivery> waterDeliveries { get; set; } = default!;
        public IList<Otb> otbs { get; set; } = default!;

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (short.TryParse(userId, out short driverId))
            {
                waterDeliveries = await _context.WaterDeliveries
                    .Include(wd => wd.Otb)
                    .Where(wd => wd.DriverId == driverId && (wd.DeliveryStatus == 1 || wd.DeliveryStatus == 2))
                    .ToListAsync();
                otbs = waterDeliveries.Select(wd => wd.Otb).Distinct().ToList();
            }
        }
    }
}
