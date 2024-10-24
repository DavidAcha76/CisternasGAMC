using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CisternasGAMC.Data;
using CisternasGAMC.Model;
using Microsoft.AspNetCore.Authorization;

namespace CisternasGAMC.Pages.Admin.Cruds.UserCrud
{
    [Authorize(Roles = "admin")]
    public class CreateModel : PageModel
    {
        private readonly CisternasGAMC.Data.ApplicationDbContext _context;

        public CreateModel(CisternasGAMC.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }


        [BindProperty]
        public User User { get; set; } = default!;
        public string password;


        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            password = User.Password;
            if (!ModelState.IsValid)
            {
                return Page();
            }
            User.Password= BCrypt.Net.BCrypt.HashPassword(password);
            User.Status = 1;
            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
