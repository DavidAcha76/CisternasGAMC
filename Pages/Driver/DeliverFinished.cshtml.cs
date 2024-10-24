using CisternasGAMC.Data;
using CisternasGAMC.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CisternasGAMC.Pages.Driver
{
    public class DeliverFinishedModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // Propiedad para almacenar el ID de la entrega de agua
        public int? WaterDeliveryId { get; private set; }

        // Modelo de la entrega de agua vinculado con el formulario
        [BindProperty]
        public WaterDelivery WaterDelivery { get; set; }

        // Constructor que recibe el contexto de la base de datos
        public DeliverFinishedModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método para cargar los detalles de la entrega de agua en la página inicial
        public async Task<IActionResult> OnGetAsync(int? waterDeliveryId)
        {
            if (waterDeliveryId == null)
            {
                return NotFound();
            }

            WaterDeliveryId = waterDeliveryId;
            await LoadWaterDeliveryAsync();

            if (WaterDelivery == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeliverAsync(int waterDeliveryId, float deliveredAmount)
        {
            var delivery = await _context.WaterDeliveries.FirstOrDefaultAsync(d => d.WaterDeliveryId == waterDeliveryId);

            if (delivery == null)
            {
                return NotFound("Water delivery not found.");
            }

            delivery.DepartureDate = DateTime.Now;
            delivery.DeliveredAmount = deliveredAmount;
            delivery.DeliveryStatus = 3; // Marcamos como completado

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Error updating the database: {ex.Message}");
            }

            return RedirectToPage("/Driver/Index");
        }


        // Método para cargar los detalles de la entrega de agua
        private async Task LoadWaterDeliveryAsync()
        {
            WaterDelivery = await _context.WaterDeliveries
                .Include(wd => wd.Otb)
                .FirstOrDefaultAsync(wd => wd.WaterDeliveryId == WaterDeliveryId);
        }
    }
}
