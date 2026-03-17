using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental_Clinic.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "المريض مطلوب")]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }

        [Required(ErrorMessage = "تاريخ الموعد مطلوب")]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ الموعد")]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "وقت البدء مطلوب")]
        [DataType(DataType.Time)]
        [Display(Name = "وقت البدء")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "وقت الانتهاء مطلوب")]
        [DataType(DataType.Time)]
        [Display(Name = "وقت الانتهاء")]
        public TimeSpan EndTime { get; set; } // Usually StartTime + 30 mins

        [Display(Name = "حالة الموعد")]
        public AppointmentStatus Status { get; set; }

        [Display(Name = "وقت الانتظار المتوقع (بالدقائق)")]
        public int ExpectedWaitTime { get; set; }
    }

    public enum AppointmentStatus
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled
    }
}
