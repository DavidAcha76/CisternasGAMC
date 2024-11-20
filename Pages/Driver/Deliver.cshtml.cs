using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using CisternasGAMC.Data;
using CisternasGAMC.Model;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Authorization;
using Telegram.Bot;

namespace CisternasGAMC.Pages.Driver
{
    [Authorize(Roles = "driver")]
    public class DeliverModel : PageModel
    {
        private readonly TelegramBotClient _botClient;
        private readonly ApplicationDbContext _context;
        private readonly string _telegramToken = "8028273148:AAFH_JfTnfHZxSYDTUTvz-ly9sU8Ls91pSY";

        public int? WaterDeliveryId { get; private set; }

        [BindProperty]
        public WaterDelivery WaterDelivery { get; set; }

        public DeliverModel(ApplicationDbContext context, IConfiguration configuration)
        {
            _botClient = new TelegramBotClient(_telegramToken);
            _context = context;
        }

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

        public async Task<IActionResult> OnPostSetArrivalAsync(int waterDeliveryId)
        {
            var delivery = await _context.WaterDeliveries
                .Include(d => d.Otb) // Incluye la OTB para evitar consultas adicionales
                .FirstOrDefaultAsync(d => d.WaterDeliveryId == waterDeliveryId);

            if (delivery == null)
            {
                return NotFound("La entrega de agua no fue encontrada.");
            }

            if (delivery.DeliveryStatus == 2 || delivery.DeliveryStatus == 3)
            {
                return RedirectToPage("/Driver/DeliverFinished", new { waterDeliveryId });
            }

            delivery.ArrivalDate = DateTime.Now;
            delivery.DeliveryStatus = 2; // Estado de "Llegada"

            try
            {
                await _context.SaveChangesAsync();

                if (delivery.Otb != null && !string.IsNullOrEmpty(delivery.Otb.ChatId))
                {
                    string message = $"La cisterna ha llegado a la OTB '{delivery.Otb.Name}' el {DateTime.Now:dd/MM/yyyy HH:mm}.";
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

            return RedirectToPage("/Driver/DeliverFinished", new { waterDeliveryId });
        }

        private async Task LoadWaterDeliveryAsync()
        {
            WaterDelivery = await _context.WaterDeliveries
                .Include(wd => wd.Otb) // Incluye la información de la OTB relacionada
                .FirstOrDefaultAsync(wd => wd.WaterDeliveryId == WaterDeliveryId);
        }
    }
}
