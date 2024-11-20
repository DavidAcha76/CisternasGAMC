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

        [BindProperty]
        public int? SelectedOtb { get; set; }

        public IList<Otb> Otbs { get; set; } = new List<Otb>();
        public IList<byte> Districts { get; set; } = new List<byte>(); // Lista de distritos �nicos

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            LoadOtbs();
            LoadUniqueDistricts(); // Carga distritos �nicos al cargar la p�gina
        }

        private void LoadOtbs()
        {
            Otbs = _context.Otbs.ToList();
        }

        private void LoadUniqueDistricts()
        {
            // Recolectar distritos �nicos
            Districts = _context.Otbs
                .Select(o => o.District) // Aseg�rate de que este sea el nombre del atributo
                .Distinct()
                .ToList();
        }

        public JsonResult OnGetOtbsByDistrict(int districtId)
        {
            var filteredOtbs = _context.Otbs
                .Where(o => o.District == districtId)
                .Select(o => new { o.OtbId, o.Name })
                .ToList();

            return new JsonResult(filteredOtbs);
        }
    }
}
