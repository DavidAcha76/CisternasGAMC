using CisternasGAMC.Data;
using CisternasGAMC.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace CisternasGAMC.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // Propiedades para enlazar los datos
        [BindProperty]
        public int? SelectedOtb { get; set; } // Cambiado a int

        public IList<Otb> Otbs { get; set; } = new List<Otb>();

        // Constructor que recibe el contexto de la base de datos
        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método que se ejecuta al cargar la página
        public void OnGet()
        {
            // Cargar todos los OTBs al cargar la página por primera vez
            LoadOtbs();
        }

        private void LoadOtbs()
        {
            Otbs = _context.Otbs.ToList();
        }

        public JsonResult OnGetOtbs()
        {
            var filteredOtbs = _context.Otbs
                .Select(o => new { o.OtbId, o.Name }) // Asegúrate de devolver estas propiedades
                .ToList();

            return new JsonResult(filteredOtbs);
        }
    }
}
