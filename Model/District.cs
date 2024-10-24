using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CisternasGAMC.Model
{
    [NotMapped]
    public class District
    {
        public int DistrictId { get; set; }
        public string ImageRoute { get; set; }
        public string PdfRoute { get; set; }

        public District(int districtId, string imageRoute, string pdf)
        {
            DistrictId = districtId;
            ImageRoute = imageRoute;
            PdfRoute = pdf;
        }
    }

    
}
