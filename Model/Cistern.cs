using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CisternasGAMC.Model
{
    public class Cistern
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte CisternId { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        public byte Status { get; set; }

        [StringLength(7, ErrorMessage = "El número de placa debe tener entre 6 y 7 caracteres.", MinimumLength = 6)]
        [Required(ErrorMessage = "PlateNumber es obligatorio.")]
        public string PlateNumber { get; set; } = string.Empty;

        [Range(1, 32767, ErrorMessage = "La capacidad debe ser un número positivo.")]
        [Required(ErrorMessage = "La capacidad es obligatoria.")]
        public short Capacity { get; set; }
    }
}