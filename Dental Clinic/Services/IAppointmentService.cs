using Dental_Clinic.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dental_Clinic.Services
{
    public interface IAppointmentService
    {
        Task<List<TimeSpan>> GetAvailableTimeSlotsAsync(DateTime date);
        Task<Appointment> BookAppointmentAsync(int patientId, DateTime date, TimeSpan startTime);
        Task<List<Appointment>> GetAppointmentsByDateAsync(DateTime date);
        Task<int> CalculateExpectedWaitTimeAsync(DateTime date, TimeSpan startTime);
    }
}
