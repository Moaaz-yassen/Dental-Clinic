using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب")]
        [StringLength(100)]
        [Display(Name = "اسم المريض")]
        public string Name { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        [StringLength(20)]
        [Display(Name = "رقم الهاتف")]
        public string Phone { get; set; }

        // Mapped to appointments
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
