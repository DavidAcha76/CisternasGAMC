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
using Microsoft.EntityFrameworkCore;

namespace CisternasGAMC.Pages.Admin.Cruds.CisternCrud
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
        public Cistern Cistern { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Verificar si ya existe una cisterna con el mismo número de placa
            var existingCistern = await _context.Cisterns
                .FirstOrDefaultAsync(c => c.PlateNumber == Cistern.PlateNumber);

            if (existingCistern != null)
            {
                // Agregar un mensaje de error al estado del modelo
                ModelState.AddModelError(string.Empty, "Ya existe una cisterna con este número de placa. Por favor, verifica los datos.");
                return Page();
            }

            // Establecer el estado inicial y guardar la nueva cisterna
            Cistern.Status = 1; // Estado inicial de la cisterna
            _context.Cisterns.Add(Cistern);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

    }
}
