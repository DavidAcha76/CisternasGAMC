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

            // Verificar si ya existe una cisterna con el mismo número de placa, ignorando la cisterna actual
            var existingCistern = await _context.Cisterns
                .FirstOrDefaultAsync(c => c.PlateNumber == Cistern.PlateNumber && c.CisternId != Cistern.CisternId);

            if (existingCistern != null)
            {
                // Agregar un mensaje de error al estado del modelo
                ModelState.AddModelError(string.Empty, "Ya existe una cisterna con este número de placa. Por favor, verifica los datos.");
                return Page();
            }

            // Adjuntar y marcar el modelo como modificado
            _context.Attach(Cistern).State = EntityState.Modified;

            try
            {
                // Guardar cambios
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Verificar si la cisterna aún existe
                if (!CisternExists(Cistern.CisternId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Redirigir a la página de índice
            return RedirectToPage("./Index");
        }

        // Método auxiliar para verificar la existencia de una cisterna
        private bool CisternExists(int id)
        {
            return _context.Cisterns.Any(e => e.CisternId == id);
        }


        private bool CisternExists(byte id)
        {
            return _context.Cisterns.Any(e => e.CisternId == id);
        }
    }
}
