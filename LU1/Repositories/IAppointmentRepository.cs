using LU1_1._3.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LU1_1._3.Repositories
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetAllAsync();
        Task<Appointment> GetByIdAsync(string id);
        Task<List<Appointment>> GetByChildIdAsync(string childId);
        Task<Appointment> CreateAsync(Appointment appointment);
        Task DeleteAsync(string id);
    }
}
