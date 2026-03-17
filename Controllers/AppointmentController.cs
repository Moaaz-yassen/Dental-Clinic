using Dental_Clinic.Data;
using Dental_Clinic.Models;
using Dental_Clinic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dental_Clinic.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ApplicationDbContext _context;

        public AppointmentController(IAppointmentService appointmentService, ApplicationDbContext context)
        {
            _appointmentService = appointmentService;
            _context = context;
        }

        // GET: Appointment/Book
        public IActionResult Book()
        {
            return View();
        }

        // POST: Appointment/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(Patient patient, DateTime AppointmentDate, TimeSpan StartTime)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if patient exists by phone, else create
                    var existingPatient = await _context.Patients.FirstOrDefaultAsync(p => p.Phone == patient.Phone);
                    if (existingPatient == null)
                    {
                        _context.Patients.Add(patient);
                        await _context.SaveChangesAsync();
                        existingPatient = patient;
                    }

                    // Book the appointment
                    var appointment = await _appointmentService.BookAppointmentAsync(existingPatient.Id, AppointmentDate, StartTime);

                    TempData["SuccessMessage"] = $"تم حجز الموعد بنجاح! وقت الانتظار المتوقع: {appointment.ExpectedWaitTime} دقيقة.";
                    return RedirectToAction("Success");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(patient);
        }

        // GET: /Appointment/Success
        public IActionResult Success()
        {
            return View();
        }

        // API Endpoint to get available times for AJAX call
        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(DateTime date)
        {
            var slots = await _appointmentService.GetAvailableTimeSlotsAsync(date);
            var formattedSlots = slots.Select(s => new {
                value = s.ToString(),
                text = new DateTime(date.Year, date.Month, date.Day, s.Hours, s.Minutes, s.Seconds).ToString("hh:mm tt")
            });
            return Json(formattedSlots);
        }

        // --- Hidden Reservation System Endpoints ---

        [HttpPost]
        public IActionResult AuthenticateDoctor([FromBody] string password)
        {
            if (password == "55555")
            {
                HttpContext.Session.SetString("IsDoctorLoggedIn", "true");
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "كلمة المرور غير صحيحة" });
        }

        [HttpGet]
        public async Task<IActionResult> ManageAppointments()
        {
            var isLoggedIn = HttpContext.Session.GetString("IsDoctorLoggedIn");
            if (isLoggedIn != "true")
            {
                return Unauthorized();
            }

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();

            return PartialView("_ManageAppointments", appointments);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var isLoggedIn = HttpContext.Session.GetString("IsDoctorLoggedIn");
            if (isLoggedIn != "true")
            {
                return Unauthorized();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "لم يتم العثور على الحجز" });
        }

        [HttpPost]
        public async Task<IActionResult> EditAppointmentTime(int id, TimeSpan newTime)
        {
            var isLoggedIn = HttpContext.Session.GetString("IsDoctorLoggedIn");
            if (isLoggedIn != "true")
            {
                return Unauthorized();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.StartTime = newTime;
                appointment.EndTime = newTime.Add(TimeSpan.FromMinutes(30)); // Assuming 30 min duration
                
                // Recalculate Expected Wait Time if necessary, but here we just update the DB
                _context.Appointments.Update(appointment);
                await _context.SaveChangesAsync();
                
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "لم يتم العثور على الحجز" });
        }
    }
}
