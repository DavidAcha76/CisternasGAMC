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

        public string UrlTelegram { get; set; }

        public string CisternStatusIcon { get; set; }

        public List<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();

        public DateTime StartOfWeek { get; private set; }
        public DateTime EndOfWeek { get; private set; }
        public string ProgrammedDeliveriesMessage { get; set; }

        public CisternCalendarModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet(int? selectedOtb)
        {
            if (!selectedOtb.HasValue)
            {
                ProgrammedDeliveriesMessage = "No se seleccionó ninguna OTB.";
                return;
            }

            SelectedOtb = selectedOtb.Value;
            LoadOtbData();
            LoadCalendarData();
        }

        private void LoadOtbData()
        {
            var otbData = _context.Otbs.FirstOrDefault(o => o.OtbId == SelectedOtb);
            if (otbData != null)
            {
                NombreOTB = otbData.Name;
                UrlTelegram = otbData.UrlTelegram;
            }
            LoadCisternStatus();
        }

        private void LoadCalendarData()
        {
            StartOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
            EndOfWeek = StartOfWeek.AddDays(6);

            var waterDeliveries = GetWaterDeliveriesByStatus(1, 2, 3);

            CalendarEvents = waterDeliveries.Select(w => new CalendarEvent
            {
                Title = w.DeliveryStatus.ToString(),
                DayOfWeek = w.DeliveryDate.DayOfWeek.ToString(),
                TimeSlot = GetTimeSlot(w.DeliveryDate.Hour),
                CssClass = GetCssClassForStatus(w.DeliveryStatus)
            }).ToList();

            var programadas = waterDeliveries
                .Select(w => new { w.DeliveryDate, DayName = GetSpanishDayName(w.DeliveryDate.DayOfWeek) })
                .Distinct()
                .ToList();

            ProgrammedDeliveriesMessage = programadas.Any()
                ? $"Entrega programada para el día {string.Join(" / ", programadas.Select(p => $"{p.DayName} {p.DeliveryDate:dd/MM}"))}"
                : "No hay entregas programadas para esta semana.";
        }

        private void LoadCisternStatus()
        {
            var cistern = _context.WaterDeliveries
                .Where(w => w.OtbId == SelectedOtb)
                .OrderByDescending(w => w.DeliveryDate)
                .FirstOrDefault();

            CisternStatusIcon = cistern?.DeliveryStatus switch
            {
                1 => "Entrega Programada",
                2 => "Entrega En Curso",
                3 => "Entrega Finalizada",
                _ => "Sin Información",
            };
        }

        public List<WaterDelivery> GetWaterDeliveriesByStatus(params int[] statuses)
        {
            return _context.WaterDeliveries
                .Where(w => w.OtbId == SelectedOtb
                            && statuses.Contains(w.DeliveryStatus)
                            && w.DeliveryDate >= StartOfWeek
                            && w.DeliveryDate <= EndOfWeek)
                .ToList();
        }

        public List<DayOfWeek> GetWorkingDays()
        {
            return new List<DayOfWeek>
            {
                DayOfWeek.Monday,
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday,
                DayOfWeek.Friday
            };
        }

        private string GetTimeSlot(int hour)
        {
            return hour switch
            {
                >= 6 and < 12 => "En la Mañana",
                >= 12 and < 18 => "En la Tarde",
                _ => "En la Noche",
            };
        }

        private string GetCssClassForStatus(int status)
        {
            return status switch
            {
                1 => "delivery-scheduled",
                2 => "delivery-in-process",
                3 => "delivery-completed",
                _ => "delivery-default",
            };
        }

        public string GetCurrentWeekRange()
        {
            return $"{StartOfWeek:dd} {StartOfWeek:MMM} - {EndOfWeek:dd} {EndOfWeek:MMM}";
        }

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

        public class CalendarEvent
        {
            public string Title { get; set; }
            public string DayOfWeek { get; set; }
            public string TimeSlot { get; set; }
            public string CssClass { get; set; }
        }
    }
}
