using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CisternasGAMC.Pages.Citizen.Districts
{
    public class DetailsModel : PageModel
    {
        public int districtId { get; set; }
        public string docRoute { get; set; }

        public void OnGet(int id, string pdfRoute)
        {
            districtId = id;
            docRoute = pdfRoute;
        }
    }
}
