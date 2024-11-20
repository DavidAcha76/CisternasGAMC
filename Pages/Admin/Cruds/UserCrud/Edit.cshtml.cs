using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CisternasGAMC.Data;
using CisternasGAMC.Model;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace CisternasGAMC.Pages.Admin.Cruds.UserCrud
{
    [Authorize(Roles = "admin")]
    public class EditModel : PageModel
    {
        private readonly CisternasGAMC.Data.ApplicationDbContext _context;
        
        public EditModel(CisternasGAMC.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user =  await _context.Users.FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            User = user;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //User.Password = BCrypt.Net.BCrypt.HashPassword(User.Password);

            _context.Entry(User).Property(u => u.Password).IsModified = false;

            _context.Attach(User).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/Admin/Index");
        }

        private bool UserExists(short id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        public async Task<IActionResult> OnPostSendPasswordResetAsync()
        {
            // Obtener el valor enviado desde el formulario y convertirlo a string
            string emailReset = Request.Form["emailReset"].ToString();

            if (string.IsNullOrEmpty(emailReset))
            {
                ModelState.AddModelError(string.Empty, "Debes ingresar un correo electrónico válido.");
                return Page();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailReset);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Correo electrónico no encontrado.");
                return Page();
            }

            // Generar nueva contraseña
            string nuevaContraseña = GenerarContraseñaAleatoria(10);

            user.Password = BCrypt.Net.BCrypt.HashPassword(nuevaContraseña);
            await _context.SaveChangesAsync();

            // Enviar la nueva contraseña por correo
            await EnviarCorreoNuevaContraseña(user.Email, nuevaContraseña);

            TempData["SuccessMessage"] = "Se ha enviado una nueva contraseña a tu correo electrónico.";
            return RedirectToPage("/Admin/Index");
        }



        private string GenerarContraseñaAleatoria(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder result = new StringBuilder();
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    result.Append(validChars[(int)(num % (uint)validChars.Length)]);
                }
            }
            return result.ToString();
        }

        private async Task EnviarCorreoNuevaContraseña(string email, string nuevaContraseña)
        {
            // Dirección de origen y destino del correo
            var fromAddress = new MailAddress("davidachagan@gmail.com", "CisternasGAMC");
            var toAddress = new MailAddress(email);

            // Contraseña de la cuenta de Gmail (usa una contraseña de aplicación para mayor seguridad)
            const string fromPassword = "mdkj ogbv kwjm ztjq";

            // Asunto y cuerpo del correo
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

            try
            {
                // Configuración del cliente SMTP
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
                    // Creación del mensaje
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true // Indica que el cuerpo del mensaje es HTML
                    })
                    {
                        // Enviar el correo
                        await smtp.SendMailAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                throw new InvalidOperationException($"Error al enviar el correo: {ex.Message}", ex);
            }
        }

    }
}
