using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CisternasGAMC.Data;
using CisternasGAMC.Model;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;

namespace CisternasGAMC.Pages.Admin.Cruds.WaterDeliveryCrud
{
    [Authorize(Roles = "admin")]
    public class CreateModelPro : PageModel
    {
        private readonly string _telegramToken = "8028273148:AAFH_JfTnfHZxSYDTUTvz-ly9sU8Ls91pSY";
        private readonly TelegramBotClient _botClient;
        private readonly ApplicationDbContext _context;
        public string ErrorMessage { get; private set; }

        public CreateModelPro(ApplicationDbContext context)
        {
            _botClient = new TelegramBotClient(_telegramToken);
            _context = context;
        }

        [BindProperty]
        public WaterDelivery WaterDelivery { get; set; } = default!;

        // Propiedad para almacenar las OTBs seleccionadas
        [BindProperty]
        public List<int> SelectedOtbs { get; set; } = new List<int>();

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
            // Verificar si el modelo es válido y si hay 3 OTBs seleccionadas
            if (!ModelState.IsValid)
            {

                PopulateSelectLists();
                return Page();
            }

            // Verificar la validez de Cisterna y Conductor
            if (!await _context.Cisterns.AnyAsync(c => c.CisternId == WaterDelivery.CisternId) ||
                !await _context.Users.AnyAsync(u => u.UserId == WaterDelivery.DriverId && u.Role == "driver"))
            {
                ModelState.AddModelError(string.Empty, "Cisterna o Conductor seleccionado no son válidos.");
                PopulateSelectLists();
                return Page();
            }

            // Crear un registro de entrega por cada OTB seleccionada
            foreach (var otbId in SelectedOtbs)
            {
                var newDelivery = new WaterDelivery
                {
                    CisternId = WaterDelivery.CisternId,
                    DriverId = WaterDelivery.DriverId,
                    OtbId = (short)otbId,
                    DeliveryDate = WaterDelivery.DeliveryDate,
                    DeliveredAmount = 0,
                    DeliveryStatus = 1
                };
                if(newDelivery.OtbId == 0)
                {
                    break;
                }
                _context.WaterDeliveries.Add(newDelivery);
                var otb = _context.Otbs.FirstOrDefault(o => o.OtbId == otbId);
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
            }
            
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        private void PopulateSelectLists()
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
                .Select(o => new SelectListItem
                {
                    Value = o.OtbId.ToString(),
                    Text = o.Name
                })
                .ToList();
        }
    }
}
