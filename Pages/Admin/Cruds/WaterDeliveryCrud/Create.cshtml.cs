using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CisternasGAMC.Data;
using CisternasGAMC.Model;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CisternasGAMC.Pages.Admin.Cruds.WaterDeliveryCrud
{
    [Authorize(Roles = "admin")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public WaterDelivery WaterDelivery { get; set; } = default!;

        public IEnumerable<SelectListItem> Cisterns { get; set; }
        public IEnumerable<SelectListItem> Drivers { get; set; }
        public IEnumerable<SelectListItem> Otbs { get; set; }

        public IActionResult OnGet()
        {
            PopulateSelectLists();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validar si los IDs seleccionados existen en la base de datos
            if (!ModelState.IsValid ||
                !await _context.Cisterns.AnyAsync(c => c.CisternId == WaterDelivery.CisternId) ||
                !await _context.Users.AnyAsync(u => u.UserId == WaterDelivery.DriverId) ||
                !await _context.Otbs.AnyAsync(o => o.OtbId == WaterDelivery.OtbId))
            {
                if (!await _context.Cisterns.AnyAsync(c => c.CisternId == WaterDelivery.CisternId))
                    ModelState.AddModelError("WaterDelivery.CisternId", "La cisterna seleccionada no es válida.");

                if (!await _context.Users
                    .AnyAsync(u => u.UserId == WaterDelivery.DriverId && u.Role == "driver"))
                {
                    ModelState.AddModelError("WaterDelivery.DriverId", "El conductor seleccionado no es válido o no tiene el rol 'driver'.");
                }


                if (!await _context.Otbs.AnyAsync(o => o.OtbId == WaterDelivery.OtbId))
                    ModelState.AddModelError("WaterDelivery.OtbId", "La OTB seleccionada no es válida.");

                PopulateSelectLists(); // Recargar las listas desplegables si hay errores
                return Page();
            }

            // Establecer valores predeterminados
            WaterDelivery.DeliveredAmount = 0; // Asigna 0 a DeliveredAmount
            WaterDelivery.DeliveryStatus = 1;  // Asigna 1 a DeliveryStatus

            // Guardar en la base de datos
            _context.WaterDeliveries.Add(WaterDelivery);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private void PopulateSelectLists()
        {
            Cisterns = _context.Cisterns
                .Select(c => new SelectListItem
                {
                    Value = c.CisternId.ToString(),
                    Text = c.PlateNumber // Muestra algún valor representativo
                })
                .ToList();

            Drivers = _context.Users
    .Where(u => u.Role == "driver") // Filtra solo los usuarios con el rol "driver"
    .Select(u => new SelectListItem
    {
        Value = u.UserId.ToString(),
        Text = $"{u.FirstName} {u.LastName}" // Mostrar el nombre completo del conductor
    })
    .ToList();


            Otbs = _context.Otbs
                .Select(o => new SelectListItem
                {
                    Value = o.OtbId.ToString(),
                    Text = o.Name // Nombre de la OTB
                })
                .ToList();
        }
    }
}
