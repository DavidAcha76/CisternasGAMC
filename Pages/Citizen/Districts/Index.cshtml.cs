using CisternasGAMC.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CisternasGAMC.Pages.Citizen.Districts
{
    public class IndexModel : PageModel
    {
        public IList<District> districts { get; set; } = new List<District>();
        public void OnGet()
        {
            GetDistricts();
        }

        public IActionResult Details(int id)
        {
            string pdfRoute = districts.FirstOrDefault(d => d.DistrictId == id)?.PdfRoute;

            if (pdfRoute == null)
            {
                return NotFound();
            }

            return RedirectToPage("Details", new { id = id, pdfRoute = pdfRoute });
        }

        public void GetDistricts()
        {
            District distrito1 = new District(1,Url.Content("~/images/Distritos/Distrito1.png"),Url.Content("~/docs/DISTRITO 01.pdf"));
            District distrito2 = new District(2,Url.Content("~/images/Distritos/Distrito2.png"),Url.Content("~/docs/DISTRITO 02.pdf"));
            District distrito3 = new District(3,Url.Content("~/images/Distritos/Distrito3.png"),Url.Content("~/docs/DISTRITO 03.pdf"));
            District distrito4 = new District(4,Url.Content("~/images/Distritos/Distrito4.png"),Url.Content("~/docs/DISTRITO 04.pdf"));
            District distrito5 = new District(5,Url.Content("~/images/Distritos/Distrito5.png"),Url.Content("~/docs/DISTRITO 05.pdf"));
            District distrito6 = new District(6,Url.Content("~/images/Distritos/Distrito6.png"),Url.Content("~/docs/DISTRITO 06.pdf"));
            District distrito7 = new District(7,Url.Content("~/images/Distritos/Distrito7.png"),Url.Content("~/docs/DISTRITO 07.pdf"));
            District distrito8 = new District(8,Url.Content("~/images/Distritos/Distrito8.png"),Url.Content("~/docs/DISTRITO 08.pdf"));
            District distrito9 = new District(9,Url.Content("~/images/Distritos/Distrito9.png"),Url.Content("~/docs/DISTRITO 09.pdf"));
            District distrito10 = new District(10,Url.Content("~/images/Distritos/Distrito10.png"),Url.Content("~/docs/DISTRITO 10.pdf"));
            District distrito11 = new District(11,Url.Content("~/images/Distritos/Distrito11.png"),Url.Content("~/docs/DISTRITO 11.pdf"));
            District distrito12 = new District(12,Url.Content("~/images/Distritos/Distrito12.png"),Url.Content("~/docs/DISTRITO 12.pdf"));
            District distrito13 = new District(13,Url.Content("~/images/Distritos/Distrito13.png"),Url.Content("~/docs/DISTRITO 13.pdf"));
            District distrito14 = new District(14,Url.Content("~/images/Distritos/Distrito14.png"),Url.Content("~/docs/DISTRITO 14.pdf"));
            District distrito15 = new District(15,Url.Content("~/images/Distritos/Distrito15.png"),Url.Content("~/docs/DISTRITO 15.pdf"));

            districts.Add(distrito1);
            districts.Add(distrito2);
            districts.Add(distrito3);
            districts.Add(distrito4);
            districts.Add(distrito5);
            districts.Add(distrito6);
            districts.Add(distrito7);
            districts.Add(distrito8);
            districts.Add(distrito9);
            districts.Add(distrito10);
            districts.Add(distrito11);
            districts.Add(distrito12);
            districts.Add(distrito13);
            districts.Add(distrito14);
            districts.Add(distrito15);
        }
    }
}
