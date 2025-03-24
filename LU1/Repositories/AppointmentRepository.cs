using System;
using Dapper;
using LU1.Models;
using Microsoft.Data.SqlClient;

namespace LU1.Repositories
{
    public class AppointmentRepository(string _connectionString)
    {
        string ReturnedChildId;
        public async Task<IEnumerable<AppointmentItem>> GetByUserIdAndChildName(string userId, string childName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                try
                {
                    ReturnedChildId = "SELECT id FROM Child WHERE userId = @UserId AND WHERE name = @Name";
                }
                catch (NullReferenceException)
                {
                    return null;
                }

                var query = "SELECT * FROM Appointments WHERE childId = @ChildId";
                var result = await connection.QueryAsync<AppointmentItem>(query, new {ChildId = ReturnedChildId });
                return result;

            }
        }
        public async Task Add(AppointmentItem appointment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "INSERT INTO Appointments (Id, UserId, TrajectId, ArtsName, Name, PrefabId) VALUES (@Id, @UserId, @TrajectId, @ArtsName, @Name, @PrefabId)";
                await connection.ExecuteAsync(query, appointment);
            }
        }
        public async Task Update(AppointmentItem appointment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "UPDATE Appointments SET UserId = @UserId, TrajectId = @TrajectId, ArtsName = @ArtsName, Name = @Name, PrefabId = @PrefabId WHERE Id = @Id";
                await connection.ExecuteAsync(query, appointment);
            }
        }
        public async Task Delete(string id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "DELETE FROM Appointments WHERE Id = @Id";
                await connection.ExecuteAsync(query, new { Id = id });
            }
        }
    }
}
