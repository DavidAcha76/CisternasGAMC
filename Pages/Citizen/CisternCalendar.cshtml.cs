using CisternasGAMC.Data;
using CisternasGAMC.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CisternasGAMC.Pages.Citizen
{
    public class CisternCalendarModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty(SupportsGet = true)]
        public int SelectedOtb { get; set; }

        public string NombreOTB { get; set; }

        public string CisternStatusIcon { get; set; }

        public List<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();

        public CisternCalendarModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet(int? selectedOtb)
        {
            if (selectedOtb.HasValue)
            {
                SelectedOtb = selectedOtb.Value;
                LoadCisternStatus();
                var otbData = _context.Otbs.FirstOrDefault(o => o.OtbId == SelectedOtb);
                if (otbData != null)
                {
                    NombreOTB = otbData.Name;
                }

                // Get water deliveries for each status
                var waterDeliveries = GetWaterDeliveriesByStatus(1)
                    .Concat(GetWaterDeliveriesByStatus(2))
                    .Concat(GetWaterDeliveriesByStatus(3))
                    .ToList();

                // Transform deliveries into calendar events
                CalendarEvents = waterDeliveries.Select(w => new CalendarEvent
                {
                    Title = w.DeliveryStatus.ToString(),
                    DayOfWeek = w.DeliveryDate.DayOfWeek.ToString(),
                    TimeSlot = GetTimeSlot(w.DeliveryDate.Hour)
                }).ToList();
            }
        }

        public List<WaterDelivery> GetWaterDeliveriesByStatus(int status)
        {
            return _context.WaterDeliveries
                .Where(w => w.OtbId == SelectedOtb && w.DeliveryStatus == status)
                .ToList();
        }

        private void LoadCisternStatus()
        {
            var cistern = _context.WaterDeliveries
                .FirstOrDefault(w => w.OtbId == SelectedOtb);

            if (cistern != null)
            {
                // Determine icon based on delivery status
                switch (cistern.DeliveryStatus)
                {
                    case 1:
                        CisternStatusIcon = "Entrega Programada";
                        break;
                    case 2:
                        CisternStatusIcon = "Entrega En Curso";
                        break;
                    case 3:
                        CisternStatusIcon = "Entrega Finalizada";
                        break;
                    default:
                        CisternStatusIcon = "";
                        break;
                }
            }
            else
            {
                CisternStatusIcon = ""; // Default icon if no deliveries
            }
        }

        private string GetTimeSlot(int hour)
        {
            if (hour >= 6 && hour < 12)
                return "En la Mañana";
            else if (hour >= 12 && hour < 18)
                return "En la Tarde";
            else
                return "En la Noche";
        }

        public class CalendarEvent
        {
            public string Title { get; set; }
            public string DayOfWeek { get; set; }
            public string TimeSlot { get; set; }
        }

        // Method to translate DayOfWeek to Spanish
        public string GetSpanishDayName(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => "Lunes",
                DayOfWeek.Tuesday => "Martes",
                DayOfWeek.Wednesday => "Miércoles",
                DayOfWeek.Thursday => "Jueves",
                DayOfWeek.Friday => "Viernes",
                DayOfWeek.Saturday => "Sábado",
                DayOfWeek.Sunday => "Domingo",
                _ => day.ToString(),
            };
        }
    }
}
