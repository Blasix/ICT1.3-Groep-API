using System;
using Dapper;
using LU1.Models;
using Microsoft.Data.SqlClient;

namespace LU1.Repositories
{
    public class AppointmentRepository(string _connectionString)
    {
        public async Task<string> GetChildByUserIdAndChildName(string userId, string childName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var returnChildQuery = "SELECT id FROM Child WHERE userId = @UserId AND name = @Name";
                    var resultChild = await connection.QueryFirstOrDefaultAsync<string>(returnChildQuery, new { UserId = userId, Name = childName });
                    return resultChild;
                }
                catch (NullReferenceException)
                {
                    return null;
                }
            }
        }

        public async Task<IEnumerable<AppointmentItem>> GetAppointmentsByUserIdAndChildName(string userId, string childName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                try
                {
                    var returnChildQuery = "SELECT id FROM Child WHERE userId = @UserId AND name = @Name";
                    var resultChild = await connection.QueryFirstOrDefaultAsync<string>(returnChildQuery, new { UserId = userId, Name = childName });
                    if (resultChild == null)
                    {
                        return null;
                    }
                    var getAppointmentQuery = "SELECT * FROM Appointment WHERE childId = @ChildId";
                    var result = await connection.QueryAsync<AppointmentItem>(getAppointmentQuery, new { ChildId = resultChild });
                    return result;
                }
                catch (NullReferenceException)
                {
                    return null;
                }
            }
        }

        public async Task<AppointmentItem> GetAppointmentIdByUserIdChildNameAndAppointmentName(string userId, string childName, string appointmentName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                try
                {
                    var returnChildQuery = "SELECT id FROM Child WHERE userId = @UserId AND name = @Name";
                    var resultChild = await connection.QueryFirstOrDefaultAsync<string>(returnChildQuery, new { UserId = userId, Name = childName });

                    if (resultChild == null)
                    {
                        return null;
                    }

                    var getAppointmentQuery = "SELECT * FROM Appointment WHERE childId = @ChildId AND name = @AppointmentName";
                    var result = await connection.QueryFirstOrDefaultAsync<AppointmentItem>(getAppointmentQuery, new { ChildId = resultChild, AppointmentName = appointmentName });
                    return result;
                }
                catch (NullReferenceException)
                {
                    return null;
                }
            }
        }

        public async Task Add(AppointmentItem appointment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "INSERT INTO Appointment (id, name, date, childId, levelId, statusLevel, LevelStep) VALUES (@Id, @AppointmentName, @AppointmentDate, @ChildId, @LevelId, @StatusLevel, @LevelStep)";
                await connection.ExecuteAsync(query, appointment);
            }
        }

        public async Task Delete(string UserId, string childName, string AppointmentId)
        {
            string childId = await GetChildByUserIdAndChildName(UserId, childName);
            if (childId != null)
            {
                return;
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "DELETE FROM Appointments WHERE childId = @ChildId AND id = @AppointmentId";
                await connection.ExecuteAsync(query, new { ChildId = childId, AppointmentId = AppointmentId });
            }
        }
    }
}
