using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CisternasGAMC.Pages.Admin
{
    [Authorize(Roles = "admin")]

    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
