using Dental_Clinic.Data;
using Dental_Clinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dental_Clinic.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ==================== DASHBOARD ====================
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var vm = new AdminDashboardViewModel
            {
                TotalPatients = await _context.Patients.CountAsync(),
                TotalAppointmentsToday = await _context.Appointments
                    .Where(a => a.AppointmentDate.Date == today).CountAsync(),
                PendingAppointments = await _context.Appointments
                    .Where(a => a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Confirmed).CountAsync(),
                TotalCases = await _context.TreatmentCases.CountAsync(),
                TodayAppointments = await _context.Appointments
                    .Include(a => a.Patient)
                    .Where(a => a.AppointmentDate.Date == today)
                    .OrderBy(a => a.StartTime)
                    .ToListAsync()
            };
            return View(vm);
        }

        // ==================== APPOINTMENTS ====================
        public async Task<IActionResult> Appointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
            return View(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, AppointmentStatus status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = status;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Appointments));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Appointments));
        }

        // ==================== TREATMENT CASES ====================
        public async Task<IActionResult> Cases()
        {
            var cases = await _context.TreatmentCases.ToListAsync();
            return View(cases);
        }

        [HttpGet]
        public IActionResult AddCase()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCase(TreatmentCase model, IFormFile? beforeImage, IFormFile? afterImage)
        {
            if (ModelState.IsValid)
            {
                if (beforeImage != null && beforeImage.Length > 0)
                    model.BeforeImagePath = await SaveImage(beforeImage, "Cases");

                if (afterImage != null && afterImage.Length > 0)
                    model.AfterImagePath = await SaveImage(afterImage, "Cases");

                _context.TreatmentCases.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم إضافة الحالة بنجاح!";
                return RedirectToAction(nameof(Cases));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditCase(int id)
        {
            var c = await _context.TreatmentCases.FindAsync(id);
            if (c == null) return NotFound();
            return View(c);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCase(TreatmentCase model, IFormFile? beforeImage, IFormFile? afterImage)
        {
            if (ModelState.IsValid)
            {
                var existing = await _context.TreatmentCases.FindAsync(model.Id);
                if (existing == null) return NotFound();

                existing.Title = model.Title;
                existing.Description = model.Description;

                if (beforeImage != null && beforeImage.Length > 0)
                    existing.BeforeImagePath = await SaveImage(beforeImage, "Cases");

                if (afterImage != null && afterImage.Length > 0)
                    existing.AfterImagePath = await SaveImage(afterImage, "Cases");

                await _context.SaveChangesAsync();
                TempData["Success"] = "تم تعديل الحالة بنجاح!";
                return RedirectToAction(nameof(Cases));
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCase(int id)
        {
            var c = await _context.TreatmentCases.FindAsync(id);
            if (c != null)
            {
                _context.TreatmentCases.Remove(c);
                await _context.SaveChangesAsync();
            }
            TempData["Success"] = "تم حذف الحالة.";
            return RedirectToAction(nameof(Cases));
        }

        // ==================== MEDIA (Clinic / Doctor Photos) ====================
        public IActionResult Media()
        {
            var clinicImages = GetImagesInFolder("Clinic");
            var doctorImages = GetImagesInFolder("Doctor");
            var vm = new MediaViewModel { ClinicImages = clinicImages, DoctorImages = doctorImages };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UploadMedia(IFormFile image, string folder)
        {
            if (image != null && image.Length > 0 && (folder == "Clinic" || folder == "Doctor"))
            {
                await SaveImage(image, folder);
                TempData["Success"] = "تم رفع الصورة بنجاح!";
            }
            return RedirectToAction(nameof(Media));
        }

        [HttpPost]
        public IActionResult DeleteMedia(string fileName, string folder)
        {
            var path = Path.Combine(_env.WebRootPath, "Images", folder, fileName);
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            TempData["Success"] = "تم حذف الصورة.";
            return RedirectToAction(nameof(Media));
        }

        // ==================== HELPERS ====================
        private async Task<string> SaveImage(IFormFile file, string folder)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "Images", folder);
            Directory.CreateDirectory(uploadsFolder);
            var uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueName);
            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);
            return $"/Images/{folder}/{uniqueName}";
        }

        private System.Collections.Generic.List<string> GetImagesInFolder(string folder)
        {
            var folderPath = Path.Combine(_env.WebRootPath, "Images", folder);
            if (!Directory.Exists(folderPath)) return new();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            return Directory.GetFiles(folderPath)
                .Where(f => allowed.Contains(Path.GetExtension(f).ToLower()))
                .Select(f => $"/Images/{folder}/{Path.GetFileName(f)}")
                .ToList();
        }
    }
}
