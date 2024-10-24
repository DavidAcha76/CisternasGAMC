using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CisternasGAMC.Data;
using CisternasGAMC.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CisternasGAMC.Pages.Admin.Cruds.WaterDeliveryCrud
{
    [Authorize(Roles = "admin")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public WaterDelivery WaterDelivery { get; set; } = default!;

        public IEnumerable<SelectListItem> Cisterns { get; set; }
        public IEnumerable<SelectListItem> Drivers { get; set; }
        public IEnumerable<SelectListItem> Otbs { get; set; }

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

            PopulateSelectLists();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                PopulateSelectLists();
                return Page();
            }

            _context.Attach(WaterDelivery).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WaterDeliveryExists(WaterDelivery.WaterDeliveryId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }
        private bool WaterDeliveryExists(int id)
        {
            return _context.WaterDeliveries.Any(e => e.WaterDeliveryId == id);
        }

        private void PopulateSelectLists()
        {
            Cisterns = _context.Cisterns
                .Select(c => new SelectListItem
                {
                    Value = c.CisternId.ToString(),
                    Text = c.PlateNumber
                })
                .ToList();

            Drivers = _context.Users
                .Where(u => u.Role == "driver")  // Filtro aplicado a usuarios con el rol de "driver"
                .Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),  // Asigna el UserId como valor
                    Text = $"{u.FirstName} {u.LastName}"  // Asigna el nombre completo como texto
                })
                .ToList();


            Otbs = _context.Otbs
                .Select(o => new SelectListItem
                {
                    Value = o.OtbId.ToString(),
                    Text = o.Name
                })
                .ToList();
        }
    }
}
