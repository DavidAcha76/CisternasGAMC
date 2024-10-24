using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CisternasGAMC.Data;
using CisternasGAMC.Model;
using Microsoft.AspNetCore.Authorization;

namespace CisternasGAMC.Pages.Admin.Cruds.UserCrud
{
    [Authorize(Roles = "admin")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public User User { get; set; } = default!;

        // Propiedades para los reportes
        public List<string> FavoriteCisterns { get; set; } = new List<string>();
        public List<string> FrequentOTBs { get; set; } = new List<string>();
        public float AverageWaterDelivered { get; set; }
        public int TotalDeliveries { get; set; }
        public float TotalWaterDelivered { get; set; }

        public async Task<IActionResult> OnGetAsync(short? id)
        {
            if (id == null) return NotFound();

            // Obtener el usuario por ID
            User = await _context.Users.FirstOrDefaultAsync(m => m.UserId == id);
            if (User == null) return NotFound();

            // Obtener todas las entregas realizadas por este conductor
            var deliveries = await _context.WaterDeliveries
                .Where(d => d.DriverId == id)
                .Include(d => d.Cistern)
                .Include(d => d.Otb)
                .ToListAsync();

            if (!deliveries.Any())
            {
                // Si no hay entregas, inicializamos los reportes en cero
                TotalWaterDelivered = 0;
                TotalDeliveries = 0;
                AverageWaterDelivered = 0;
                return Page();
            }

            // Reporte 1: Cisternas más usadas
            FavoriteCisterns = deliveries.GroupBy(d => d.Cistern.PlateNumber)
                .OrderByDescending(g => g.Count())
                .Select(g => $"{g.Key} ({g.Count()} usos)")
                .ToList();

            // Reporte 2: OTBs más visitadas
            FrequentOTBs = deliveries.GroupBy(d => d.Otb.Name)
                .OrderByDescending(g => g.Count())
                .Select(g => $"{g.Key} ({g.Count()} visitas)")
                .ToList();

            // Reporte 3: Estadísticas de agua entregada
            TotalWaterDelivered = deliveries.Sum(d => d.DeliveredAmount ?? 0);
            TotalDeliveries = deliveries.Count;
            AverageWaterDelivered = TotalDeliveries > 0
                ? TotalWaterDelivered / TotalDeliveries
                : 0;

            return Page();
        }
    }
}
