using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CisternasGAMC.Data;
using CisternasGAMC.Model;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;

namespace CisternasGAMC.Pages.Admin.Cruds.WaterDeliveryCrud
{
    [Authorize(Roles = "admin")]
    public class CreateModel : PageModel
    {
        private readonly string _telegramToken = "8028273148:AAFH_JfTnfHZxSYDTUTvz-ly9sU8Ls91pSY";
        private readonly TelegramBotClient _botClient;
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _botClient = new TelegramBotClient(_telegramToken);
            _context = context;
        }

        [BindProperty]
        public WaterDelivery WaterDelivery { get; set; } = default!;

        public IEnumerable<SelectListItem> Cisterns { get; set; }
        public IEnumerable<SelectListItem> Drivers { get; set; }
        public IEnumerable<SelectListItem> Otbs { get; set; }
        public string ErrorMessage { get; private set; }
        public IActionResult OnGet(string otbFilter = "")
        {
            PopulateSelectLists(otbFilter);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            
            if (!ModelState.IsValid ||
                !await _context.Cisterns.AnyAsync(c => c.CisternId == WaterDelivery.CisternId) ||
                !await _context.Users.AnyAsync(u => u.UserId == WaterDelivery.DriverId && u.Role == "driver") ||
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

            WaterDelivery.DeliveredAmount = 0;
            WaterDelivery.DeliveryStatus = 1;
            var otb = _context.Otbs.FirstOrDefault(o => o.OtbId == WaterDelivery.OtbId);
            try
            {
                string formattedDate = WaterDelivery.DeliveryDate.ToString("dddd, dd MMMM");
                string message = $"{formattedDate}: Programaron una entrega a su OTB";

                await _botClient.SendTextMessageAsync(otb.ChatId, message);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al enviar mensaje: {ex.Message}";
            }
            _context.WaterDeliveries.Add(WaterDelivery);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private void PopulateSelectLists(string otbFilter = "")
        {
            Cisterns = _context.Cisterns
                .Select(c => new SelectListItem
                {
                    Value = c.CisternId.ToString(),
                    Text = c.PlateNumber
                })
                .ToList();

            Drivers = _context.Users
                .Where(u => u.Role == "driver")
                .Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),
                    Text = $"{u.FirstName} {u.LastName}"
                })
                .ToList();

            Otbs = _context.Otbs
                .Where(o => o.Name.Contains(otbFilter)) // Filtrar por nombre si se proporciona un filtro
                .Select(o => new SelectListItem
                {
                    Value = o.OtbId.ToString(),
                    Text = o.Name
                })
                .ToList();
        }
    }
}
