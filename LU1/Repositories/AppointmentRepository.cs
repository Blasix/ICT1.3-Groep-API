using System;
using System.Diagnostics;
using Dapper;
using LU1.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;

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
                    var getAppointmentQuery = "SELECT id, name AS appointmentName, date, childId, levelId, statusLevel, LevelStep FROM Appointment WHERE childId = @ChildId";
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
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Appointment (id, name, date, childId, levelId, statusLevel, LevelStep) VALUES (@Id, @Name, @Date, @ChildId, @LevelId, @StatusLevel, @LevelStep)";
                    command.Parameters.AddWithValue("@Id", appointment.id);
                    command.Parameters.AddWithValue("@Name", appointment.appointmentName);
                    command.Parameters.AddWithValue("@Date", DateTime.Parse(appointment.date));
                    command.Parameters.AddWithValue("@ChildId", appointment.childId);
                    command.Parameters.AddWithValue("@LevelId", appointment.levelId);
                    command.Parameters.AddWithValue("@StatusLevel", "incompleted");
                    command.Parameters.AddWithValue("@LevelStep", appointment.LevelStep);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public async Task Put(string statusLevel, int step, string childId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                UPDATE Appointment 
                SET statusLevel = @statusLevel
                WHERE ChildId = @ChildId AND LevelStep = @LevelStep";
                   
                    if(statusLevel == "completed")
                    {
                    command.Parameters.AddWithValue("@statusLevel", "completed");
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@statusLevel", "doing");
                    }


                    command.Parameters.AddWithValue("@ChildId", childId);
                    command.Parameters.AddWithValue("@LevelStep", step);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }



        public async Task Delete(string UserId, string childName, string AppointmentId)
        {
            string childId = await GetChildByUserIdAndChildName(UserId, childName);
            string childIdTest = childId;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "DELETE FROM Appointment WHERE childId = @ChildId AND id = @AppointmentId";
                await connection.ExecuteAsync(query, new { ChildId = childId, AppointmentId = AppointmentId });
            }
        }
    }
}
