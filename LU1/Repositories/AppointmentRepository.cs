using LU1_1._3.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace LU1_1._3.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly string _sqlConnectionString;

        public AppointmentRepository(string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
        }

        public async Task<List<Appointment>> GetAllAsync()
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Appointment";

                var raw = await connection.QueryAsync<dynamic>(query);

                var appointments = new List<Appointment>();

                foreach (var data in raw)
                {
                    var appointment = new Appointment
                    {
                        Id = data.Id != null ? Guid.Parse(data.Id.ToString()) : Guid.Empty,
                        Name = data.Name,
                        Date = data.Date,
                        Url = data.Url,
                        Image = data.Image,
                        ChildId = data.ChildId != null ? Guid.Parse(data.ChildId.ToString()) : Guid.Empty
                    };

                    appointments.Add(appointment);
                }

                return appointments;
            }
        }

        public async Task<Appointment> GetByIdAsync(string id)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Appointment WHERE Id = @Id";

                var raw = await connection.QueryFirstOrDefaultAsync<dynamic>(query, new { Id = Guid.Parse(id) });

                if (raw == null)
                {
                    return null; // Or handle this scenario as needed (e.g., throw exception)
                }

                return new Appointment
                {
                    Id = raw.Id != null ? Guid.Parse(raw.Id.ToString()) : Guid.Empty,
                    Name = raw.Name,
                    Date = raw.Date,
                    Url = raw.Url,
                    Image = raw.Image,
                    ChildId = raw.ChildId != null ? Guid.Parse(raw.ChildId.ToString()) : Guid.Empty
                };
            }
        }

        public async Task<List<Appointment>> GetByChildIdAsync(string childId)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Appointment WHERE ChildId = @ChildId";

                var raw = await connection.QueryAsync<dynamic>(query, new { ChildId = Guid.Parse(childId) });

                var appointments = new List<Appointment>();

                foreach (var data in raw)
                {
                    var appointment = new Appointment
                    {
                        Id = data.Id != null ? Guid.Parse(data.Id.ToString()) : Guid.Empty,
                        Name = data.Name,
                        Date = data.Date,
                        Url = data.Url,
                        Image = data.Image,
                        ChildId = data.ChildId != null ? Guid.Parse(data.ChildId.ToString()) : Guid.Empty
                    };

                    appointments.Add(appointment);
                }

                return appointments;
            }
        }

        public async Task<Appointment> CreateAsync(Appointment appointment)
        {
            appointment.Id = Guid.NewGuid(); // Generate a new Guid for the Id

            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO Appointment (Id, Name, Date, Url, Image, ChildId) VALUES (@Id, @Name, @Date, @Url, @Image, @ChildId)";

                await connection.ExecuteAsync(query, new
                {
                    appointment.Id,
                    appointment.Name,
                    appointment.Date,
                    appointment.Url,
                    appointment.Image,
                    appointment.ChildId
                });
            }

            return appointment;
        }

        public async Task DeleteAsync(string id)
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM Appointment WHERE Id = @Id";
                await connection.ExecuteAsync(query, new { Id = Guid.Parse(id) });
            }
        }
    }
}
