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
    public class DeleteModel : PageModel
    {
        private readonly CisternasGAMC.Data.ApplicationDbContext _context;

        public DeleteModel(CisternasGAMC.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cistern Cistern { get; set; } = default!;

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
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cistern = await _context.Cisterns.FindAsync(id);
            if (cistern != null)
            {
                Cistern = cistern;
                _context.Cisterns.Remove(Cistern);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
