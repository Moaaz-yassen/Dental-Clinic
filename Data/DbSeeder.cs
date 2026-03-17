using Dental_Clinic.Data;
using Dental_Clinic.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Dental_Clinic
{
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            context.Database.Migrate();

            if (!context.TreatmentCases.Any())
            {
                context.TreatmentCases.AddRange(
                    new TreatmentCase
                    {
                        Title = "تسوس وبروز فى الأسنان الأمامية",
                        Description = "صورة قبل المعركة توضح تسوس جانب اللثة و تسوس بينى فى الأسنان الاماميه و بروز للسن الثانى جهة اليسار و اصفرار فى السن الأول اليمين و شرخ طولى نتيجة للجز علي الأسنان. الصورة النهائية بعد الانتهاء مباشره والأهم الشكل الطبيعى والمريح للعين.",
                        BeforeImagePath = "/Images/Cases/case_new_before.jpg",
                        AfterImagePath = "/Images/Cases/case_new_after.jpg"
                    },
                    new TreatmentCase
                    {
                        Title = "تعويض السن المكسور",
                        Description = "سنه مكسوره مفقود فيها الأمل ! بس الجذر قوى اهم حاجه ❤️ بإذن الله نبنيها من اول و جديد بالحشو التجميلي فقط. بفضل الله تم تجميل الأسنان الاماميه و تعويض السن المكسور بالدعامة و الحشو التجميلي فقط بدون اي تركيبات.",
                        BeforeImagePath = "/Images/Cases/case1.jpg",
                        AfterImagePath = "/Images/Cases/case1.jpg"
                    },
                    new TreatmentCase
                    {
                        Title = "ترميم الأسنان الأمامية",
                        Description = "حالة من عيادتنا المتواضعة لترميم الأسنان الأمامية بحشوات مضيئة.",
                        BeforeImagePath = "/Images/Cases/case2.jpg",
                        AfterImagePath = "/Images/Cases/case2.jpg"
                    },
                    new TreatmentCase
                    {
                        Title = "علاج التسوس الظاهري",
                        Description = "تسوس ظاهري فى القواطع الأمامية - تمت الازاله باستخدام الحشوات التجميلية و توحيد لون السن. معانا أسنانك هترجع تضحك من تاني ❤️.",
                        BeforeImagePath = "/Images/Cases/case3.jpg",
                        AfterImagePath = "/Images/Cases/case3.jpg"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
