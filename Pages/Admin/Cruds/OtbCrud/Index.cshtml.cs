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
    public class IndexModel : PageModel
    {
        private readonly CisternasGAMC.Data.ApplicationDbContext _context;

        public IndexModel(CisternasGAMC.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Otb> Otb { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Otb = await _context.Otbs.ToListAsync();
        }
    }
}
