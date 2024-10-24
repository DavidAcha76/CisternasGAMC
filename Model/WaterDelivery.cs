using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CisternasGAMC.Model
{
    public class WaterDelivery
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WaterDeliveryId { get; set; }

        [Required(ErrorMessage = "El ID de la cisterna es obligatorio.")]
        [ForeignKey(nameof(Cistern))]
        public byte CisternId { get; set; }
        public Cistern? Cistern { get; set; } = default!;

        [Required(ErrorMessage = "El ID del conductor es obligatorio.")]
        [ForeignKey(nameof(User))]
        public short DriverId { get; set; }
        public User? Driver { get; set; } = default!;

        [Required(ErrorMessage = "El ID de la OTB es obligatorio.")]
        [ForeignKey(nameof(Otb))]
        public short OtbId { get; set; }
        public Otb? Otb { get; set; } = default!;

        [Required(ErrorMessage = "La fecha de entrega es obligatoria.")]
        [DataType(DataType.DateTime)]
        public DateTime DeliveryDate { get; set; }

        [Range(0.1, float.MaxValue, ErrorMessage = "La cantidad entregada debe ser positiva.")]
        public float? DeliveredAmount { get; set; }

        [Required(ErrorMessage = "El estado de entrega es obligatorio.")]
        public byte DeliveryStatus { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ArrivalDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DepartureDate { get; set; }
    }
}
