using System.Collections.Generic;

namespace Dental_Clinic.Models
{
    public class HomeViewModel
    {
        public List<TreatmentCase> Cases { get; set; }
        public string QRCodeBase64 { get; set; }
    }
}
