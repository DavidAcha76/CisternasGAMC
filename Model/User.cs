using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CisternasGAMC.Model
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short UserId { get; set; }

        [StringLength(30, ErrorMessage = "El nombre no debe exceder los 30 caracteres.")]
        [Required(ErrorMessage = "FirstName es obligatorio.")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(40, ErrorMessage = "El apellido no debe exceder los 40 caracteres.")]
        [Required(ErrorMessage = "LastName es obligatorio.")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(30, ErrorMessage = "El email no debe exceder los 30 caracteres.")]
        [Required(ErrorMessage = "Email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        public string Email { get; set; } = string.Empty;

        [StringLength(70, ErrorMessage = "La contraseña no debe exceder los 70 caracteres.")]
        [Required(ErrorMessage = "Password es obligatorio.")]
        public string Password { get; set; } = string.Empty;

        [StringLength(8, ErrorMessage = "El teléfono debe tener exactamente 8 caracteres.")]
        [Required(ErrorMessage = "PhoneNumber es obligatorio.")]
        public string PhoneNumber { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado es obligatorio.")]
        public byte Status { get; set; }
    }
}