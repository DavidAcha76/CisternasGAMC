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
    public class DeleteModel : PageModel
    {
        private readonly CisternasGAMC.Data.ApplicationDbContext _context;

        public DeleteModel(CisternasGAMC.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Otb Otb { get; set; } = default!;

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
            else
            {
                Otb = otb;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var otb = await _context.Otbs.FindAsync(id);
            if (otb != null)
            {
                Otb = otb;
                _context.Otbs.Remove(Otb);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
