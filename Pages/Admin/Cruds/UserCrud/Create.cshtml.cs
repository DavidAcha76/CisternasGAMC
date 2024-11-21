using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CisternasGAMC.Data;
using CisternasGAMC.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Telegram.Bot.Types;

namespace CisternasGAMC.Pages.Admin.Cruds.UserCrud
{
    [Authorize(Roles = "admin")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Model.User User { get; set; } = default!;

        public IEnumerable<SelectListItem> Otbs { get; set; } = new List<SelectListItem>();

        public IActionResult OnGet()
        {
            PopulateOtbsList(); // Cargar la lista de OTBs al cargar la página
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Inicializa el campo Password con un valor predeterminado basado en el número de teléfono
            if (string.IsNullOrEmpty(User.Password))
            {
                User.Password = BCrypt.Net.BCrypt.HashPassword(User.PhoneNumber);
            }

            if (!ModelState.IsValid)
            {
                // Depurar errores en el ModelState
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Error en {key}: {error.ErrorMessage}");
                    }
                }

                PopulateOtbsList(); // Recargar la lista de OTBs en caso de errores de validación
                return Page();
            }

            // Validar duplicación de correo electrónico
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == User.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("User.Email", "Este correo electrónico ya está registrado. Por favor, utiliza uno diferente.");
                PopulateOtbsList(); // Recargar la lista de OTBs
                return Page();
            }

            // Validar que la OTB sea seleccionada si el rol es "president"
            if (User.Role == "president" && !User.OtbId.HasValue)
            {
                ModelState.AddModelError("User.OtbId", "Debe seleccionar una OTB si el rol es Presidente.");
                PopulateOtbsList();
                return Page();
            }

            // Validar si la OTB seleccionada existe
            if (User.OtbId.HasValue && !await _context.Otbs.AnyAsync(o => o.OtbId == User.OtbId))
            {
                ModelState.AddModelError("User.OtbId", "La OTB seleccionada no es válida.");
                PopulateOtbsList();
                return Page();
            }

            User.Status = 1;
            User.Password = BCrypt.Net.BCrypt.HashPassword(User.PhoneNumber);
            _context.Users.Add(User);

            // Enviar el correo con la nueva contraseña
            await EnviarCorreoNuevaContraseña(User.Email, User.PhoneNumber);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }





        private void PopulateOtbsList()
        {
            Otbs = _context.Otbs
                .Select(o => new SelectListItem
                {
                    Value = o.OtbId.ToString(),
                    Text = o.Name
                })
                .ToList();
        }
        private async Task EnviarCorreoNuevaContraseña(string email, string nuevaContraseña)
        {
            var fromAddress = new MailAddress("davidachagan@gmail.com", "CisternasGAMC");
            var toAddress = new MailAddress(email);
            const string fromPassword = "mdkj ogbv kwjm ztjq"; // Contraseña de aplicación de Gmail

            const string subject = "Tu nueva contraseña";
            string body = $@"
    <div style=""font-family: Arial, sans-serif; color: #019084;"">
    <h2 style=""color: #05c7b9;"">Tu nueva contraseña ha sido generada</h2>
    <p>Hola,</p>
    <p>Nos complace informarte que tu nueva contraseña ha sido creada con éxito:</p>
    <div style=""background-color: #03a396; padding: 10px; margin: 10px 0; border-radius: 5px; color: #fff;"">
    <p style=""font-size: 18px; text-align: center; font-weight: bold;"">{nuevaContraseña}</p>
    </div>
    <p>Por favor, <a href=""#"" style=""color: #05c7b9; text-decoration: none;"">inicia sesión</a> y asegúrate de cambiarla por una que recuerdes.</p>
    <p>Si no solicitaste este cambio, por favor ponte en contacto con nuestro equipo de soporte inmediatamente.</p>
    <br>
    <p>Gracias por confiar en nosotros.</p>
    <p><strong style=""color: #007e72;"">Equipo de CisternasGAMC</strong></p>
    </div>";

            using (var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            })
            {
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    await smtp.SendMailAsync(message);
                }
            }
        }
    }
}
