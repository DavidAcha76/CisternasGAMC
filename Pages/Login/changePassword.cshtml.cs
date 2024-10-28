using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CisternasGAMC.Data;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace CisternasGAMC.Pages.Login
{
    [Authorize(Roles = "admin y driver")]
    public class ChangePasswordModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ChangePasswordModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PasswordChangeModel ChangePassword { get; set; }

        public class PasswordChangeModel
        {
            [Required(ErrorMessage = "La contraseña antigua es obligatoria.")]
            public string OldPassword { get; set; }

            [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "La nueva contraseña debe tener al menos 6 caracteres.")]
            [RegularExpression(@"^(?=.*[0-9]).+$", ErrorMessage = "La nueva contraseña debe contener al menos un número.")]
            public string NewPassword { get; set; }

            [Required(ErrorMessage = "La confirmación de la nueva contraseña es obligatoria.")]
            [Compare("NewPassword", ErrorMessage = "La confirmación no coincide con la nueva contraseña.")]
            public string ConfirmNewPassword { get; set; }
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "El formulario no es válido.";
                return Page();
            }

            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null || !BCrypt.Net.BCrypt.Verify(ChangePassword.OldPassword, user.Password))
            {
                ModelState.AddModelError("ChangePassword.OldPassword", "La contraseña antigua es incorrecta.");
                return Page();
            }

            if (ChangePassword.NewPassword != ChangePassword.ConfirmNewPassword)
            {
                ModelState.AddModelError("ChangePassword.ConfirmNewPassword", "Las nuevas contraseñas no coinciden.");
                return Page();
            }

            // Encriptamos y guardamos la nueva contraseña
            user.Password = BCrypt.Net.BCrypt.HashPassword(ChangePassword.NewPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Cerramos la sesión después de cambiar la contraseña.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Message"] = "Contraseña cambiada con éxito.";
            return RedirectToPage("/Index");
        }
    }
}
