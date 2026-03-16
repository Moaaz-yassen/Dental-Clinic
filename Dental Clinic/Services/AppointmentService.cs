using Dental_Clinic.Data;
using Dental_Clinic.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dental_Clinic.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly TimeSpan _clinicOpenTime = new TimeSpan(16, 0, 0); // 4:00 PM
        private readonly TimeSpan _clinicCloseTime = new TimeSpan(22, 0, 0); // 10:00 PM
        private readonly TimeSpan _slotDuration = TimeSpan.FromMinutes(30);

        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TimeSpan>> GetAvailableTimeSlotsAsync(DateTime date)
        {
            var availableSlots = new List<TimeSpan>();

            if (date.DayOfWeek == DayOfWeek.Friday || date.Date < DateTime.Today)
            {
                return availableSlots; // Friday is closed, or date is in the past
            }

            var bookedAppointments = await _context.Appointments
                .Where(a => a.AppointmentDate.Date == date.Date && a.Status != AppointmentStatus.Cancelled)
                .Select(a => a.StartTime)
                .ToListAsync();

            var currentTime = _clinicOpenTime;

            // If it's today, only show future slots
            if (date.Date == DateTime.Today && DateTime.Now.TimeOfDay > _clinicOpenTime)
            {
                // Find the next available 30-min slot after current time
                var now = DateTime.Now.TimeOfDay;
                var mins = now.Minutes;
                var extraMins = mins <= 30 ? 30 - mins : 60 - mins;
                currentTime = now.Add(TimeSpan.FromMinutes(extraMins));
                currentTime = new TimeSpan(currentTime.Hours, currentTime.Minutes, 0);
            }

            while (currentTime + _slotDuration <= _clinicCloseTime)
            {
                if (!bookedAppointments.Contains(currentTime))
                {
                    availableSlots.Add(currentTime);
                }
                currentTime = currentTime.Add(_slotDuration);
            }

            return availableSlots;
        }

        public async Task<Appointment> BookAppointmentAsync(int patientId, DateTime date, TimeSpan startTime)
        {
            // Check if available
            var availableSlots = await GetAvailableTimeSlotsAsync(date);
            if (!availableSlots.Contains(startTime))
            {
                throw new InvalidOperationException("هذا الموعد غير متاح.");
            }

            var expectedWait = await CalculateExpectedWaitTimeAsync(date, startTime);

            var appointment = new Appointment
            {
                PatientId = patientId,
                AppointmentDate = date.Date,
                StartTime = startTime,
                EndTime = startTime.Add(_slotDuration),
                Status = AppointmentStatus.Confirmed,
                ExpectedWaitTime = expectedWait
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return appointment;
        }

        public async Task<List<Appointment>> GetAppointmentsByDateAsync(DateTime date)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.AppointmentDate.Date == date.Date)
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<int> CalculateExpectedWaitTimeAsync(DateTime date, TimeSpan startTime)
        {
            // Simple logic: If there are earlier appointments, calculate buffer base
            // In a real scenario, this would depend on the actual progress of the clinic
            var priorAppointmentsCount = await _context.Appointments
                .Where(a => a.AppointmentDate.Date == date.Date && a.StartTime < startTime && a.Status != AppointmentStatus.Cancelled)
                .CountAsync();

            // Example: Add 5 minutes expected wait for each prior appointment (just a simple buffer)
            return priorAppointmentsCount * 5;
        }
    }
}
