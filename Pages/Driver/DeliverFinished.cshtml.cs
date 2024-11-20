using CisternasGAMC.Data;
using CisternasGAMC.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Telegram.Bot;

namespace CisternasGAMC.Pages.Driver
{
    [Authorize(Roles = "driver")]
    public class DeliverFinishedModel : PageModel
    {
        private readonly string _telegramToken = "8028273148:AAFH_JfTnfHZxSYDTUTvz-ly9sU8Ls91pSY";

        private readonly TelegramBotClient _botClient;
        private readonly ApplicationDbContext _context;

        // Propiedad para almacenar el ID de la entrega de agua
        public int? WaterDeliveryId { get; private set; }

        // Modelo de la entrega de agua vinculado con el formulario
        [BindProperty]
        public WaterDelivery WaterDelivery { get; set; }

        // Constructor que recibe el contexto de la base de datos
        public DeliverFinishedModel(ApplicationDbContext context, IConfiguration configuration)
        {
            _botClient = new TelegramBotClient(_telegramToken);
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
            if (deliveredAmount <= 0)
            {
                ModelState.AddModelError("DeliveredAmount", "La cantidad entregada debe ser mayor a cero.");
                return Page();
            }

            var delivery = await _context.WaterDeliveries
                .Include(d => d.Otb) // Incluye la OTB para reducir consultas adicionales
                .FirstOrDefaultAsync(d => d.WaterDeliveryId == waterDeliveryId);

            if (delivery == null)
            {
                return NotFound("La entrega de agua no fue encontrada.");
            }

            delivery.DepartureDate = DateTime.Now;
            delivery.DeliveredAmount = deliveredAmount;
            delivery.DeliveryStatus = 3; // Marcamos como completado

            try
            {
                await _context.SaveChangesAsync();

                if (delivery.Otb != null && !string.IsNullOrEmpty(delivery.Otb.ChatId))
                {
                    string message = $"La cisterna ha completado su entrega en la OTB '{delivery.Otb.Name}' el {DateTime.Now:dd/MM/yyyy HH:mm}.";
                    await _botClient.SendTextMessageAsync(delivery.Otb.ChatId, message);
                }
                else
                {
                    Console.WriteLine("La OTB no tiene un ChatId válido para enviar notificaciones.");
                }
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Error actualizando la base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando mensaje a Telegram: {ex.Message}");
            }

            return RedirectToPage("/Driver/Index");
        }

        // Método para cargar los detalles de la entrega de agua
        private async Task LoadWaterDeliveryAsync()
        {
            WaterDelivery = await _context.WaterDeliveries
                .Include(wd => wd.Otb) // Incluye la información de la OTB relacionada
                .FirstOrDefaultAsync(wd => wd.WaterDeliveryId == WaterDeliveryId);
        }
    }
}
