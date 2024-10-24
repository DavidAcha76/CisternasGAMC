using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CisternasGAMC.Model
{
    public class Otb
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short OtbId { get; set; }

        [StringLength(60, ErrorMessage = "El nombre no debe exceder los 60 caracteres.")]
        [Required(ErrorMessage = "Name es obligatorio.")]
        public string Name { get; set; } = string.Empty;

        [Range(1, 32767, ErrorMessage = "El número de familias debe ser positivo.")]
        [Required(ErrorMessage = "FamilyCount es obligatorio.")]
        public short FamilyCount { get; set; }

        [Required(ErrorMessage = "El distrito es obligatorio.")]
        public byte District { get; set; }
    }
}