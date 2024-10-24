using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CisternasGAMC.Data;
using CisternasGAMC.Model;
using Microsoft.AspNetCore.Authorization;

namespace CisternasGAMC.Pages.Admin.Cruds.CisternCrud
{
    [Authorize(Roles = "admin")]
    public class EditModel : PageModel
    {
        private readonly CisternasGAMC.Data.ApplicationDbContext _context;

        public EditModel(CisternasGAMC.Data.ApplicationDbContext context)
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

            var cistern =  await _context.Cisterns.FirstOrDefaultAsync(m => m.CisternId == id);
            if (cistern == null)
            {
                return NotFound();
            }
            Cistern = cistern;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Cistern).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CisternExists(Cistern.CisternId))
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

        private bool CisternExists(byte id)
        {
            return _context.Cisterns.Any(e => e.CisternId == id);
        }
    }
}
