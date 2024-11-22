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

namespace CisternasGAMC.Pages.Admin.Cruds.UserCrud
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
        public User User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.UserId == id);

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                User = user;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(short? id, string? action)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (action == "despedir")
            {
                // Cambiar el estado del usuario a "Despedido"
                user.Status = 3;
                await _context.SaveChangesAsync();
            }
            else if (action == "eliminar")
            {
                // Eliminar definitivamente el usuario
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }

    }
}
