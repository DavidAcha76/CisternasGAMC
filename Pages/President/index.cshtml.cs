using CisternasGAMC.Data;
using CisternasGAMC.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CisternasGAMC.Pages.President
{
    [Authorize(Roles = "president")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<WaterDelivery> WaterDeliveries { get; private set; } = new List<WaterDelivery>();
        public User CurrentUser { get; set; } // Renombrado para evitar conflicto

        public async Task OnGetAsync()
        {
            // Obtén el valor del claim 'Country' y asegúrate de que sea convertible a short
            var otbIdClaim = User.FindFirst(ClaimTypes.Country)?.Value;

            if (short.TryParse(otbIdClaim, out short otbId))
            {
                WaterDeliveries = await _context.WaterDeliveries
                    .Include(wd => wd.Otb)
                    .Where(wd => wd.DeliveryStatus == 3 && wd.OtbId == otbId)
                    .ToListAsync();
            }
            else
            {
                // Maneja el caso en el que el claim no es válido o no está presente
                System.Diagnostics.Debug.WriteLine("El OtbId en el claim 'Country' no es válido.");
                WaterDeliveries = new List<WaterDelivery>();
            }
        }
    }
}
