using System.Data;
using Dapper;
using LU1.Models;
using Microsoft.Data.SqlClient;

namespace LU1.Repositories;

public class ChildRepository(string connectionString)
{
    public async Task<IEnumerable<Child>> GetByUserId(string userId)
    {
        using (IDbConnection db = new SqlConnection(connectionString))
        {
            return await db.QueryAsync<Child>("SELECT * FROM Child WHERE UserId = @UserId", new { UserId = userId });
        }
    }
    
    public async Task Add(Child child)
    {
        using (IDbConnection db = new SqlConnection(connectionString))
        {
            var sql = "INSERT INTO Child (Id, Name, Age, UserId) VALUES (@Id, @Name, @Age, @UserId)";
            await db.ExecuteAsync(sql, child);
        }
    }
    
    public async Task Update(Child child)
    {
        using (IDbConnection db = new SqlConnection(connectionString))
        {
            var sql = "UPDATE Child SET Name = @Name, Age = @Age WHERE Id = @Id";
            await db.ExecuteAsync(sql, child);
        }
    }
    
    public async Task Delete(string id)
    {
        using (IDbConnection db = new SqlConnection(connectionString))
        {
            var sql = "DELETE FROM Child WHERE Id = @Id";
            await db.ExecuteAsync(sql, new { Id = id });
        }
    }
}