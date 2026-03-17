using System.Collections.Generic;

namespace Dental_Clinic.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalPatients { get; set; }
        public int TotalAppointmentsToday { get; set; }
        public int PendingAppointments { get; set; }
        public int TotalCases { get; set; }
        public List<Appointment> TodayAppointments { get; set; } = new();
    }

    public class MediaViewModel
    {
        public List<string> ClinicImages { get; set; } = new();
        public List<string> DoctorImages { get; set; } = new();
    }
}
