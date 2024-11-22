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
    public class IndexModel : PageModel
    {
        private readonly CisternasGAMC.Data.ApplicationDbContext _context;

        public IndexModel(CisternasGAMC.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Cistern> Cistern { get;set; } = default!;

        public async Task OnGetAsync()
        {
            // Cargar solo las cisternas activas (estado 1 o 2)
            Cistern = await _context.Cisterns
                .Where(c => c.Status == 1 || c.Status == 2)
                .ToListAsync();
        }

    }
}
