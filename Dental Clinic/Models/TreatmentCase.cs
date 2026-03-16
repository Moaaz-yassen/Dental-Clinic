using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic.Models
{
    public class TreatmentCase
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان الحالة مطلوب")]
        [StringLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessage = "وصف الحالة مطلوب")]
        public string Description { get; set; }

        [Display(Name = "صورة قبل العلاج")]
        public string BeforeImagePath { get; set; }

        [Display(Name = "صورة بعد العلاج")]
        public string AfterImagePath { get; set; }
    }
}
