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
            return await db.QueryAsync<Child>("SELECT CAST(Id AS UNIQUEIDENTIFIER) AS Id, CAST(UserId AS UNIQUEIDENTIFIER) AS UserId,  CAST(TrajectId AS UNIQUEIDENTIFIER) as TrajectId, ArtsName, Name, PrefabId FROM Child WHERE UserId = @UserId", new { UserId = userId });
        }
    }
    
    public async Task Add(Child child)
    {
        using (IDbConnection db = new SqlConnection(connectionString))
        {
            var sql = "INSERT INTO Child (Id, UserId, TrajectId, ArtsName, Name, PrefabId) VALUES (@Id, @UserId, @TrajectId, @ArtsName, @Name, @PrefabId)";
            await db.ExecuteAsync(sql, child);
        }
    }
    
    public async Task Update(Child child)
    {
        using (IDbConnection db = new SqlConnection(connectionString))
        {
            var sql = "UPDATE Child SET UserId = @UserId, TrajectId = @TrajectId, ArtsName = @ArtsName, Name = @Name, PrefabId = @PrefabId WHERE Id = @Id";
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