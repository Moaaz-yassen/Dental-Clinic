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
    }
}
